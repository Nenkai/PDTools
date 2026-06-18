using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Reflection.Emit;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

using BCnEncoder.Decoder;
using BCnEncoder.Encoder;
using BCnEncoder.Shared;

using PDTools.Files.Textures.PS2;
using PDTools.Utils;

using SixLabors.Fonts;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.Processing.Processors.Quantization;

using Syroot.BinaryData;

namespace PDTools.Files.Textures.PSP;

/// <summary>
/// PDI GL GE (PSP) Texture Info. Thin wrapper over PSP texture registers
/// </summary>
public class PGLUGETextureInfo : PGLUTextureInfo
{
    public float UMIN { get; set; }
    public float VMIN { get; set; }
    public float UMAX { get; set; }
    public float VMAX { get; set; }

    public SCE_GE_TMAP TMAP { get; set; } = new();
    public SCE_GE_TSHADE TSHADE { get; set; } = new();
    public SCE_GE_TMODE TMODE { get; set; } = new();
    public SCE_GE_TPF TPF { get; set; } = new();
    public SCE_GE_CLOAD CLOAD { get; set; } = new();
    public SCE_GE_CLUT CLUT { get; set; } = new();
    public SCE_GE_TFILTER FILTER { get; set; } = new();
    public SCE_GE_TWRAP TWRAP { get; set; } = new();
    public SCE_GE_TLEVEL LEVEL { get; set; } = new();
    public SCE_GE_TFUNC TFUNC { get; set; } = new();
    public SCE_GE_TEC TEC { get; set; } = new();
    public GEMipInfo[] MipInfos { get; } = new GEMipInfo[8];
    public GECommandList CommandList { get; } = new();

    public short ClutMapEntryIndex { get; set; }

    public GEClutBufferInfo ClutBufferInfo { get; set; }

    public override void Write(BinaryStream bs)
    {
        throw new NotImplementedException();
    }

    public override void Read(BinaryStream bs, long basePos)
    {
        byte[] buffer = bs.ReadBytes(0x98);
        BitStream bitStream = new BitStream(BitStreamMode.Read, buffer, BitStreamSignificantBitOrder.MSB);

        uint subParamsOffset = bitStream.ReadUInt32();
        bitStream.ReadUInt32();
        UMIN = bitStream.ReadSingle();
        VMIN = bitStream.ReadSingle();
        UMAX = bitStream.ReadSingle();
        VMAX = bitStream.ReadSingle();
        TMAP.Read(ref bitStream);
        TSHADE.Read(ref bitStream);
        TMODE.Read(ref bitStream);
        TPF.Read(ref bitStream);
        CLOAD.Read(ref bitStream);
        CLUT.Read(ref bitStream);
        FILTER.Read(ref bitStream);
        TWRAP.Read(ref bitStream);
        LEVEL.Read(ref bitStream);
        TFUNC.Read(ref bitStream);
        TEC.Read(ref bitStream);
        bitStream.ReadUInt32(); // Runtime clut offset

        for (int i = 0; i < 8; i++)
        {
            bitStream.ReadUInt32(); // Reloc ptr

            MipInfos[i] = new GEMipInfo();
            MipInfos[i].Width = bitStream.ReadUInt16();
            MipInfos[i].Unk1 = (byte)bitStream.ReadBits(5);
            bitStream.ReadBits(3);
            MipInfos[i].Index = (byte)bitStream.ReadBits(8);
        }

        bitStream.ReadInt16();
        ClutMapEntryIndex = bitStream.ReadInt16();
        bitStream.ReadInt16();
        BufferId = bitStream.ReadUInt16();
        bitStream.ReadUInt32();

        uint nameOffset = bitStream.ReadUInt32();
        bs.Position = nameOffset - basePos;
        Name = bs.ReadString(StringCoding.ZeroTerminated);

        bs.Position = subParamsOffset - basePos;
        CommandList.Read(bs, this);
    }

    public override string GetPixelFormatName() => TPF.tpf switch
    {
        eSCE_GE_TPF.SCE_GE_TPF_8888 => "8888",
        eSCE_GE_TPF.SCE_GE_TPF_IDTEX4 => "IDTEX4",
        eSCE_GE_TPF.SCE_GE_TPF_IDTEX8 => "IDTEX8",
        eSCE_GE_TPF.SCE_GE_TPF_DXT1 => "DXT1",
        eSCE_GE_TPF.SCE_GE_TPF_DXT3 => "DXT3",
        eSCE_GE_TPF.SCE_GE_TPF_DXT5 => "DXT5",
        _ => TPF.tpf.ToString(),
    };

    public override Image GetAsImage()
    {
        var mip1 = MipInfos[0];

        // mip1.Width is the texture buffer stride: the real width padded up to a power of
        // two. Pixels are decoded at that stride, then cropped to the real display size.
        int width = mip1.Width;

        // The real (display) dimensions come from the buffer info. The previous code derived
        // the crop width from width * UMIN, but UMIN is a texture-coordinate bound, not the
        // real/padded ratio, so it left padding columns in (e.g. a 16-wide texture in a
        // 32-wide buffer came out 32 wide). Height already uses BufferInfo.Height directly.
        int regionWidth = BufferInfo.Width;
        int regionHeight = BufferInfo.Height; //(int)(height * VMIN);
        Image<Rgba32> img = new Image<Rgba32>(width, BufferInfo.Height);

        switch (TPF.tpf)
        {
            case eSCE_GE_TPF.SCE_GE_TPF_5650:
                throw new NotImplementedException("SCE_GE_TPF_5650 not yet implemented.");
                break;
            case eSCE_GE_TPF.SCE_GE_TPF_5551:
                throw new NotImplementedException("SCE_GE_TPF_5551 not yet implemented.");
                break;
            case eSCE_GE_TPF.SCE_GE_TPF_4444:
                throw new NotImplementedException("SCE_GE_TPF_4444 not yet implemented.");
                break;
            case eSCE_GE_TPF.SCE_GE_TPF_8888:
                {
                    byte[] data = new byte[BufferInfo.ImageData.Span.Length];
                    BufferInfo.ImageData.Span.CopyTo(data);

                    if (TMODE.hsm == SCE_GE_TMODE_HSM.SCE_GE_TMODE_HSM_HIGHSPEED)
                        Swizzle(BufferInfo.ImageData.Span, data, width, BufferInfo.Height, GEUtils.BitsPerPixel(TPF.tpf));

                    // PSP GU_PSM_8888 stores each pixel as little-endian R,G,B,A bytes,
                    // which maps directly onto Rgba32.
                    ReadOnlySpan<Rgba32> pixels = MemoryMarshal.Cast<byte, Rgba32>(data);
                    for (var y = 0; y < BufferInfo.Height; y++)
                    {
                        for (var x = 0; x < width; x++)
                            img[x, y] = pixels[y * width + x];
                    }
                }
                break;
            case eSCE_GE_TPF.SCE_GE_TPF_IDTEX4:
            case eSCE_GE_TPF.SCE_GE_TPF_IDTEX8:
                {
                    int bpp = GEUtils.BitsPerPixel(TPF.tpf);

                    byte[] data = new byte[BufferInfo.ImageData.Span.Length];
                    BufferInfo.ImageData.Span.CopyTo(data);

                    if (TMODE.hsm == SCE_GE_TMODE_HSM.SCE_GE_TMODE_HSM_HIGHSPEED)
                        Swizzle(BufferInfo.ImageData.Span, data, width, BufferInfo.Height, bpp);

                    Span<uint> clut = MemoryMarshal.Cast<byte, uint>(ClutBufferInfo.ClutData);

                    // IDTEX4 packs two 4-bit indices per byte; PSP (GU_PSM_T4) stores the
                    // even pixel in the LOW nibble and the odd pixel in the HIGH nibble.
                    for (var y = 0; y < BufferInfo.Height; y++)
                    {
                        for (var x = 0; x < width; x++)
                        {
                            int pixelIndex = (y * width) + x;
                            int idx = bpp == 8
                                ? data[pixelIndex]
                                : ((pixelIndex & 1) == 0 ? data[pixelIndex >> 1] & 0x0F : data[pixelIndex >> 1] >> 4);
                            img[x, y] = new Rgba32(clut[idx]);
                        }
                    }
                }
                break;
            case eSCE_GE_TPF.SCE_GE_TPF_IDTEX16:
                throw new NotImplementedException("SCE_GE_TPF_IDTEX16 not yet implemented.");
                break;
            case eSCE_GE_TPF.SCE_GE_TPF_IDTEX32:
                throw new NotImplementedException("SCE_GE_TPF_IDTEX32 not yet implemented.");
                break;
            case eSCE_GE_TPF.SCE_GE_TPF_DXT1:
            case eSCE_GE_TPF.SCE_GE_TPF_DXT3:
            case eSCE_GE_TPF.SCE_GE_TPF_DXT5:
                {
                    BcDecoder decoder = new BcDecoder();

                    var bcType = TPF.tpf switch
                    {
                        eSCE_GE_TPF.SCE_GE_TPF_DXT1 => CompressionFormat.Bc1,
                        eSCE_GE_TPF.SCE_GE_TPF_DXT3 => CompressionFormat.Bc2,
                        eSCE_GE_TPF.SCE_GE_TPF_DXT5 => CompressionFormat.Bc3,
                    };

                    byte[] data = new byte[BufferInfo.ImageData.Span.Length];
                    BufferInfo.ImageData.Span.CopyTo(data);

                    // PSP stores the BCn colour endpoints/indices halves swapped vs BCn:
                    // [idx_lo, idx_hi, color0, color1] on PSP -> [color0, color1, idx_lo, idx_hi].
                    var src = MemoryMarshal.Cast<byte, ushort>(data);
                    if (TPF.tpf == eSCE_GE_TPF.SCE_GE_TPF_DXT1)
                    {
                        // DXT1 blocks are 8 bytes (4 u16) = just the colour block.
                        for (int i = 0; i + 4 <= src.Length; i += 4)
                        {
                            (src[i + 0], src[i + 2]) = (src[i + 2], src[i + 0]);
                            (src[i + 1], src[i + 3]) = (src[i + 3], src[i + 1]);
                        }
                    }
                    else
                    {
                        // DXT3/DXT5 blocks are 16 bytes (8 u16) = [alpha block][colour block].
                        // The alpha block (u16 0..3) is stored as-is; only the colour block (u16 4..7)
                        // has its endpoint/index halves swapped.
                        for (int i = 0; i + 8 <= src.Length; i += 8)
                        {
                            (src[i + 4], src[i + 6]) = (src[i + 6], src[i + 4]);
                            (src[i + 5], src[i + 7]) = (src[i + 7], src[i + 5]);
                        }
                    }

                    ColorRgba32[] colors = decoder.DecodeRaw(data.ToArray(), width, BufferInfo.Height, bcType);
                    ReadOnlySpan<Rgba32> rgba32 = MemoryMarshal.Cast<ColorRgba32, Rgba32>(colors);
                    img = Image.LoadPixelData(rgba32, width, BufferInfo.Height);
                    break;
                }
        }

        if (img.Width != regionWidth || img.Height != regionHeight)
        {
            img.Mutate(e => e.Crop(regionWidth, regionHeight));
        }

        return img;
    }

    void Swizzle(Span<byte> data, Span<byte> output, int width, int height, int bpp)
    {
        int stride = (width * bpp) / 8;
        int rowBlocks = stride / 16;
        int outputOffset = 0;

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < stride; x++)
            {
                int blockX = x / 16;
                int blockY = y / 8;
                int blockIndex = blockX + (blockY * rowBlocks);
                int blockAddress = blockIndex * 16 * 8;
                output[outputOffset] = data[blockAddress + (x - blockX * 16) + ((y - blockY * 8) * 16)];
                outputOffset++;
            }
        }
    }

    /// <summary>
    /// Row stride (in texels) for a PSP texture: a power-of-two width for 3SXT containers, or a
    /// 16-byte-aligned row for Tpp1 containers (16-byte alignment is required by the swizzler).
    /// </summary>
    public static int RowStrideTexels(int width, int bpp, bool tpp1)
    {
        // The swizzler works on 16-byte blocks, so the row stride must be at least 16 bytes.
        int minTexels = 16 * 8 / bpp;

        if (tpp1)
        {
            int strideBytes = (((width * bpp + 7) / 8) + 15) & ~15;
            return Math.Max(strideBytes * 8 / bpp, minTexels);
        }

        return Math.Max((int)BitOperations.RoundUpToPowerOf2((uint)width), minTexels);
    }

    /// <summary>
    /// Builds a PSP 8888/RGBA texture from a standard image file. The pixel data is padded to the
    /// row stride and swizzled (high-speed mode), matching how the game stores its own 8888
    /// textures. <paramref name="tpp1"/> selects the Tpp1 (16-byte-aligned) stride instead of 3SXT.
    /// </summary>
    public static PGLUGETextureInfo CreateFrom8888(string imagePath, bool tpp1 = false)
    {
        using Image<Rgba32> image = Image.Load<Rgba32>(imagePath);

        int width = image.Width;
        int height = image.Height;
        int mipWidth = RowStrideTexels(width, 32, tpp1);
        int strideBytes = mipWidth * 4;
        int dataHeight = (height + 7) & ~7; // swizzle operates on 8-row blocks

        // Tightly-packed source pixels (R,G,B,A), re-laid at the padded row stride.
        byte[] src = new byte[width * height * 4];
        image.CopyPixelDataTo(src);

        byte[] linear = new byte[strideBytes * dataHeight];
        for (int y = 0; y < height; y++)
            Array.Copy(src, y * width * 4, linear, y * strideBytes, width * 4);

        byte[] swizzled = new byte[strideBytes * dataHeight];
        SwizzleForWrite(linear, swizzled, mipWidth, dataHeight, 32);

        var tex = new PGLUGETextureInfo();
        tex.Name = Path.GetFileName(imagePath);
        tex.TPF.tpf = eSCE_GE_TPF.SCE_GE_TPF_8888;
        tex.TMODE.hsm = SCE_GE_TMODE_HSM.SCE_GE_TMODE_HSM_HIGHSPEED;
        tex.MipInfos[0] = new GEMipInfo { Width = (ushort)mipWidth };
        tex.BufferInfo = new GETextureBuffer
        {
            Width = (ushort)width,
            Height = (ushort)height,
            FormatBits = eSCE_GE_TPF.SCE_GE_TPF_8888,
            LastMipmapLevel = 1,
            ImageData = swizzled,
            ImageSize = (uint)swizzled.Length,
        };
        return tex;
    }

    /// <summary>
    /// Builds a PSP (3SXT) paletted texture (IDTEX4 = 16 colors, IDTEX8 = 256 colors) from a
    /// standard image file. The image is colour-quantized into a 32-bit (C8888) CLUT, the indices
    /// are packed, padded to a power-of-two stride and swizzled (high-speed mode).
    /// </summary>
    public static PGLUGETextureInfo CreateFromIndexed(string imagePath, eSCE_GE_TPF format, bool tpp1 = false)
    {
        int maxColors;
        int bpp;
        if (format == eSCE_GE_TPF.SCE_GE_TPF_IDTEX4) { maxColors = 16; bpp = 4; }
        else if (format == eSCE_GE_TPF.SCE_GE_TPF_IDTEX8) { maxColors = 256; bpp = 8; }
        else throw new ArgumentException($"{format} is not an indexed PSP format.", nameof(format));

        using Image<Rgba32> image = Image.Load<Rgba32>(imagePath);
        int width = image.Width;
        int height = image.Height;
        int mipWidth = RowStrideTexels(width, bpp, tpp1);
        int strideBytes = (mipWidth * bpp) / 8;

        // Tpp1 always swizzles; 3SXT swizzles only when the width fills the stride (mipWidth==pow2(W)).
        // Swizzled data must be whole 8-row blocks; LINEAR data uses the real height (PD doesn't pad
        // linear textures - padding it to align8 inflates the file and shifts later GPB textures).
        int texWidth = (int)BitOperations.RoundUpToPowerOf2((uint)width);
        bool swizzle = tpp1 || mipWidth == texWidth;
        int dataHeight = swizzle ? ((height + 7) & ~7) : height;

        byte[] sourcePixels = new byte[width * height * 4];
        image.CopyPixelDataTo(sourcePixels); // tightly-packed R,G,B,A

        byte[] clutData = new byte[maxColors * 4]; // C8888, padded to the full palette
        byte[] pixelIndices = new byte[width * height];

        // Prefer an exact palette: if the image already has <= maxColors unique colours (true for
        // the game's own UI/map textures) this is lossless. Quantization is only a fallback for
        // true-colour inputs, since the quantizer's nearest-match underweights alpha differences.
        var colorToIndex = new Dictionary<uint, int>();
        bool exact = true;
        for (int i = 0; i < width * height; i++)
        {
            uint color = BitConverter.ToUInt32(sourcePixels, i * 4);
            if (!colorToIndex.TryGetValue(color, out int idx))
            {
                if (colorToIndex.Count >= maxColors) { exact = false; break; }
                idx = colorToIndex.Count;
                colorToIndex[color] = idx;
                Array.Copy(sourcePixels, i * 4, clutData, idx * 4, 4);
            }
            pixelIndices[i] = (byte)idx;
        }

        if (!exact)
        {
            var quantizer = new WuQuantizer(new QuantizerOptions { MaxColors = maxColors, Dither = null });
            using IQuantizer<Rgba32> frameQuantizer = quantizer.CreatePixelSpecificQuantizer<Rgba32>(Configuration.Default);
            using IndexedImageFrame<Rgba32> indexed = frameQuantizer.BuildPaletteAndQuantizeFrame(
                image.Frames.RootFrame, new SixLabors.ImageSharp.Rectangle(0, 0, width, height));

            ReadOnlySpan<Rgba32> palette = indexed.Palette.Span;
            Array.Clear(clutData);
            for (int i = 0; i < palette.Length && i < maxColors; i++)
            {
                clutData[i * 4 + 0] = palette[i].R;
                clutData[i * 4 + 1] = palette[i].G;
                clutData[i * 4 + 2] = palette[i].B;
                clutData[i * 4 + 3] = palette[i].A;
            }

            for (int y = 0; y < height; y++)
            {
                ReadOnlySpan<byte> indices = indexed.DangerousGetRowSpan(y);
                for (int x = 0; x < width; x++)
                    pixelIndices[y * width + x] = indices[x];
            }
        }

        // Pack indices at the padded row stride. IDTEX4 (GU_PSM_T4) stores the even pixel in
        // the LOW nibble and the odd pixel in the HIGH nibble.
        byte[] linear = new byte[strideBytes * dataHeight];
        for (int y = 0; y < height; y++)
        {
            int rowBase = y * strideBytes;
            for (int x = 0; x < width; x++)
            {
                byte idx = pixelIndices[y * width + x];
                if (bpp == 8)
                {
                    linear[rowBase + x] = idx;
                }
                else
                {
                    int bi = rowBase + (x >> 1);
                    linear[bi] |= (x & 1) == 0 ? (byte)(idx & 0xF) : (byte)(idx << 4);
                }
            }
        }

        // (swizzle decided up top: Tpp1 always; 3SXT only when mipWidth == pow2(W). A narrow 3SXT
        // IDTEX is LINEAR/hsm=0 - swizzling it there mis-sets TSIZE0 -> invisible UI.)
        byte[] imageData;
        if (swizzle)
        {
            imageData = new byte[strideBytes * dataHeight];
            SwizzleForWrite(linear, imageData, mipWidth, dataHeight, bpp);
        }
        else
        {
            imageData = linear;
        }

        var tex = new PGLUGETextureInfo();
        tex.Name = Path.GetFileName(imagePath);
        tex.TPF.tpf = format;
        tex.TMODE.hsm = swizzle ? SCE_GE_TMODE_HSM.SCE_GE_TMODE_HSM_HIGHSPEED : SCE_GE_TMODE_HSM.SCE_GE_TMODE_HSM_NORMAL;
        tex.MipInfos[0] = new GEMipInfo { Width = (ushort)mipWidth };
        tex.BufferInfo = new GETextureBuffer
        {
            Width = (ushort)width,
            Height = (ushort)height,
            FormatBits = format,
            LastMipmapLevel = 1,
            ImageData = imageData,
            ImageSize = (uint)imageData.Length,
        };
        // 3SXT stores exactly the colours the image uses (its clutinfo carries an explicit NumColors,
        // e.g. a 4-colour dot => 4) - matching PD keeps the tail compact. Tpp1 has NO clutinfo: its
        // decoder derives the count from CLOAD (whole 8-entry blocks), so its palette MUST stay the
        // full maxColors size or the decode runs off the end ("Could not read 32 bytes").
        int usedColors = (exact && !tpp1) ? colorToIndex.Count : maxColors;
        byte[] finalClut = new byte[usedColors * 4];
        Array.Copy(clutData, finalClut, usedColors * 4);

        tex.ClutBufferInfo = new GEClutBufferInfo
        {
            ClutType = SCE_GE_CLUT_CPF.SCE_GE_CLUT_CPF_8888,
            NumColors = (ushort)usedColors,
            ClutData = finalClut,
        };
        return tex;
    }

    /// <summary>
    /// Builds the texture data for a Tpp1 container (8888 / IDTEX4 / IDTEX8). Same encoding as the
    /// 3SXT path but using Tpp1's 16-byte-aligned row stride; written by <see cref="TextureSet3.BuildTpp1File"/>.
    /// </summary>
    public static PGLUGETextureInfo CreateTpp1(string imagePath, eSCE_GE_TPF format) => format switch
    {
        eSCE_GE_TPF.SCE_GE_TPF_8888 => CreateFrom8888(imagePath, tpp1: true),
        eSCE_GE_TPF.SCE_GE_TPF_IDTEX4 => CreateFromIndexed(imagePath, format, tpp1: true),
        eSCE_GE_TPF.SCE_GE_TPF_IDTEX8 => CreateFromIndexed(imagePath, format, tpp1: true),
        _ => throw new NotSupportedException($"Tpp1 building supports 8888 / IDTEX4 / IDTEX8 (got {format})."),
    };

    /// <summary>
    /// Builds a PSP (3SXT) DXT1 texture from a standard image file.
    /// </summary>
    public static PGLUGETextureInfo CreateFromDXT1(string imagePath)
        => CreateFromDXT(imagePath, eSCE_GE_TPF.SCE_GE_TPF_DXT1);

    /// <summary>
    /// Builds a PSP (3SXT) DXT1/DXT5 texture from a standard image file. The image is BCn-encoded
    /// and the colour block's endpoint/index halves are swapped to PSP order (the alpha block of
    /// DXT5 is left as-is). DXT data is stored linearly (not swizzled).
    /// </summary>
    public static PGLUGETextureInfo CreateFromDXT(string imagePath, eSCE_GE_TPF format)
    {
        bool isDxt5 = format == eSCE_GE_TPF.SCE_GE_TPF_DXT5;
        if (format != eSCE_GE_TPF.SCE_GE_TPF_DXT1 && !isDxt5)
            throw new ArgumentException($"{format} is not a supported DXT format (use DXT1 or DXT5).", nameof(format));

        using Image<Rgba32> image = Image.Load<Rgba32>(imagePath);
        int width = image.Width;
        int height = image.Height;
        int encWidth = (width + 3) & ~3; // DXT blocks are 4x4
        int encHeight = (height + 3) & ~3;

        byte[] source = new byte[width * height * 4];
        image.CopyPixelDataTo(source); // tightly-packed R,G,B,A

        var encoder = new BcEncoder();
        encoder.OutputOptions.Format = isDxt5 ? CompressionFormat.Bc3 : CompressionFormat.Bc1;
        encoder.OutputOptions.Quality = CompressionQuality.BestQuality;
        encoder.OutputOptions.GenerateMipMaps = false;
        byte[] bc = encoder.EncodeToRawBytes(source, width, height, PixelFormat.Rgba32)[0];

        // BCn colour block stores [color0, color1, idx_lo, idx_hi]; PSP wants
        // [idx_lo, idx_hi, color0, color1] - swap the two 4-byte halves of the colour block.
        // (DXT5 = 16-byte blocks with the colour block at +8; the 8-byte alpha block is untouched.)
        int blockSize = isDxt5 ? 16 : 8;
        int colourOffset = isDxt5 ? 8 : 0;
        for (int i = 0; i + blockSize <= bc.Length; i += blockSize)
        {
            for (int k = 0; k < 4; k++)
                (bc[i + colourOffset + k], bc[i + colourOffset + 4 + k]) = (bc[i + colourOffset + 4 + k], bc[i + colourOffset + k]);
        }

        var tex = new PGLUGETextureInfo();
        tex.Name = Path.GetFileName(imagePath);
        tex.TPF.tpf = format;
        tex.TMODE.hsm = SCE_GE_TMODE_HSM.SCE_GE_TMODE_HSM_NORMAL; // DXT is not swizzled
        tex.MipInfos[0] = new GEMipInfo { Width = (ushort)encWidth };
        tex.BufferInfo = new GETextureBuffer
        {
            Width = (ushort)width,
            Height = (ushort)height,
            FormatBits = format,
            LastMipmapLevel = 1,
            ImageData = bc,
            ImageSize = (uint)bc.Length,
        };
        return tex;
    }

    /// <summary>
    /// Inverse of <see cref="Swizzle"/> - lays out linear pixel rows into the PSP's
    /// 16-byte x 8-row swizzled block order.
    /// </summary>
    static void SwizzleForWrite(ReadOnlySpan<byte> linear, Span<byte> output, int width, int height, int bpp)
    {
        int stride = (width * bpp) / 8;
        int rowBlocks = stride / 16;
        int srcOffset = 0;

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < stride; x++)
            {
                int blockX = x / 16;
                int blockY = y / 8;
                int blockIndex = blockX + (blockY * rowBlocks);
                int blockAddress = blockIndex * 16 * 8;
                output[blockAddress + (x - blockX * 16) + ((y - blockY * 8) * 16)] = linear[srcOffset];
                srcOffset++;
            }
        }
    }
}

public class GEMipInfo
{
    public ushort Width;
    public byte Unk1;
    public byte Index;
}