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

namespace PDTools.Files.Textures
{
    public class CellTexture : Texture
    {
        public string Name 
        { 
            get => (TextureRenderInfo as PGLUCellTextureInfo).Name; 
            set => (TextureRenderInfo as PGLUCellTextureInfo).Name = value;
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

        public void InitFromDDSImage(Pfim.IImage image, CELL_GCM_TEXTURE_FORMAT format)
        {
            FormatBits = format;
            Width = (ushort)image.Width;
            Height = (ushort)image.Height;
            LastMipmapLevel = image.MipMaps.Length + 1;

            var textureInfo = TextureRenderInfo as PGLUCellTextureInfo;
            textureInfo.Width = Width;
            textureInfo.Height = Height;
            textureInfo.MipmapLevelLast = (byte)LastMipmapLevel;

             textureInfo.Pitch = (int)Width * 4;

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

            var dds = Pfim.Pfim.FromFile(ddsFileName);
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
            CreateDDSData(ImageData.ToArray(), ms, noConvertFormat: isOutputDds); // Change format for DXT10 if we're doing a direct extract to dds
            ms.Position = 0;

            if (isOutputDds)
            {
                File.WriteAllBytes(outputFileName, ms.ToArray());
            }
            else
            {
                var dds = Pfim.Pfim.FromStream(ms);

                if (dds.Format == Pfim.ImageFormat.Rgb24)
                {
                    var i = Image.LoadPixelData<Bgr24>(dds.Data, dds.Width, dds.Height);
                    i.Save(outputFileName);
                }
                else if (dds.Format == Pfim.ImageFormat.Rgba32)
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

        private void CreateDDSData(byte[] imageData, Stream outStream, bool noConvertFormat = false)
        {
            var bs = new BinaryStream(outStream);
            PGLUCellTextureInfo textureInfo = TextureRenderInfo as PGLUCellTextureInfo;

            // https://gist.github.com/Scobalula/d9474f3fcf3d5a2ca596fceb64e16c98#file-directxtexutil-cs-L355
            bs.WriteString("DDS ", StringCoding.Raw);
            bs.WriteInt32(124);    // dwSize (Struct Size)
            bs.WriteUInt32((uint)(DDSHeaderFlags.TEXTURE)); // dwFlags
            bs.WriteInt32(Height); // dwHeight

            // Dirty fix, some TXS3's in GTHD have 1920 as width, but its actually 2048. Stride is correct, so use it instead.
            bs.WriteInt32(Width);

            CELL_GCM_TEXTURE_FORMAT format = (FormatBits & ~CELL_GCM_TEXTURE_FORMAT.CELL_GCM_TEXTURE_LN);

            switch (format)   // dwPitchOrLinearSize
            {
                case CELL_GCM_TEXTURE_FORMAT.CELL_GCM_TEXTURE_COMPRESSED_DXT1:
                    bs.WriteInt32(Height * Width / 2);
                    break;
                case CELL_GCM_TEXTURE_FORMAT.CELL_GCM_TEXTURE_COMPRESSED_DXT23:
                case CELL_GCM_TEXTURE_FORMAT.CELL_GCM_TEXTURE_COMPRESSED_DXT45:
                    bs.WriteInt32(Height * Width);
                    break;
                default:
                    // 32bpp
                    bs.WriteInt32((Width * 32 + 7) / 8);
                    //bs.WriteInt32(0);
                    break;
            }

            bs.WriteInt32(0);    // Depth

            bs.WriteInt32(LastMipmapLevel);
            bs.WriteBytes(new byte[44]); // reserved
            bs.WriteInt32(32); // DDSPixelFormat Header starts here - Struct Size


            switch (format)
            {
                case CELL_GCM_TEXTURE_FORMAT.CELL_GCM_TEXTURE_COMPRESSED_DXT1:
                case CELL_GCM_TEXTURE_FORMAT.CELL_GCM_TEXTURE_COMPRESSED_DXT23:
                case CELL_GCM_TEXTURE_FORMAT.CELL_GCM_TEXTURE_COMPRESSED_DXT45:
                    bs.WriteUInt32((uint)DDSPixelFormatFlags.DDPF_FOURCC); // Format Flags

                    // FourCC
                    if (format == CELL_GCM_TEXTURE_FORMAT.CELL_GCM_TEXTURE_COMPRESSED_DXT1)
                        bs.WriteString("DXT1", StringCoding.Raw);
                    else if (format == CELL_GCM_TEXTURE_FORMAT.CELL_GCM_TEXTURE_COMPRESSED_DXT23)
                        bs.WriteString("DXT3", StringCoding.Raw);
                    else if (format == CELL_GCM_TEXTURE_FORMAT.CELL_GCM_TEXTURE_COMPRESSED_DXT45)
                        bs.WriteString("DXT5", StringCoding.Raw);

                    bs.WriteInt32(0); // RGBBitCount
                    bs.WriteInt32(0); // RBitMask
                    bs.WriteInt32(0); // GBitMask
                    bs.WriteInt32(0); // BBitMask
                    bs.WriteInt32(0); // ABitMask
                    break;
                case CELL_GCM_TEXTURE_FORMAT.CELL_GCM_TEXTURE_A8R8G8B8:
                    bs.WriteUInt32((uint)(DDSPixelFormatFlags.DDPF_RGB | DDSPixelFormatFlags.DDPF_ALPHAPIXELS | DDSPixelFormatFlags.DDPF_FOURCC));           // Format Flags
                    bs.WriteString("DX10", StringCoding.Raw); // FourCC
                    bs.WriteInt32(32);         // RGBBitCount

                    bs.WriteUInt32(0x0000FF00);  // RBitMask 
                    bs.WriteUInt32(0x00FF0000);  // GBitMask
                    bs.WriteUInt32(0xFF000000);  // BBitMask
                    bs.WriteUInt32(0x000000FF);  // ABitMask
                    break;
            }

            bs.WriteInt32(0x1000); // dwCaps, 0x1000 = required
            bs.WriteBytes(new byte[16]); // dwCaps1-4

            if (format == CELL_GCM_TEXTURE_FORMAT.CELL_GCM_TEXTURE_A8R8G8B8)
            {
                // DDS_HEADER_DXT10
                bs.WriteInt32(87); // DXGI_FORMAT_B8G8R8A8_UNORM
                bs.WriteInt32(3);  // DDS_DIMENSION_TEXTURE2D
                bs.BaseStream.Seek(4, SeekOrigin.Current);  // miscFlag
                bs.WriteInt32(1); // arraySize
                bs.WriteInt32(0); // miscFlags2
            }

            // Unswizzle

            if (!FormatBits.HasFlag(CELL_GCM_TEXTURE_FORMAT.CELL_GCM_TEXTURE_LN))
            {
                int bytesPerPix = 4;
                int byteCount = (Width * Height) * 4;
                byte[] newImageData = new byte[byteCount];

                Syroot.BinaryData.Memory.SpanReader sr = new Syroot.BinaryData.Memory.SpanReader(imageData);
                Syroot.BinaryData.Memory.SpanWriter sw = new Syroot.BinaryData.Memory.SpanWriter(newImageData);

                Span<byte> pixBuffer = new byte[4];
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

            if (!noConvertFormat && format == CELL_GCM_TEXTURE_FORMAT.CELL_GCM_TEXTURE_A8R8G8B8) // B8G8R8A8 to A8R8G8B8
            {
                var pixels = MemoryMarshal.Cast<byte, uint>(imageData.AsSpan());
                for (int j = 0; j < Width * Height; j++)
                    pixels[j] = BinaryPrimitives.ReverseEndianness(pixels[j]);
            }

            bs.Write(imageData);
        }
    }
}