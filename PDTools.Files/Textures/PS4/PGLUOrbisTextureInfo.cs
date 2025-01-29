using System;
using System.Buffers;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

using BCnEncoder.Decoder;
using BCnEncoder.Shared;

using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp;

using Syroot.BinaryData;

namespace PDTools.Files.Textures.PS4;

/// <summary>
/// PDI-GL Orbis (PS4) Texture info. Thin wrapper over GNF texture parameters
/// </summary>
public class PGLUOrbisTextureInfo : PGLUTextureInfo
{
    public GNFSurfaceFormat SurfaceFormat { get; set; }
    public GNFRenderTargetChannelType ChannelType { get; set; }
    public int Width { get; set; }
    public int Height { get; set; }
    public int SamplerModulationFormat { get; set; }
    public GNFTextureChannel ChannelOrderX { get; set; }
    public GNFTextureChannel ChannelOrderY { get; set; }
    public GNFTextureChannel ChannelOrderZ { get; set; }
    public GNFTextureChannel ChannelOrderW { get; set; }
    public int BaseMipLevel { get; set; }
    public int LastMipLevel { get; set; }
    public GNFTileMode TileMode { get; set; }
    public bool IsPaddedToPow2 { get; set; }
    public GNFTextureType TextureType { get; set; }

    public int Depth { get; set; }
    public int Pitch { get; set; }
    public int ImageSize { get; set; }

    public override void Read(BinaryStream bs, long basePos)
    {
        bs.ReadInt32(); // Nothing
        int bits = bs.ReadInt32();
        SurfaceFormat = (GNFSurfaceFormat)(bits >> 20 & 0b111111);
        ChannelType = (GNFRenderTargetChannelType)(bits >> 26 & 0b1111);
        // 2 bits empty


        bits = bs.ReadInt32();
        Width = (bits & 0b111111_11111111) + 1; // 14 bits
        Height = (bits >> 14 & 0b111111_11111111) + 1; // 14 bits
        SamplerModulationFormat = bits >> 28 & 0b1111; // 4 bits

        bits = bs.ReadInt32();
        ChannelOrderX = (GNFTextureChannel)(bits & 0b111);
        ChannelOrderY = (GNFTextureChannel)(bits >> 3 & 0b111);
        ChannelOrderZ = (GNFTextureChannel)(bits >> 6 & 0b111);
        ChannelOrderW = (GNFTextureChannel)(bits >> 9 & 0b111);
        BaseMipLevel = bits >> 12 & 0b1111;
        LastMipLevel = bits >> 16 & 0b1111;
        TileMode = (GNFTileMode)(bits >> 20 & 0b11111);
        IsPaddedToPow2 = (bits >> 25 & 1) == 1;
        TextureType = (GNFTextureType)(bits >> 28 & 0b1111);

        bits = bs.ReadInt32();
        Depth = bits & 0b11111_11111111;
        Pitch = (bits >> 13 & 0b111111_11111111) + 1;
        bs.Position += 8;

        ImageSize = bs.ReadInt32();

        uint field_0x20 = bs.ReadUInt32();
        uint field_0x24 = bs.ReadUInt32();
        uint field_0x28 = bs.ReadUInt32();
        uint field_0x2C = bs.ReadUInt32();
        uint field_0x30 = bs.ReadUInt32();
        uint field_0x34 = bs.ReadUInt16();
        BufferId = bs.ReadUInt16();
        uint field_0x38 = bs.ReadUInt32();
        uint field_0x3C = bs.ReadUInt32();
        uint fileNameOffset = bs.ReadUInt32();
        uint field_0x44 = bs.ReadUInt32();

        bs.Position = fileNameOffset - basePos;
        Name = bs.ReadString(StringCoding.ZeroTerminated);
    }

    public override void Write(BinaryStream bs)
    {
        throw new NotImplementedException();
    }

    public byte[] GetDDS()
    {
        byte[] unswizzled = ArrayPool<byte>.Shared.Rent(BufferInfo.ImageData.Length);

        int rawWidth = Pitch;
        int paddedHeight = IsPaddedToPow2 ? (int)BitOperations.RoundUpToPowerOf2((uint)Height) : Height;

        Swizzler.DoSwizzle(BufferInfo.ImageData.Span, unswizzled, rawWidth, paddedHeight, 16);

        var header = new DdsHeader();
        header.Height = Height;
        header.Width = Width;
        header.PitchOrLinearSize = Height * Width;
        header.LastMipmapLevel = 1;
        header.FormatFlags = DDSPixelFormatFlags.DDPF_FOURCC;
        header.FourCCName = "DX10";

        var dxgiFormat = this.SurfaceFormat switch
        {
            GNFSurfaceFormat.kSurfaceFormatBc1 => DDS_DXGI_FORMAT.DXGI_FORMAT_BC1_UNORM,
            GNFSurfaceFormat.kSurfaceFormatBc2 => DDS_DXGI_FORMAT.DXGI_FORMAT_BC2_UNORM,
            GNFSurfaceFormat.kSurfaceFormatBc3 => DDS_DXGI_FORMAT.DXGI_FORMAT_BC3_UNORM,
            GNFSurfaceFormat.kSurfaceFormatBc5 => DDS_DXGI_FORMAT.DXGI_FORMAT_BC5_UNORM,
            GNFSurfaceFormat.kSurfaceFormatBc7 => DDS_DXGI_FORMAT.DXGI_FORMAT_BC7_UNORM,
            _ => throw new NotImplementedException($"Orbis texture surface format {this.SurfaceFormat} is not yet supported")
        };

        header.DxgiFormat = dxgiFormat;
        header.ImageData = unswizzled;

        using var ms = new MemoryStream();
        header.Write(ms);
        ms.Position = 0;

        ArrayPool<byte>.Shared.Return(unswizzled);

        return ms.ToArray();
    }

    public override Image GetAsImage()
    {
        byte[] unswizzled = ArrayPool<byte>.Shared.Rent(BufferInfo.ImageData.Length);

        int rawWidth = Pitch;
        int paddedHeight = IsPaddedToPow2 ? (int)BitOperations.RoundUpToPowerOf2((uint)Height) : Height;

        Swizzler.DoSwizzle(BufferInfo.ImageData.Span, unswizzled, rawWidth, paddedHeight, 16);

        var bcType = this.SurfaceFormat switch
        {
            GNFSurfaceFormat.kSurfaceFormatBc1 => CompressionFormat.Bc1,
            GNFSurfaceFormat.kSurfaceFormatBc2 => CompressionFormat.Bc2,
            GNFSurfaceFormat.kSurfaceFormatBc3 => CompressionFormat.Bc3,
            GNFSurfaceFormat.kSurfaceFormatBc5 => CompressionFormat.Bc5,
            GNFSurfaceFormat.kSurfaceFormatBc7 => CompressionFormat.Bc7,
            _ => throw new NotImplementedException($"Orbis texture surface format {this.SurfaceFormat} is not yet supported")
        };

        BcDecoder decoder = new BcDecoder();
        ColorRgba32[] colors = decoder.DecodeRaw(unswizzled, rawWidth, paddedHeight, bcType);

        ReadOnlySpan<Rgba32> rgba32 = MemoryMarshal.Cast<ColorRgba32, Rgba32>(colors);
        Image<Rgba32> image = Image.LoadPixelData(rgba32, rawWidth, paddedHeight);

        if (rawWidth != Width)
        {
            image.Mutate(e => e.Crop(Width, Height));
        }

        ArrayPool<byte>.Shared.Return(unswizzled);

        return image;
    }
}
