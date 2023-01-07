using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Buffers;
using System.Runtime.InteropServices;
using System.Numerics;
using Syroot.BinaryData.Core;
using Syroot.BinaryData;

using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.Formats;

using BCnEncoder.Decoder;
using BCnEncoder.Shared;


namespace PDTools.Files.Textures
{
    public class OrbisTexture : Texture
    {
        public string Name { get; set; }

        public int Mipmap { get; set; }

        public ushort Width { get; set; }
        public ushort Height { get; set; }

        public OrbisTexture()
        {

        }

        public override void ReadTextureDetails(BinaryStream bs)
        {
            ImageOffset = bs.ReadUInt32();
            bs.ReadUInt32();
            ImageSize = bs.ReadUInt32();
            bs.ReadUInt32();

            Width = bs.ReadUInt16();
            Height = bs.ReadUInt16();

            bs.ReadInt32(); // Unknown 1
        }

        public void InitFromDDSImage(Pfim.IImage image)
        {
            Width = (ushort)image.Width;
            Height = (ushort)image.Height;
            Mipmap = image.MipMaps.Length;
        }

        public override void ConvertFileToPng(string outputFile)
        {
            byte[] unswizzled = ArrayPool<byte>.Shared.Rent(ImageData.Length);

            PGLUOrbisTextureInfo textureInfo = TextureRenderInfo as PGLUOrbisTextureInfo;

            int rawWidth = textureInfo.Pitch;
            BcDecoder decoder = new BcDecoder();

            int paddedHeight = textureInfo.IsPaddedToPow2 ? (int)BitOperations.RoundUpToPowerOf2((uint)Height) : Height;
            DoSwizzle(ImageData.Span, unswizzled, rawWidth, paddedHeight, 16);

            ColorRgba32[] colors = decoder.DecodeRaw(unswizzled, rawWidth, paddedHeight, BCnEncoder.Shared.CompressionFormat.Bc7);

            ReadOnlySpan<Rgba32> rgba32 = MemoryMarshal.Cast<ColorRgba32, Rgba32>(colors);
            Image<Rgba32> image = Image.LoadPixelData(rgba32, rawWidth, paddedHeight);

            
            if (rawWidth != Width)
            {
                image.Mutate(e => e.Crop(Width, Height));
            }

            image.SaveAsPng(outputFile);

            ArrayPool<byte>.Shared.Return(unswizzled);
        }

        private void DoSwizzle(Span<byte> input, Span<byte> output, int width, int height, int blockSize)
        {
            var heightTexels = height / 4;
            var heightTexelsAligned = (heightTexels + 7) / 8;
            int widthTexels = width / 4;
            var widthTexelsAligned = (widthTexels + 7) / 8;
            var dataIndex = 0;

            for (int y = 0; y < heightTexelsAligned; y++)
            {
                for (int x = 0; x < widthTexelsAligned; x++)
                {
                    for (int t = 0; t < 64; t++)
                    {
                        int pixelIndex = Swizzler.MortonReorder(t, 8, 8);
                        int cPixel = pixelIndex / 8;
                        int remPixel = pixelIndex % 8;
                        var yOffset = (y * 8) + cPixel;
                        var xOffset = (x * 8) + remPixel;

                        if (xOffset < widthTexels && yOffset < heightTexels)
                        {
                            var destPixelIndex = yOffset * widthTexels + xOffset;
                            int destIndex = blockSize * destPixelIndex;

                            // Memcpy(input + dataIndex, output + destIndex, size)
                            input.Slice(dataIndex, blockSize).CopyTo(output.Slice(destIndex, blockSize));
                        }

                        dataIndex += blockSize;
                    }
                }
            }
        }
    }
}