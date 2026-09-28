// Copyright (c) 2026 Nenkai
// SPDX-License-Identifier: MIT

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
    public int BaseArray { get; set; }
    public int LastArray { get; set; }

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

        bits = bs.ReadInt32();
        BaseArray = bits & 0b11111_11111111;
        LastArray = bits >> 13 & 0b11111_11111111;
        bs.Position += 4;

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

        // basePos = absolute position of the set's offset 0 (set position minus its reloc pointer).
        bs.Position = basePos + fileNameOffset;
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

    public bool IsBlockCompressed => SurfaceFormat >= GNFSurfaceFormat.kSurfaceFormatBc1 && SurfaceFormat <= GNFSurfaceFormat.kSurfaceFormatBc7;

    /// <summary>Bytes per element: a 4x4 block for BCn, a texel otherwise.</summary>
    public int ElementBytes => SurfaceFormat switch
    {
        GNFSurfaceFormat.kSurfaceFormatBc1 or GNFSurfaceFormat.kSurfaceFormatBc4 => 8,
        >= GNFSurfaceFormat.kSurfaceFormatBc1 and <= GNFSurfaceFormat.kSurfaceFormatBc7 => 16,
        GNFSurfaceFormat.kSurfaceFormat8 => 1,
        GNFSurfaceFormat.kSurfaceFormat16 or GNFSurfaceFormat.kSurfaceFormat8_8 or GNFSurfaceFormat.kSurfaceFormat5_6_5
            or GNFSurfaceFormat.kSurfaceFormat4_4_4_4 or GNFSurfaceFormat.kSurfaceFormat1_5_5_5 or GNFSurfaceFormat.kSurfaceFormat5_5_5_1 => 2,
        GNFSurfaceFormat.kSurfaceFormat32 or GNFSurfaceFormat.kSurfaceFormat16_16 or GNFSurfaceFormat.kSurfaceFormat10_11_11
            or GNFSurfaceFormat.kSurfaceFormat11_11_10 or GNFSurfaceFormat.kSurfaceFormat10_10_10_2 or GNFSurfaceFormat.kSurfaceFormat2_10_10_10
            or GNFSurfaceFormat.kSurfaceFormat8_8_8_8 or GNFSurfaceFormat.kSurfaceFormat5_9_9_9 => 4,
        GNFSurfaceFormat.kSurfaceFormat32_32 or GNFSurfaceFormat.kSurfaceFormat16_16_16_16 => 8,
        GNFSurfaceFormat.kSurfaceFormat32_32_32_32 => 16,
        _ => throw new NotImplementedException($"Orbis texture surface format {SurfaceFormat} is not yet supported")
    };

    /// <summary>Number of 2D surfaces stored at mip 0: array layers, cube faces or 3D depth slices.</summary>
    public int SliceCount => TextureType switch
    {
        GNFTextureType.kTextureType3d => Depth + 1,
        GNFTextureType.kTextureTypeCubemap or GNFTextureType.kTextureType2dArray or GNFTextureType.kTextureType1dArray
            => Math.Max(1, LastArray - BaseArray + 1),
        _ => 1,
    };

    public override Image GetAsImage() => GetSlices(1)[0];

    /// <summary>
    /// Mip 0 of every slice. Element grid = Pitch x (padded) Height in texels, /4 for BCn; tiles are
    /// 8x8 elements in Morton order (verified against BC7), slices follow each other at mip 0 size.
    /// HDR formats (BC6H, 10_11_11, float) are clamped to 0..1 into 16-bit channels.
    /// </summary>
    public List<Image> GetSlices(int max = int.MaxValue)
    {
        int bw = IsBlockCompressed ? 4 : 1;
        int paddedHeight = IsPaddedToPow2 ? (int)BitOperations.RoundUpToPowerOf2((uint)Height) : Height;
        int ew = (Pitch + bw - 1) / bw, eh = (paddedHeight + bw - 1) / bw;
        int ewAligned = (ew + 7) & ~7, ehAligned = (eh + 7) & ~7;
        int eb = ElementBytes;
        int sliceBytes = ewAligned * ehAligned * eb;

        var images = new List<Image>();
        ReadOnlySpan<byte> data = BufferInfo.ImageData.Span;
        int slices = Math.Min(Math.Min(SliceCount, max), Math.Max(1, data.Length / sliceBytes));
        for (int s = 0; s < slices; s++)
        {
            byte[] linear = new byte[ew * eh * eb];
            Swizzler.Untile(data.Slice(s * sliceBytes, Math.Min(sliceBytes, data.Length - s * sliceBytes)), linear, ew, eh, eb);
            Image img = Decode(linear, ew * bw, eh * bw);
            if (ew * bw != Width || eh * bw != Height)
                img.Mutate(e => e.Crop(Math.Min(Width, img.Width), Math.Min(Height, img.Height)));
            images.Add(img);
        }
        return images;
    }

    private Image Decode(byte[] linear, int w, int h)
    {
        var decoder = new BcDecoder();
        switch (SurfaceFormat)
        {
            case GNFSurfaceFormat.kSurfaceFormatBc6:
            {
                ColorRgbFloat[] hdr = decoder.DecodeRawHdr(linear, w, h, CompressionFormat.Bc6U);
                var px = new Rgba64[hdr.Length];
                for (int i = 0; i < hdr.Length; i++)
                    px[i] = new Rgba64(new System.Numerics.Vector4(Math.Clamp(hdr[i].r, 0, 1), Math.Clamp(hdr[i].g, 0, 1), Math.Clamp(hdr[i].b, 0, 1), 1));
                return Image.LoadPixelData<Rgba64>(px, w, h);
            }
            case >= GNFSurfaceFormat.kSurfaceFormatBc1 and <= GNFSurfaceFormat.kSurfaceFormatBc7:
            {
                var fmt = SurfaceFormat switch
                {
                    GNFSurfaceFormat.kSurfaceFormatBc1 => CompressionFormat.Bc1WithAlpha,
                    GNFSurfaceFormat.kSurfaceFormatBc2 => CompressionFormat.Bc2,
                    GNFSurfaceFormat.kSurfaceFormatBc3 => CompressionFormat.Bc3,
                    GNFSurfaceFormat.kSurfaceFormatBc4 => CompressionFormat.Bc4,
                    GNFSurfaceFormat.kSurfaceFormatBc5 => CompressionFormat.Bc5,
                    _ => CompressionFormat.Bc7,
                };
                ColorRgba32[] colors = decoder.DecodeRaw(linear, w, h, fmt);
                return Image.LoadPixelData<Rgba32>(MemoryMarshal.Cast<ColorRgba32, Rgba32>(colors), w, h);
            }
            case GNFSurfaceFormat.kSurfaceFormat8_8_8_8:
                return Image.LoadPixelData<Rgba32>(linear, w, h);
            case GNFSurfaceFormat.kSurfaceFormat8:
                return Image.LoadPixelData<L8>(linear, w, h);
            case GNFSurfaceFormat.kSurfaceFormat8_8:
            {
                var px = new Rgba32[w * h];
                for (int i = 0; i < px.Length; i++) px[i] = new Rgba32(linear[i * 2], linear[i * 2 + 1], 0, 255);
                return Image.LoadPixelData<Rgba32>(px, w, h);
            }
            case GNFSurfaceFormat.kSurfaceFormat16:
                return Image.LoadPixelData<L16>(linear, w, h);
            case GNFSurfaceFormat.kSurfaceFormat32:
            {
                // Float (the usual channel type for 32-bit single-channel GT7 data); clamped 0..1.
                var px = new L16[w * h];
                for (int i = 0; i < px.Length; i++)
                    px[i] = new L16((ushort)(Math.Clamp(BitConverter.ToSingle(linear, i * 4), 0f, 1f) * 65535));
                return Image.LoadPixelData<L16>(px, w, h);
            }
            case GNFSurfaceFormat.kSurfaceFormat10_11_11:
            case GNFSurfaceFormat.kSurfaceFormat11_11_10:
            {
                // GCN 10_11_11 = R11G11B10_FLOAT in DXGI terms (R low bits).
                var px = new Rgba64[w * h];
                for (int i = 0; i < px.Length; i++)
                {
                    uint v = BitConverter.ToUInt32(linear, i * 4);
                    float r = Float11(v & 0x7FF), g = Float11((v >> 11) & 0x7FF), b = Float10((v >> 22) & 0x3FF);
                    px[i] = new Rgba64(new System.Numerics.Vector4(Math.Clamp(r, 0, 1), Math.Clamp(g, 0, 1), Math.Clamp(b, 0, 1), 1));
                }
                return Image.LoadPixelData<Rgba64>(px, w, h);
            }
            case GNFSurfaceFormat.kSurfaceFormat16_16:
            {
                bool isFloat = ChannelType == GNFRenderTargetChannelType.kRenderTargetChannelTypeFloat;
                var px = new Rgba64[w * h];
                for (int i = 0; i < px.Length; i++)
                {
                    float c(int k) => isFloat
                        ? Math.Clamp((float)BitConverter.ToHalf(linear, i * 4 + k * 2), 0f, 1f)
                        : BitConverter.ToUInt16(linear, i * 4 + k * 2) / 65535f;
                    px[i] = new Rgba64(new System.Numerics.Vector4(c(0), c(1), 0, 1));
                }
                return Image.LoadPixelData<Rgba64>(px, w, h);
            }
            case GNFSurfaceFormat.kSurfaceFormat16_16_16_16:
            {
                var px = new Rgba64[w * h];
                for (int i = 0; i < px.Length; i++)
                {
                    float c(int k) => Math.Clamp((float)BitConverter.ToHalf(linear, i * 8 + k * 2), 0f, 1f);
                    px[i] = new Rgba64(new System.Numerics.Vector4(c(0), c(1), c(2), c(3)));
                }
                return Image.LoadPixelData<Rgba64>(px, w, h);
            }
            default:
                throw new NotImplementedException($"Orbis texture surface format {SurfaceFormat} is not yet supported");
        }
    }

    private static float Float11(uint v) => SmallFloat(v >> 6, v & 0x3F, 6);
    private static float Float10(uint v) => SmallFloat(v >> 5, v & 0x1F, 5);

    private static float SmallFloat(uint exp, uint mant, int mantBits)
    {
        if (exp == 0) return mant / (float)(1 << mantBits) * MathF.Pow(2, -14);
        if (exp == 31) return mant == 0 ? float.PositiveInfinity : float.NaN;
        return (1 + mant / (float)(1 << mantBits)) * MathF.Pow(2, (int)exp - 15);
    }
}
