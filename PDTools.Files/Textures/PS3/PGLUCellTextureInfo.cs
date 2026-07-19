using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Pfim;

using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats;

using SixLabors.ImageSharp.PixelFormats;

using Syroot.BinaryData;

namespace PDTools.Files.Textures.PS3;

/// <summary>
/// PDI GL Cell (PS3) Texture Info. Thin wrapper over PS3 texture registers
/// </summary>
public class PGLUCellTextureInfo : PGLUTextureInfo
{
    public uint Head0 { get; set; }
    public uint Offset { get; set; }
    public byte MipmapLevelLast { get; set; }
    public CELL_GCM_TEXTURE_FORMAT FormatBits { get; set; }

    /// <summary>
    /// 1 = 1D
    /// 2 = 2D
    /// </summary>
    public CELL_GCM_TEXTURE_DIMENSION Dimension { get; set; } = CELL_GCM_TEXTURE_DIMENSION.CELL_GCM_TEXTURE_DIMENSION_2;

    public CELL_GCM_TEXTURE_BORDER Border { get; set; } = CELL_GCM_TEXTURE_BORDER.CELL_GCM_TEXTURE_BORDER_COLOR;
    public CELL_GCM_BOOL CubeMap { get; set; } = CELL_GCM_BOOL.CELL_GCM_FALSE;

    /// <summary>
    /// 0 = local memory
    /// 1 = main memory
    /// </summary>
    public CELL_GCM_LOCATION Location { get; set; } = CELL_GCM_LOCATION.CELL_GCM_LOCATION_LOCAL;

    public CELL_GCM_TEXTURE_ZFUNC ZFunc { get; set; } = CELL_GCM_TEXTURE_ZFUNC.CELL_GCM_TEXTURE_ZFUNC_NEVER;
    public byte Gamma { get; set; } = 0;
    public CELL_GCM_TEXTURE_WRAP WrapR { get; set; } = CELL_GCM_TEXTURE_WRAP.CELL_GCM_TEXTURE_CLAMP_TO_EDGE;
    public CELL_GCM_TEXTURE_UNSIGNED_REMAP UnsignedRemap { get; set; } = CELL_GCM_TEXTURE_UNSIGNED_REMAP.CELL_GCM_TEXTURE_UNSIGNED_REMAP_NORMAL;
    public CELL_GCM_TEXTURE_WRAP WrapT { get; set; } = CELL_GCM_TEXTURE_WRAP.CELL_GCM_TEXTURE_CLAMP_TO_EDGE;
    public byte AnisoBias { get; set; } = 0;
    public CELL_GCM_TEXTURE_WRAP WrapS { get; set; } = CELL_GCM_TEXTURE_WRAP.CELL_GCM_TEXTURE_CLAMP_TO_EDGE;
    public CELL_GCM_BOOL VertexTextureSamplerEnable { get; set; } = CELL_GCM_BOOL.CELL_GCM_TRUE;

    /// <summary>
    /// min LOD of texture reduction filter
    /// 12-bit unsigned fixed point value from 0 to 12
    /// </summary>
    public short LODMin { get; set; } = 0;

    /// <summary>
    /// max LOD of texture reduction filter
    /// 12-bit unsigned fixed point value from 0 to 12
    /// </summary>
    public short LODMax { get; set; } = 3840;

    public CELL_GCM_TEXTURE_MAX_ANISO MaxAniso { get; set; } = CELL_GCM_TEXTURE_MAX_ANISO.CELL_GCM_TEXTURE_MAX_ANISO_1;
    public CELL_GCM_BOOL AlphaKill { get; set; } = CELL_GCM_BOOL.CELL_GCM_FALSE;

    public CELL_GCM_TEXTURE_REMAP_ORDER RemapOrder { get; set; } = CELL_GCM_TEXTURE_REMAP_ORDER.CELL_GCM_TEXTURE_REMAP_ORDER_XYXY;
    public CELL_GCM_TEXTURE_REMAP_OUT OutB { get; set; } = CELL_GCM_TEXTURE_REMAP_OUT.CELL_GCM_TEXTURE_REMAP_REMAP;
    public CELL_GCM_TEXTURE_REMAP_OUT OutG { get; set; } = CELL_GCM_TEXTURE_REMAP_OUT.CELL_GCM_TEXTURE_REMAP_REMAP;
    public CELL_GCM_TEXTURE_REMAP_OUT OutR { get; set; } = CELL_GCM_TEXTURE_REMAP_OUT.CELL_GCM_TEXTURE_REMAP_REMAP;
    public CELL_GCM_TEXTURE_REMAP_OUT OutA { get; set; } = CELL_GCM_TEXTURE_REMAP_OUT.CELL_GCM_TEXTURE_REMAP_REMAP;
    public CELL_GCM_TEXTURE_REMAP_FROM InB { get; set; } = CELL_GCM_TEXTURE_REMAP_FROM.CELL_GCM_TEXTURE_REMAP_FROM_B;
    public CELL_GCM_TEXTURE_REMAP_FROM InG { get; set; } = CELL_GCM_TEXTURE_REMAP_FROM.CELL_GCM_TEXTURE_REMAP_FROM_G;
    public CELL_GCM_TEXTURE_REMAP_FROM InR { get; set; } = CELL_GCM_TEXTURE_REMAP_FROM.CELL_GCM_TEXTURE_REMAP_FROM_R;
    public CELL_GCM_TEXTURE_REMAP_FROM InA { get; set; } = CELL_GCM_TEXTURE_REMAP_FROM.CELL_GCM_TEXTURE_REMAP_FROM_A;

    /// <summary>
    /// 1 bit per color, to hold CELL_GCM_BOOL on whether they are handled as complement of 2
    /// </summary>
    public byte SignedRGBA { get; set; }

    public CELL_GCM_TEXTURE_MAG Mag { get; set; } = CELL_GCM_TEXTURE_MAG.CELL_GCM_TEXTURE_LINEAR_MAG;
    public CELL_GCM_TEXTURE_MIN Min { get; set; } = CELL_GCM_TEXTURE_MIN.CELL_GCM_TEXTURE_LINEAR;
    public CELL_GCM_TEXTURE_CONVOLUTION Convultion { get; set; } = CELL_GCM_TEXTURE_CONVOLUTION.CELL_GCM_TEXTURE_CONVOLUTION_QUINCUNX;
    public byte LODBias { get; set; }
    public int BorderColor { get; set; } = 0;

    public ushort Width { get; set; }
    public ushort Height { get; set; }

    public short Depth { get; set; } = 1;
    public int Pitch { get; set; }

    public uint ImageId { get; set; }
    public string SourceFileName { get; set; }

    public PGLUCellTextureInfo()
    {
        BufferInfo = new CellTextureBuffer();
    }

    public override void Write(BinaryStream bs)
    {
        bs.WriteInt32(6656); // head0
        bs.WriteInt32(0); // offset (runtime)

        // CELL_GCM_METHOD_DATA_TEXTURE_BORDER_FORMAT
        int bits = 0;
        bits |= (MipmapLevelLast & 0b_11111111) << 16;
        bits |= ((byte)FormatBits & 0b_11111111) << 8;
        bits |= (byte)(((byte)Dimension & 0b_1111) << 4);
        bits |= (byte)(((byte)Border & 1) << 3);
        bits |= (byte)(((byte)CubeMap & 1) << 2);
        bits |= (byte)((byte)Location + 1 & 0b_11);
        bs.WriteInt32(bits);

        // CELL_GCM_METHOD_DATA_TEXTURE_ADDRESS
        bits = 0;
        bits |= ((byte)ZFunc & 0b_1111) << 28;
        bits |= (Gamma & 0b_1111_1111) << 20;
        bits |= ((byte)WrapR & 0b_1111) << 16;
        bits |= (byte)UnsignedRemap << 0b1111 << 12;
        bits |= ((byte)WrapT & 0b_1111) << 8;
        bits |= (AnisoBias & 0b_1111) << 4;
        bits |= (byte)WrapS & 0b_1111;
        bs.WriteInt32(bits);

        // CELL_GCM_METHOD_DATA_TEXTURE_CONTROL0_ALPHA_KILL
        bits = 0;
        bits |= ((byte)VertexTextureSamplerEnable & 1) << 31;
        bits |= (LODMin & 0b_1111_11111111) << 19;
        bits |= (LODMax & 0b_1111_11111111) << 7;
        bits |= (byte)MaxAniso << 4 & 0b_111;
        bits |= (byte)AlphaKill << 2 & 1;
        bs.WriteInt32(bits);

        // CELL_GCM_METHOD_DATA_TEXTURE_CONTROL1
        bits = 0;
        bits |= ((byte)RemapOrder & 1) << 16;
        bits |= ((byte)OutB & 0b_11) << 14;
        bits |= ((byte)OutG & 0b_11) << 12;
        bits |= ((byte)OutR & 0b_11) << 10;
        bits |= ((byte)OutA & 0b_11) << 8;
        bits |= ((byte)InB & 0b_11) << 6;
        bits |= ((byte)InG & 0b_11) << 4;
        bits |= ((byte)InR & 0b_11) << 2;
        bits |= (byte)InA & 0b_11;
        bs.WriteInt32(bits);

        // CELL_GCM_METHOD_DATA_TEXTURE_FILTER_SIGNED
        bits = 0;
        bits |= (SignedRGBA & 0b1_1111) << 27;
        bits |= ((byte)Mag & 0b_111) << 24;
        bits |= ((byte)Min & 0b_1111_1111) << 16;
        bits |= ((byte)Convultion & 0b_111) << 13;
        bits |= LODBias & 0b_1111_1111_1111;
        bs.WriteInt32(bits);

        // CELL_GCM_METHOD_DATA_TEXTURE_IMAGE_RECT
        bits = 0;
        bits |= (Width & 0b11111111_11111111) << 16;
        bits |= Height & 0b11111111_11111111;
        bs.WriteInt32(bits);

        // CELL_GCM_METHOD_DATA_TEXTURE_BORDER_COLOR
        bs.WriteInt32(BorderColor);

        // CELL_GCM_METHOD_DATA_TEXTURE_CONTROL2
        bs.WriteInt32(6208); // TODO

        // CELL_GCM_METHOD_DATA_TEXTURE_CONTROL3
        int bits5 = 0;
        bits5 |= (int)((Depth & 0x1111_1111_1111) << 20);
        bits5 |= Pitch & 0b1111_1111_1111_1111_1111;
        bs.WriteInt32(bits5); // head1 fixme

        bs.WriteInt32(0); // Reserved.. or not?
        bs.WriteInt32(0); // Same
        bs.WriteInt32(0);
        bs.WriteUInt32(ImageId); // Image Id
        bs.WriteInt32(0);
        bs.WriteInt32(0); // Img name offset to write later if exists
    }

    public override void Read(BinaryStream bs, long basePos, long relocPtr = 0)
    {
        Head0 = bs.ReadUInt32();
        Offset = bs.ReadUInt32();

        uint bits = bs.ReadUInt32();
        MipmapLevelLast = (byte)(bits >> 16 & 0b11111111);
        FormatBits = (CELL_GCM_TEXTURE_FORMAT)(bits >> 8 & 0b11111111);
        Dimension = (CELL_GCM_TEXTURE_DIMENSION)(bits >> 4 & 0b1111);
        Border = (CELL_GCM_TEXTURE_BORDER)(bits >> 3 & 1);
        CubeMap = (CELL_GCM_BOOL)(bits >> 2 & 1);
        Location = (CELL_GCM_LOCATION)((bits & 0b11) - 1);

        bits = bs.ReadUInt32();
        ZFunc = (CELL_GCM_TEXTURE_ZFUNC)(bits >> 28 & 0b11111);
        Gamma = (byte)(bits >> 20 & 0b1111_1111);
        WrapR = (CELL_GCM_TEXTURE_WRAP)(bits >> 16 & 0b1111);
        UnsignedRemap = (CELL_GCM_TEXTURE_UNSIGNED_REMAP)(bits >> 12 & 0b1111);
        WrapT = (CELL_GCM_TEXTURE_WRAP)(bits >> 8 & 0b1111);
        AnisoBias = (byte)(bits >> 4 & 0b1111);
        WrapS = (CELL_GCM_TEXTURE_WRAP)(bits & 0b1111);

        bits = bs.ReadUInt32();
        VertexTextureSamplerEnable = (CELL_GCM_BOOL)(bits >> 31 & 1);
        LODMin = (short)(bits >> 19 & 0b_1111_1111_1111);
        LODMax = (short)(bits >> 7 & 0b_1111_1111_1111);
        MaxAniso = (CELL_GCM_TEXTURE_MAX_ANISO)(bits >> 4 & 0b_111);
        AlphaKill = (CELL_GCM_BOOL)(bits >> 2 & 1); // Some padding before and after

        bits = bs.ReadUInt32();
        RemapOrder = (CELL_GCM_TEXTURE_REMAP_ORDER)(bits >> 16 & 1);
        OutB = (CELL_GCM_TEXTURE_REMAP_OUT)(bits >> 14 & 0b11);
        OutG = (CELL_GCM_TEXTURE_REMAP_OUT)(bits >> 12 & 0b11);
        OutR = (CELL_GCM_TEXTURE_REMAP_OUT)(bits >> 10 & 0b11);
        OutA = (CELL_GCM_TEXTURE_REMAP_OUT)(bits >> 8 & 0b11);
        InB = (CELL_GCM_TEXTURE_REMAP_FROM)(bits >> 6 & 0b11);
        InG = (CELL_GCM_TEXTURE_REMAP_FROM)(bits >> 4 & 0b11);
        InR = (CELL_GCM_TEXTURE_REMAP_FROM)(bits >> 2 & 0b11);
        InA = (CELL_GCM_TEXTURE_REMAP_FROM)(bits & 0b11);

        bits = bs.ReadUInt32();
        SignedRGBA = (byte)(bits >> 27 & 0b1_1111);
        Mag = (CELL_GCM_TEXTURE_MAG)(bits >> 24 & 0b_111);
        Min = (CELL_GCM_TEXTURE_MIN)(bits >> 16 & 0b_11111111);
        Convultion = (CELL_GCM_TEXTURE_CONVOLUTION)(bits >> 13 & 0b_111);
        LODBias = (byte)(bits & 0b_1111_1111_1111);

        bits = bs.ReadUInt32();
        Width = (ushort)(bits >> 16);
        Height = (ushort)(bits & 0b11111111_11111111);

        BorderColor = bs.ReadInt32();
        bs.ReadInt32(); // CELL_GCM_METHOD_DATA_TEXTURE_CONTROL2 TODO

        bits = bs.ReadUInt32();
        Depth = (short)(bits >> 20 & 0x1111_1111_1111);
        Pitch = (int)(bits & 0b1111_11111111_11111111);

        bs.ReadUInt32();
        bs.ReadUInt32();
        bs.ReadUInt32();
        ImageId = bs.ReadUInt32();
        bs.ReadUInt32();
        uint imageNameOffset = bs.ReadUInt32();
        // The set's pointers are absolute against its relocation pointer, so rebase then offset from
        // the set's own start. (0 - 0 for a standalone file; the old "offset - basePos" went negative
        // for any set read in place inside a bigger file.)
        bs.Position = basePos + (imageNameOffset - relocPtr);
        SourceFileName = bs.ReadString(StringCoding.ZeroTerminated);
        Name = SourceFileName;
    }

    /// <summary>
    /// Serialises this texture's pixels into a standard DDS in <paramref name="outStream"/>.
    /// </summary>
    /// <remarks>
    /// ORIENTATION: PD stores PS3 textures bottom-up (bottom-left UV origin) whereas DDS/PNG are
    /// top-down, so decoded images come out vertically flipped compared to how they appear in-game.
    /// That is intentional and left alone: the rows are reproduced byte-for-byte, and because the
    /// build path doesn't flip either, extract -> edit -> rebuild round-trips stay byte-exact. Flip
    /// vertically in an image editor if you want to view/author art the right way up. (PSP 3SXT is
    /// top-down and needs no such flip.)
    /// </remarks>
    internal void CreateDDSData(byte[] imageData, Stream outStream)
    {
        var header = new DdsHeader();
        header.Height = Height;
        header.Width = Width;

        // https://gist.github.com/Scobalula/d9474f3fcf3d5a2ca596fceb64e16c98#file-directxtexutil-cs-L355

        CELL_GCM_TEXTURE_FORMAT format = FormatBits & ~CELL_GCM_TEXTURE_FORMAT.CELL_GCM_TEXTURE_LN;

        switch (format)   // dwPitchOrLinearSize
        {
            case CELL_GCM_TEXTURE_FORMAT.CELL_GCM_TEXTURE_COMPRESSED_DXT1:
                header.PitchOrLinearSize = Height * Width / 2;
                break;
            case CELL_GCM_TEXTURE_FORMAT.CELL_GCM_TEXTURE_COMPRESSED_DXT23:
            case CELL_GCM_TEXTURE_FORMAT.CELL_GCM_TEXTURE_COMPRESSED_DXT45:
                header.PitchOrLinearSize = Height * Width;
                break;
            default:
                // 32bpp
                header.PitchOrLinearSize = (Width * 32 + 7) / 8;
                break;
        }


        header.LastMipmapLevel = MipmapLevelLast;

        switch (format)
        {
            case CELL_GCM_TEXTURE_FORMAT.CELL_GCM_TEXTURE_COMPRESSED_DXT1:
            case CELL_GCM_TEXTURE_FORMAT.CELL_GCM_TEXTURE_COMPRESSED_DXT23:
            case CELL_GCM_TEXTURE_FORMAT.CELL_GCM_TEXTURE_COMPRESSED_DXT45:
                header.FormatFlags = DDSPixelFormatFlags.DDPF_FOURCC;

                if (format == CELL_GCM_TEXTURE_FORMAT.CELL_GCM_TEXTURE_COMPRESSED_DXT1)
                    header.FourCCName = "DXT1";
                else if (format == CELL_GCM_TEXTURE_FORMAT.CELL_GCM_TEXTURE_COMPRESSED_DXT23)
                    header.FourCCName = "DXT3";
                else if (format == CELL_GCM_TEXTURE_FORMAT.CELL_GCM_TEXTURE_COMPRESSED_DXT45)
                    header.FourCCName = "DXT5";
                break;
            case CELL_GCM_TEXTURE_FORMAT.CELL_GCM_TEXTURE_A8R8G8B8:
            case CELL_GCM_TEXTURE_FORMAT.CELL_GCM_TEXTURE_D8R8G8B8:
                header.FormatFlags = DDSPixelFormatFlags.DDPF_RGB | DDSPixelFormatFlags.DDPF_ALPHAPIXELS | DDSPixelFormatFlags.DDPF_FOURCC;
                header.FourCCName = "DX10";
                header.RGBBitCount = 32;

                header.RBitMask = 0x000000FF;  // R BitMask 
                header.GBitMask = 0x0000FF00;  // G BitMask
                header.BBitMask = 0x00FF0000;  // B BitMask
                header.ABitMask = 0xFF000000;  // A BitMask

                header.DxgiFormat = DDS_DXGI_FORMAT.DXGI_FORMAT_R8G8B8A8_UNORM;
                break;
        }

        bool is32bpp = format == CELL_GCM_TEXTURE_FORMAT.CELL_GCM_TEXTURE_A8R8G8B8
                    || format == CELL_GCM_TEXTURE_FORMAT.CELL_GCM_TEXTURE_D8R8G8B8;

        // Unswizzle
        if (is32bpp && !FormatBits.HasFlag(CELL_GCM_TEXTURE_FORMAT.CELL_GCM_TEXTURE_LN))
        {
            // Swizzled (SZ): de-swizzle via Morton order (dimensions are power-of-two).
            int byteCount = Width * Height * 4;
            byte[] newImageData = new byte[byteCount];

            Syroot.BinaryData.Memory.SpanReader sr = new Syroot.BinaryData.Memory.SpanReader(imageData);
            Syroot.BinaryData.Memory.SpanWriter sw = new Syroot.BinaryData.Memory.SpanWriter(newImageData);

            Span<byte> pixBuffer;
            for (int i = 0; i < Width * Height; i++)
            {
                int pixIndex = Swizzler.MortonReorder(i, Width, Height);
                pixBuffer = sr.ReadBytes(4);
                int destIndex = 4 * pixIndex;
                sw.Position = destIndex;
                sw.WriteBytes(pixBuffer);
            }

            imageData = newImageData;
        }
        else if (is32bpp)
        {
            // Linear (LN): rows are padded to the row pitch (Pitch, in bytes; typically
            // nextPow2(Width) * 4). DDS/Pfim expect tightly-packed rows, so strip the
            // per-row padding here. Without this, any texture whose width isn't a power of
            // two decodes sheared into diagonal stripes.
            int rowBytes = Width * 4;
            int stride = Pitch >= rowBytes ? Pitch : rowBytes;
            if (stride != rowBytes && imageData.Length >= stride * Height)
            {
                byte[] tight = new byte[rowBytes * Height];
                for (int y = 0; y < Height; y++)
                    Array.Copy(imageData, y * stride, tight, y * rowBytes, rowBytes);

                imageData = tight;
            }
        }

        // Remap channels into the DDS R8G8B8A8 byte order.
        // Source is big-endian A8R8G8B8, i.e. bytes [A,R,G,B]; the GCM remap (InR/InG/InB/InA)
        // selects, per output channel, the source component using A=0,R=1,G=2,B=3 — which is
        // exactly the source byte index. (The previous code byte-reversed each pixel first,
        // which scrambled the standard identity remap into R<->G / B<->A channel swaps.)
        if (is32bpp)
        {
            for (var i = 0; i < Width * Height * 4; i += 4)
            {
                byte r = imageData[i + (byte)InR];
                byte g = imageData[i + (byte)InG];
                byte b = imageData[i + (byte)InB];
                byte a = imageData[i + (byte)InA];

                imageData[i + 0] = r;
                imageData[i + 1] = g;
                imageData[i + 2] = b;
                imageData[i + 3] = a;
            }
        }

        header.ImageData = imageData;
        header.Write(outStream);
    }

    public void InitFromDDSImage(IImage image, CELL_GCM_TEXTURE_FORMAT format)
    {
        // The built data is linear (not swizzled), so the LN flag must be set or the decoder
        // (which keys off this register) will Morton-deswizzle it back into garbage.
        FormatBits = format | CELL_GCM_TEXTURE_FORMAT.CELL_GCM_TEXTURE_LN;
        Width = (ushort)image.Width;
        Height = (ushort)image.Height;

        // RSX linear (LN) row pitch = bytes per row that PD writes, and it's format-specific:
        //   32bpp (A8R8G8B8/D8R8G8B8): nextPow2(Width) * 4  (e.g. 46px -> 64px -> 256; a tight 184
        //     isn't 64-aligned and the GPU rejects it). FromStandardImage row-pads data to match.
        //   DXT1 : blocksPerRow * 8   (8 bytes per 4x4 block)
        //   DXT23/DXT45 : blocksPerRow * 16  (16 bytes per block)
        // The old code wrote Width*4 for ALL DXT, which equals blocksPerRow*16 for block-aligned
        // widths (so DXT23/45 were right) but is DOUBLE the DXT1 pitch -> the RSX over-strides each
        // block row and runs off the data half-way down -> garbled bottom half in-game.
        bool is32bpp = format == CELL_GCM_TEXTURE_FORMAT.CELL_GCM_TEXTURE_A8R8G8B8
                    || format == CELL_GCM_TEXTURE_FORMAT.CELL_GCM_TEXTURE_D8R8G8B8;
        int blocksPerRow = (Width + 3) / 4;
        Pitch = format switch
        {
            CELL_GCM_TEXTURE_FORMAT.CELL_GCM_TEXTURE_COMPRESSED_DXT1 => blocksPerRow * 8,
            CELL_GCM_TEXTURE_FORMAT.CELL_GCM_TEXTURE_COMPRESSED_DXT23 => blocksPerRow * 16,
            CELL_GCM_TEXTURE_FORMAT.CELL_GCM_TEXTURE_COMPRESSED_DXT45 => blocksPerRow * 16,
            _ => is32bpp ? NextPowerOfTwo(Width) * 4 : Width * 4,
        };

        // PD's GPB textures live in main (XDR) memory, not RSX local video memory. Writing the
        // default LOCAL makes the GPU sample texels from the wrong memory pool -> in-game crash.
        Location = CELL_GCM_LOCATION.CELL_GCM_LOCATION_MAIN;

        // MipmapLevelLast is a 1-based level COUNT (PD writes 1 for a single-mip texture); Pfim's
        // MipMaps does not include the base level, so add 1. We always build a single mip (-m 1).
        MipmapLevelLast = (byte)(image.MipMaps.Length + 1);

        var cellBufferInfo = BufferInfo as CellTextureBuffer;
        cellBufferInfo.Width = (ushort)image.Width;
        cellBufferInfo.Height = (ushort)image.Height;
        cellBufferInfo.LastMipmapLevel = MipmapLevelLast;
        cellBufferInfo.FormatBits = format | CELL_GCM_TEXTURE_FORMAT.CELL_GCM_TEXTURE_LN;
    }

    public bool FromStandardImage(string path, CELL_GCM_TEXTURE_FORMAT format)
    {
        IImageFormat i = Image.DetectFormat(path);
        if (i is null)
        {
            Console.WriteLine($"This file is not a regular image file. {path}");
            return false;
        }

        ConvertFileToDDS(path, format);

        string ddsFileName = Path.ChangeExtension(path, ".dds");
        if (!File.Exists(ddsFileName))
            return false;

        var dds = Pfimage.FromFile(ddsFileName);
        InitFromDDSImage(dds, format);

        Memory<byte> ddsData = File.ReadAllBytes(ddsFileName).AsMemory(0x80);

        // Convert B8G8R8A8_UNORM (from conversion to dds) to A8R8G8B8 since TexConv does not support it
        if (format == CELL_GCM_TEXTURE_FORMAT.CELL_GCM_TEXTURE_A8R8G8B8)
        {
            var pixels = MemoryMarshal.Cast<byte, uint>(ddsData.Span);
            for (int j = 0; j < Width * Height; j++)
                pixels[j] = BinaryPrimitives.ReverseEndianness(pixels[j]);
        }

        // Row-pad the tightly-packed pixels up to the RSX pitch (nextPow2(Width)*4) so the
        // stored layout matches PD/hardware. No-op when Pitch == Width*4 (pow2 widths, DXT).
        BufferInfo.ImageData = PadRowsToPitch(ddsData, Width, Height, Pitch);
        SourceFileName = Path.GetFileNameWithoutExtension(path);

        File.Delete(ddsFileName);
        return true;
    }

    /// <summary>The pixel format a .dds file actually stores (probed from its header).</summary>
    private enum DdsSourceFormat { Unknown, Uncompressed, Dxt1, Dxt3, Dxt5 }

    /// <summary>
    /// Probes a DDS file's header for the pixel format it ACTUALLY stores and where its pixel data
    /// begins. Handles legacy FourCC DXTn, DX10-extended (DXGI) BC/uncompressed, and raw RGB(A).
    /// </summary>
    private static (DdsSourceFormat fmt, int dataOffset) ProbeDds(byte[] file)
    {
        const uint DDPF_FOURCC = 0x4;
        uint pfFlags = BinaryPrimitives.ReadUInt32LittleEndian(file.AsSpan(0x50));
        string fourCC = Encoding.ASCII.GetString(file, 0x54, 4);

        if ((pfFlags & DDPF_FOURCC) != 0)
        {
            if (fourCC == "DX10")
            {
                // DX10 extended header (20 bytes) follows the 0x80 base header; dxgiFormat is at 0x80.
                uint dxgi = BinaryPrimitives.ReadUInt32LittleEndian(file.AsSpan(0x80));
                DdsSourceFormat f = dxgi switch
                {
                    70 or 71 or 72 => DdsSourceFormat.Dxt1, // BC1 (typeless/unorm/unorm_srgb)
                    73 or 74 or 75 => DdsSourceFormat.Dxt3, // BC2
                    76 or 77 or 78 => DdsSourceFormat.Dxt5, // BC3
                    _ => DdsSourceFormat.Uncompressed,      // e.g. 28 R8G8B8A8, 87 B8G8R8A8
                };
                return (f, 0x94);
            }

            DdsSourceFormat ff = fourCC switch
            {
                "DXT1" => DdsSourceFormat.Dxt1,
                "DXT2" or "DXT3" => DdsSourceFormat.Dxt3,
                "DXT4" or "DXT5" => DdsSourceFormat.Dxt5,
                _ => DdsSourceFormat.Unknown,
            };
            return (ff, 0x80);
        }

        // No FourCC -> uncompressed RGB(A) described by the legacy bit masks.
        return (DdsSourceFormat.Uncompressed, 0x80);
    }

    /// <summary>
    /// Decodes a Pfim-loaded DDS into tightly-packed canonical RGBA bytes ([R,G,B,A] per pixel).
    /// Pfim normalises any source channel order/format (DXT, A8R8G8B8, B8G8R8A8, DX10, ...) into its
    /// BGRA buffer, so this works regardless of how the .dds was authored.
    /// </summary>
    private static byte[] DecodeToRgba(IImage dds)
    {
        int w = dds.Width, h = dds.Height, stride = dds.Stride;
        byte[] src = dds.Data;
        byte[] outp = new byte[w * h * 4];
        int di = 0;
        switch (dds.Format)
        {
            case ImageFormat.Rgba32: // Pfim Rgba32 == BGRA byte order
                for (int y = 0; y < h; y++)
                {
                    int row = y * stride;
                    for (int x = 0; x < w; x++)
                    {
                        int s = row + x * 4;
                        outp[di++] = src[s + 2]; // R
                        outp[di++] = src[s + 1]; // G
                        outp[di++] = src[s + 0]; // B
                        outp[di++] = src[s + 3]; // A
                    }
                }
                break;
            case ImageFormat.Rgb24: // BGR byte order, opaque
                for (int y = 0; y < h; y++)
                {
                    int row = y * stride;
                    for (int x = 0; x < w; x++)
                    {
                        int s = row + x * 3;
                        outp[di++] = src[s + 2]; // R
                        outp[di++] = src[s + 1]; // G
                        outp[di++] = src[s + 0]; // B
                        outp[di++] = 255;        // A
                    }
                }
                break;
            default:
                throw new NotSupportedException($"Unsupported DDS pixel layout for decode: {dds.Format}");
        }
        return outp;
    }

    /// <summary>
    /// Builds a Cell texture from a .dds file. The filename flag (DXT1/DXT23/DXT45/A8R8G8B8/...) is the
    /// authority on the OUTPUT format; this probes what the .dds actually contains and, if it already
    /// matches a DXT target, copies the blocks verbatim (lossless, the inverse of <see cref="GetDDS"/>).
    /// Otherwise it decodes the image and re-encodes/repacks into the requested format: 32bpp targets
    /// repack channels into PD's big-endian A8R8G8B8, mismatched DXT targets are BC-encoded.
    /// No texconv involved.
    /// </summary>
    public bool FromDDS(string path, CELL_GCM_TEXTURE_FORMAT format)
    {
        byte[] file = File.ReadAllBytes(path);
        if (file.Length < 0x80 || file[0] != 'D' || file[1] != 'D' || file[2] != 'S' || file[3] != ' ')
        {
            Console.WriteLine($"Not a valid DDS file: {path}");
            return false;
        }

        var (srcFmt, dataOffset) = ProbeDds(file);

        var dds = Pfimage.FromFile(path);
        InitFromDDSImage(dds, format);

        // We only ever store the base level, so present the texture as single-mip regardless of any
        // mips the source .dds carried (avoids claiming mip data we didn't write).
        MipmapLevelLast = 1;
        if (BufferInfo is CellTextureBuffer cbuf)
            cbuf.LastMipmapLevel = 1;

        bool targetIsDxt = format is CELL_GCM_TEXTURE_FORMAT.CELL_GCM_TEXTURE_COMPRESSED_DXT1
                                  or CELL_GCM_TEXTURE_FORMAT.CELL_GCM_TEXTURE_COMPRESSED_DXT23
                                  or CELL_GCM_TEXTURE_FORMAT.CELL_GCM_TEXTURE_COMPRESSED_DXT45;

        if (targetIsDxt)
        {
            DdsSourceFormat targetAsSrc = format switch
            {
                CELL_GCM_TEXTURE_FORMAT.CELL_GCM_TEXTURE_COMPRESSED_DXT1 => DdsSourceFormat.Dxt1,
                CELL_GCM_TEXTURE_FORMAT.CELL_GCM_TEXTURE_COMPRESSED_DXT23 => DdsSourceFormat.Dxt3,
                _ => DdsSourceFormat.Dxt5,
            };

            int blocksPerRow = (Width + 3) / 4;
            int blocksPerCol = (Height + 3) / 4;
            int blockBytes = blocksPerRow * blocksPerCol * (targetAsSrc == DdsSourceFormat.Dxt1 ? 8 : 16);

            if (srcFmt == targetAsSrc && dataOffset + blockBytes <= file.Length)
            {
                // Lossless: the .dds already holds exactly this DXT format. Standard DDS block order
                // == PD PS3 order, so copy the base-level blocks verbatim (no BC re-encode).
                BufferInfo.ImageData = file.AsMemory(dataOffset, blockBytes);
            }
            else
            {
                // Source is uncompressed or a different BC format -> decode and BC-encode to target.
                byte[] rgba = DecodeToRgba(dds);
                var encoder = new BCnEncoder.Encoder.BcEncoder();
                encoder.OutputOptions.Format = targetAsSrc switch
                {
                    DdsSourceFormat.Dxt1 => BCnEncoder.Shared.CompressionFormat.Bc1,
                    DdsSourceFormat.Dxt3 => BCnEncoder.Shared.CompressionFormat.Bc2,
                    _ => BCnEncoder.Shared.CompressionFormat.Bc3,
                };
                encoder.OutputOptions.Quality = BCnEncoder.Encoder.CompressionQuality.BestQuality;
                encoder.OutputOptions.GenerateMipMaps = false;
                BufferInfo.ImageData = encoder.EncodeToRawBytes(rgba, Width, Height, BCnEncoder.Encoder.PixelFormat.Rgba32)[0];
            }
        }
        else
        {
            // 32bpp target (A8R8G8B8 / D8R8G8B8): decode to canonical RGBA, then store PD's big-endian
            // [A,R,G,B] byte order, row-padded to the RSX pitch. Lossless for any 32bpp source.
            byte[] rgba = DecodeToRgba(dds);
            byte[] tight = new byte[Width * Height * 4];
            for (int i = 0; i < Width * Height; i++)
            {
                byte r = rgba[i * 4 + 0], g = rgba[i * 4 + 1], b = rgba[i * 4 + 2], a = rgba[i * 4 + 3];
                tight[i * 4 + 0] = a;
                tight[i * 4 + 1] = r;
                tight[i * 4 + 2] = g;
                tight[i * 4 + 3] = b;
            }
            BufferInfo.ImageData = PadRowsToPitch(tight, Width, Height, Pitch);
        }

        SourceFileName = Path.GetFileNameWithoutExtension(path);
        return true;
    }

    private static int NextPowerOfTwo(int value)
    {
        int p = 1;
        while (p < value)
            p <<= 1;
        return p;
    }

    /// <summary>
    /// Pads each 4-bytes-per-pixel row of <paramref name="tight"/> (width*4 bytes) out to
    /// <paramref name="pitch"/> bytes. Returns the input unchanged when no padding is needed.
    /// </summary>
    private static Memory<byte> PadRowsToPitch(Memory<byte> tight, int width, int height, int pitch)
    {
        int rowBytes = width * 4;
        if (pitch <= rowBytes)
            return tight;

        byte[] padded = new byte[pitch * height];
        Span<byte> src = tight.Span;
        for (int y = 0; y < height; y++)
            src.Slice(y * rowBytes, rowBytes).CopyTo(padded.AsSpan(y * pitch));
        return padded;
    }

    /// <summary>
    /// Serialises this texture to a standard .dds (DXT FourCC for DXT1/23/45, a DX10 R8G8B8A8 header
    /// for 32bpp) - the editable form used by the lossless DDS round-trip. Inverse of <see cref="FromDDS"/>.
    /// </summary>
    public byte[] GetDDS()
    {
        using var ms = new MemoryStream();
        CreateDDSData(BufferInfo.ImageData.ToArray(), ms);
        ms.Position = 0;

        return ms.ToArray();
    }

    public override string GetPixelFormatName()
    {
        CELL_GCM_TEXTURE_FORMAT format = FormatBits & ~CELL_GCM_TEXTURE_FORMAT.CELL_GCM_TEXTURE_LN;
        return format switch
        {
            CELL_GCM_TEXTURE_FORMAT.CELL_GCM_TEXTURE_A8R8G8B8 => "A8R8G8B8",
            CELL_GCM_TEXTURE_FORMAT.CELL_GCM_TEXTURE_D8R8G8B8 => "D8R8G8B8",
            CELL_GCM_TEXTURE_FORMAT.CELL_GCM_TEXTURE_COMPRESSED_DXT1 => "DXT1",
            CELL_GCM_TEXTURE_FORMAT.CELL_GCM_TEXTURE_COMPRESSED_DXT23 => "DXT23",
            CELL_GCM_TEXTURE_FORMAT.CELL_GCM_TEXTURE_COMPRESSED_DXT45 => "DXT45",
            _ => format.ToString(),
        };
    }

    public override Image GetAsImage()
    {
        // TODO: don't make a dds first. decode straight away.
        using var ms = new MemoryStream();
        CreateDDSData(BufferInfo.ImageData.ToArray(), ms);
        ms.Position = 0;

        var dds = Pfimage.FromStream(ms);
        if (dds.Format == ImageFormat.Rgb24)
        {
            var i = Image.LoadPixelData<Bgr24>(dds.Data, dds.Width, dds.Height);
            return i;
        }
        else if (dds.Format == ImageFormat.Rgba32)
        {
            // Image data is now tightly packed (rows de-padded in CreateDDSData), so the
            // image width matches the data exactly. The previous code rounded the width up
            // to a multiple of 4, which made LoadPixelData over-read and throw for any
            // texture whose width wasn't a multiple of 4 (producing no output at all).
            var i = Image.LoadPixelData<Bgra32>(dds.Data, dds.Width, dds.Height);
            return i;
        }
        else
        {
            Console.WriteLine($"Invalid format to save..? {dds.Format}");
            return null;
        }

    }

    private static void ConvertFileToDDS(string fileName, CELL_GCM_TEXTURE_FORMAT imgFormat)
    {
        // Strip the PNG's colour-profile chunks (sRGB/gAMA/iCCP/cHRM) before handing it to texconv.
        // texconv/WIC otherwise honor an embedded profile and gamma-convert the pixels - WITHOUT a
        // profile it treats the data as-is (raw passthrough) for every output format. This replaces
        // the old "-srgbo" flag, which was passthrough for A8R8G8B8 but BRIGHTENED DXT (linear->sRGB)
        // so unmodified DXT textures came back too bright. Stripping (no srgb flag) is byte-exact
        // passthrough for A8R8G8B8/D8R8G8B8 AND DXT, for both chunkless dumps and editor-saved PNGs.
        string tempDir = Path.Combine(Path.GetTempPath(), "txs3conv_" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(tempDir);
        string strippedPng = Path.Combine(tempDir, Path.GetFileName(fileName)); // same base name -> <dir>/<base>.dds
        StripPngColorChunks(fileName, strippedPng);

        string arguments = $"\"{strippedPng}\"";
        if (imgFormat == CELL_GCM_TEXTURE_FORMAT.CELL_GCM_TEXTURE_COMPRESSED_DXT1)
            arguments += " -f DXT1";
        else if (imgFormat == CELL_GCM_TEXTURE_FORMAT.CELL_GCM_TEXTURE_COMPRESSED_DXT23)
            arguments += " -f DXT3";
        else if (imgFormat == CELL_GCM_TEXTURE_FORMAT.CELL_GCM_TEXTURE_COMPRESSED_DXT45)
            arguments += " -f DXT5";
        else if (imgFormat == CELL_GCM_TEXTURE_FORMAT.CELL_GCM_TEXTURE_D8R8G8B8)
            arguments += " -f B8G8R8X8_UNORM";
        else if (imgFormat == CELL_GCM_TEXTURE_FORMAT.CELL_GCM_TEXTURE_A8R8G8B8)
            arguments += " -f B8G8R8A8_UNORM"; // We'll reverse it later, TexConv does not support A8R8G8B8

        arguments += " -y"      // Overwrite if it exists
                  + " -m 1"     // Don't care about extra mipmaps
                  + " -nologo"  // No copyright logo
                  + $" -o {Path.GetDirectoryName(fileName)}"; // Set directory to file input's directory

        // texconv.exe is deployed next to the converter executable. Resolve it from the
        // app's base directory (not the caller's CWD) so the build works no matter where
        // the process was launched from
        Process converter = Process.Start(Path.Combine(AppContext.BaseDirectory, "texconv.exe"), arguments);
        converter.WaitForExit();

        try { Directory.Delete(tempDir, true); } catch { /* best-effort temp cleanup */ }
    }

    /// <summary>
    /// Copies <paramref name="src"/> PNG to <paramref name="dst"/>, dropping the colour-profile
    /// ancillary chunks (sRGB/gAMA/iCCP/cHRM) so texconv doesn't gamma-convert by them. Pixels are
    /// untouched. Falls back to a plain copy if the file isn't a PNG.
    /// </summary>
    private static void StripPngColorChunks(string src, string dst)
    {
        byte[] d = File.ReadAllBytes(src);
        ReadOnlySpan<byte> sig = [0x89, (byte)'P', (byte)'N', (byte)'G', 0x0D, 0x0A, 0x1A, 0x0A];
        if (d.Length < 8 || !d.AsSpan(0, 8).SequenceEqual(sig))
        {
            File.Copy(src, dst, overwrite: true);
            return;
        }

        using var outFs = new FileStream(dst, FileMode.Create);
        outFs.Write(d, 0, 8);
        int p = 8;
        while (p + 8 <= d.Length)
        {
            int len = (d[p] << 24) | (d[p + 1] << 16) | (d[p + 2] << 8) | d[p + 3];
            int total = 12 + len;
            if (total < 12 || p + total > d.Length)
                break;

            bool isColourChunk = (d[p + 4] == 's' && d[p + 5] == 'R' && d[p + 6] == 'G' && d[p + 7] == 'B')
                              || (d[p + 4] == 'g' && d[p + 5] == 'A' && d[p + 6] == 'M' && d[p + 7] == 'A')
                              || (d[p + 4] == 'i' && d[p + 5] == 'C' && d[p + 6] == 'C' && d[p + 7] == 'P')
                              || (d[p + 4] == 'c' && d[p + 5] == 'H' && d[p + 6] == 'R' && d[p + 7] == 'M');
            if (!isColourChunk)
                outFs.Write(d, p, total);

            bool isEnd = d[p + 4] == 'I' && d[p + 5] == 'E' && d[p + 6] == 'N' && d[p + 7] == 'D';
            p += total;
            if (isEnd)
                break;
        }
    }

}
