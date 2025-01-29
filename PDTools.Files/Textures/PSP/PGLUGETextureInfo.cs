using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection.Emit;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

using BCnEncoder.Decoder;
using BCnEncoder.Shared;

using PDTools.Files.Textures.PS2;
using PDTools.Utils;

using SixLabors.Fonts;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;

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

    public override Image GetAsImage()
    {
        var mip1 = MipInfos[0];
        int width = mip1.Width;
        int height = 1 << mip1.Unk1;

        int regionWidth = (int)(width * UMIN);
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
                    BitStream bs = new BitStream(BitStreamMode.Read, data, BitStreamSignificantBitOrder.MSB);

                    for (var y = 0; y < BufferInfo.Height; y++)
                    {
                        for (var x = 0; x < width; x++)
                        {
                            int idx = (int)bs.ReadBits(bpp);
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

                    var src = MemoryMarshal.Cast<byte, ushort>(data);
                    int size = data.Length;
                    for (int j = 0; size >= 16; size -= 16, j++)
                    {
                        ushort[] converted = new ushort[8];

                        converted[4] = src[1];
                        converted[5] = src[2];
                        converted[6] = src[3];
                        converted[7] = src[0];

                        converted[0] = src[6];
                        converted[1] = src[7];
                        converted[2] = src[4];
                        converted[3] = src[5];

                        for (int i = 0; i < 8; i++)
                            src[i] = converted[i];

                        src = src[8..];
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
}

public class GEMipInfo
{
    public ushort Width;
    public byte Unk1;
    public byte Index;
}