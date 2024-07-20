using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Syroot.BinaryData.Core;
using Syroot.BinaryData;
using System.IO;

using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.Formats;

using Pfim;
using Pfim.dds;
using System.Buffers.Binary;
using System.Runtime.InteropServices;
using System.Diagnostics;

namespace PDTools.Files.Textures.PS3
{
    public class CellTexture : Texture
    {
        public new string Name
        {
            get => (TextureRenderInfo as PGLUCellTextureInfo)?.Name;
            set
            {
                if (TextureRenderInfo != null)
                    (TextureRenderInfo as PGLUCellTextureInfo).Name = value;
            }
        }

        public int LastMipmapLevel { get; set; }
        public CELL_GCM_TEXTURE_FORMAT FormatBits { get; set; }

        public ushort Width { get; set; }
        public ushort Height { get; set; }

        public short Depth { get; set; } = 1;

        public CellTexture()
        {
            TextureRenderInfo = new PGLUCellTextureInfo();
        }

        public void InitFromDDSImage(IImage image, CELL_GCM_TEXTURE_FORMAT format)
        {
            FormatBits = format;
            Width = (ushort)image.Width;
            Height = (ushort)image.Height;
            LastMipmapLevel = image.MipMaps.Length + 1;

            var textureInfo = TextureRenderInfo as PGLUCellTextureInfo;
            textureInfo.Width = Width;
            textureInfo.Height = Height;
            textureInfo.MipmapLevelLast = (byte)LastMipmapLevel;

            textureInfo.Pitch = Width * 4;

            textureInfo.FormatBits = format | CELL_GCM_TEXTURE_FORMAT.CELL_GCM_TEXTURE_LN;
        }

        public override void ReadTextureDetails(BinaryStream bs)
        {
            ImageOffset = bs.ReadUInt32();
            ImageSize = bs.ReadUInt32();
            bs.ReadByte(); // 2
            FormatBits = (CELL_GCM_TEXTURE_FORMAT)bs.ReadByte();
            LastMipmapLevel = bs.ReadByte();
            bs.ReadByte();

            Width = bs.ReadUInt16();
            Height = bs.ReadUInt16();
            bs.ReadUInt16();
            bs.ReadUInt16();
            bs.ReadInt32(); // Stream Set Offset
            bs.ReadInt32(); // Pad
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

            ImageData = ddsData;
            Name = Path.GetFileNameWithoutExtension(path);

            File.Delete(ddsFileName);
            return true;
        }

        public override void ConvertTextureToStandardFormat(string outputFileName)
        {
            bool isOutputDds = Path.GetExtension(outputFileName) == ".dds";

            using var ms = new MemoryStream();
            CreateDDSData(ImageData.ToArray(), ms); // Change format for DXT10 if we're doing a direct extract to dds
            ms.Position = 0;

            if (isOutputDds)
            {
                File.WriteAllBytes(outputFileName, ms.ToArray());
            }
            else
            {
                var dds = Pfimage.FromStream(ms);

                if (dds.Format == ImageFormat.Rgb24)
                {
                    var i = Image.LoadPixelData<Bgr24>(dds.Data, dds.Width, dds.Height);
                    i.Save(outputFileName);
                }
                else if (dds.Format == ImageFormat.Rgba32)
                {
                    var i = Image.LoadPixelData<Bgra32>(dds.Data, dds.Width, dds.Height);
                    i.Save(outputFileName);
                }
                else
                {
                    Console.WriteLine($"Invalid format to save..? {dds.Format}");
                    return;
                }
            }

        }

        private static void ConvertFileToDDS(string fileName, CELL_GCM_TEXTURE_FORMAT imgFormat)
        {
            string arguments = $"\"{fileName}\"";
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
                      + " -srgb"   // Auto correct gamma
                      + $" -o {Path.GetDirectoryName(fileName)}"; // Set directory to file input's directory

            Process converter = Process.Start(Path.Combine(Directory.GetCurrentDirectory(), "texconv.exe"), arguments);
            converter.WaitForExit();
        }

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
                    //bs.WriteInt32(0);
                    break;
            }


            header.LastMipmapLevel = LastMipmapLevel;

            switch (format)
            {
                case CELL_GCM_TEXTURE_FORMAT.CELL_GCM_TEXTURE_COMPRESSED_DXT1:
                case CELL_GCM_TEXTURE_FORMAT.CELL_GCM_TEXTURE_COMPRESSED_DXT23:
                case CELL_GCM_TEXTURE_FORMAT.CELL_GCM_TEXTURE_COMPRESSED_DXT45:
                    header.FormatFlags = DDSPixelFormatFlags.DDPF_FOURCC;

                    // FourCC
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

                    header.RBitMask = 0x000000FF;  // RBitMask 
                    header.GBitMask = 0x0000FF00;  // GBitMask
                    header.BBitMask = 0x00FF0000;  // BBitMask
                    header.ABitMask = 0xFF000000;  // ABitMask

                    header.DxgiFormat = DDS_DXGI_FORMAT.DXGI_FORMAT_R8G8B8A8_UNORM;
                    break;
            }

            // Unswizzle

            if ((format == CELL_GCM_TEXTURE_FORMAT.CELL_GCM_TEXTURE_A8R8G8B8 || format == CELL_GCM_TEXTURE_FORMAT.CELL_GCM_TEXTURE_D8R8G8B8)
                && !FormatBits.HasFlag(CELL_GCM_TEXTURE_FORMAT.CELL_GCM_TEXTURE_LN))
            {
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

            PGLUCellTextureInfo textureInfo = TextureRenderInfo as PGLUCellTextureInfo;

            // Swap channels for DDS
            if (format == CELL_GCM_TEXTURE_FORMAT.CELL_GCM_TEXTURE_A8R8G8B8 || format == CELL_GCM_TEXTURE_FORMAT.CELL_GCM_TEXTURE_D8R8G8B8)
            {
                var sp = MemoryMarshal.Cast<byte, uint>(imageData);
                for (var i = 0; i < Width * Height * 4; i += 4)
                {
                    // Swap endian first
                    sp[i / 4] = BinaryPrimitives.ReverseEndianness(sp[i / 4]);

                    // Remap channels
                    byte r = imageData[i + (byte)textureInfo.InR];
                    byte g = imageData[i + (byte)textureInfo.InG];
                    byte b = imageData[i + (byte)textureInfo.InB];
                    byte a = imageData[i + (byte)textureInfo.InA];

                    imageData[i + 0] = r;
                    imageData[i + 1] = g;
                    imageData[i + 2] = b;
                    imageData[i + 3] = a;
                }
            }

            header.ImageData = imageData;
            header.Write(outStream);
        }
    }
}