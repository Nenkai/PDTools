using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Diagnostics;
using System.Runtime.InteropServices;

using Syroot.BinaryData;

using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;

using PDTools.Utils;
using Microsoft.Toolkit.HighPerformance;

namespace PDTools.Files.Textures.PS2
{
    public class TextureSet1
    {
        /// <summary>
        /// Magic - "Tex1".
        /// </summary>
        public const uint Magic = 0x31786554;

        public ushort TotalBlockSize { get; set; }

        public List<PGLUtexture> pgluTextures { get; set; } = new List<PGLUtexture>();
        public List<GSTransfers> GSTransfers { get; set; } = new List<GSTransfers>();

        private GSMemory _gsMemory = new();

        private byte[] _inputData;

        public void FromStream(Stream stream)
        {
            long basePos = stream.Position;

            var bs = new BinaryStream(stream);
            uint magic = bs.ReadUInt32();
            if (magic != Magic)
                throw new InvalidDataException("Expected Tex1 magic");

            int relocPtr = bs.ReadInt32();
            int empty = bs.ReadInt32();
            int textureSetSize = bs.ReadInt32();

            if (stream.Length - basePos < textureSetSize)
                throw new InvalidDataException("Texture data provided is smaller than texture set specified size.");

            bs.Position = basePos;
            _inputData = bs.ReadBytes(textureSetSize);
            bs.Position = basePos + 0x10;

            short baseTbp = bs.ReadInt16(); // Realistically always 0
            TotalBlockSize = bs.ReadUInt16();
            ushort pgluTextureCount = bs.ReadUInt16();
            ushort textureInfoCount = bs.ReadUInt16();
            uint pgluTextureMapOffset = bs.ReadUInt32();
            uint textureInfosOffset = bs.ReadUInt32();

            bs.Position = basePos + (int)pgluTextureMapOffset;
            for (var i = 0; i < pgluTextureCount; i++)
            {
                PGLUtexture tex = new PGLUtexture();
                tex.Read(bs);
                pgluTextures.Add(tex);
            }

            bs.Position = basePos + (int)textureInfosOffset;
            for (var i = 0; i < textureInfoCount; i++)
            {
                GSTransfers transfer = new GSTransfers();
                transfer.Read(bs);
                GSTransfers.Add(transfer);
            }

            /*
            var genOffset = Tex1Utils.FindBlockIndexAtPosition(GSTransfers[0].Format, GSTransfers[0].Width, GSTransfers[0].Height);
            Console.WriteLine($"Infos[0] - Format: {GSTransfers[0].Format} ({GSTransfers[0].Width}x{GSTransfers[0].Height}) - Size:{TotalBlockSize:X8} - Gen:{genOffset:X8}");
            */

            /*
            for (var i = 0; i < pgluTextures.Count; i++)
                Console.WriteLine($"- Textures[{i}] - {pgluTextures[i].tex0.PSM} ({pgluTextures[i].ClampSettings.MAXU + 1}x{pgluTextures[i].ClampSettings.MAXV + 1}) - Offset:{pgluTextures[i].tex0.TBP0_TextureBaseAddress:X8}");


            for (var i = 0; i < GSTransfers.Count; i++)
                Console.WriteLine($"- Transfers[{i}] - {GSTransfers[i].Format} ({GSTransfers[i].Width}x{GSTransfers[i].Height}) - Offset:{GSTransfers[i].BP:X8}");
            */


            InitializeGSMemory();
        }

        private void InitializeGSMemory()
        {
            // Initialize GS Memory. This will unswizzle each buffer as needed
            // Reminder: GS Memory always stores its data as PSMCT32
            foreach (var transfer in GSTransfers)
            {
                switch (transfer.Format)
                {
                    case SCE_GS_PSM.SCE_GS_PSMCT32:
                        Span<uint> transferData = MemoryMarshal.Cast<byte, uint>(_inputData.AsSpan((int)transfer.DataOffset));
                        _gsMemory.WriteTexPSMCT32(transfer.BP, transfer.BW, 0, 0, transfer.Width, transfer.Height, transferData);
                        break;

                    case SCE_GS_PSM.SCE_GS_PSMT4:
                        Span<byte> transferDataPSMT4 = _inputData.AsSpan((int)transfer.DataOffset);
                        _gsMemory.WriteTexPSMT4(transfer.BP, transfer.BW, 0, 0, transfer.Width, transfer.Height, transferDataPSMT4);
                        break;

                    case SCE_GS_PSM.SCE_GS_PSMT8:
                        Span<byte> transferDataPSMT8 = _inputData.AsSpan((int)transfer.DataOffset);
                        _gsMemory.WriteTexPSMT8(transfer.BP, transfer.BW, 0, 0, transfer.Width, transfer.Height, transferDataPSMT8);
                        break;

                    default:
                        throw new NotImplementedException($"Transfer format {transfer.Format} not yet supported");
                }
            }
        }

        /// <summary>
        /// Gets a texture by index in this texture set.
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public Image<Rgba32> GetTextureImage(int index)
        {
            if (index > pgluTextures.Count)
                throw new IndexOutOfRangeException("Texture index is out of range.");

            PGLUtexture texture = pgluTextures[index];
            return GetImageData(texture);
        }

        /// <summary>
        /// Gets image data of a texture.
        /// </summary>
        /// <param name="texture"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        /// <exception cref="NotImplementedException"></exception>
        private Image<Rgba32> GetImageData(PGLUtexture texture)
        {
            if (_gsMemory is null)
                throw new Exception("Not input mode");

            int fullWidth = (int)Math.Pow(2, texture.tex0.TW_TextureWidth);
            int fullHeight = (int)Math.Pow(2, texture.tex0.TH_TextureHeight);

            byte[] textureData;
            uint[] palette = null;

            Image<Rgba32> img = new Image<Rgba32>(fullWidth, fullHeight);
            switch (texture.tex0.PSM)
            {
                case SCE_GS_PSM.SCE_GS_PSMT4:
                    textureData = new byte[(fullWidth * fullHeight) / 2];

                    _gsMemory.ReadTexPSMT4(texture.tex0.TBP0_TextureBaseAddress, texture.tex0.TBW_TextureBufferWidth,
                        0, 0,
                        fullWidth, fullHeight,
                        textureData);

                    palette = new uint[8 * 2];

                    _gsMemory.ReadTexPSMCT32((int)texture.tex0.CBP_ClutBlockPointer,
                        1,
                        0, 0,
                        8, 2, // Always 8x2 for PSMT4
                        palette);

                    break;

                case SCE_GS_PSM.SCE_GS_PSMT8:
                    textureData = new byte[fullWidth * fullHeight];

                    _gsMemory.ReadTexPSMT8(texture.tex0.TBP0_TextureBaseAddress, texture.tex0.TBW_TextureBufferWidth,
                        0, 0,
                        fullWidth, fullHeight,
                        textureData);

                    palette = new uint[16 * 16];
                    _gsMemory.ReadTexPSMCT32((int)texture.tex0.CBP_ClutBlockPointer,
                        1,
                        0, 0,
                        16, 16, // Always 16x16 for PSMT8
                        palette);
                    break;

                case SCE_GS_PSM.SCE_GS_PSMCT32:
                    textureData = new byte[fullWidth * fullHeight * 4];

                    _gsMemory.ReadTexPSMCT32(texture.tex0.TBP0_TextureBaseAddress, texture.tex0.TBW_TextureBufferWidth,
                        0, 0,
                        fullWidth, fullHeight,
                        MemoryMarshal.Cast<byte, uint>(textureData));
                    break;

                default:
                    throw new NotImplementedException($"Not implemented format: {texture.tex0.PSM}");
            }

            // Formats with palettes
            if (texture.tex0.PSM == SCE_GS_PSM.SCE_GS_PSMT4 || texture.tex0.PSM == SCE_GS_PSM.SCE_GS_PSMT8)
            {
                var paletteColors = MemoryMarshal.Cast<uint, Rgba32>(palette);
                for (int i = 0; i < paletteColors.Length; i++)
                    paletteColors[i].A = (byte)Math.Clamp(paletteColors[i].A * 2, 0x00, 0xFF); // Rescale alpha 0-128 to 0-256. PS2 things

                if (texture.tex0.PSM == SCE_GS_PSM.SCE_GS_PSMT8)
                    paletteColors = MakeTiledPalette(paletteColors);

                BitStream bs = new BitStream(BitStreamMode.Read, textureData, BitStreamSignificantBitOrder.MSB);
                int bpp = GetBitsPerPixel(texture.tex0.PSM);
                for (var y = 0; y < fullHeight; y++)
                {
                    for (var x = 0; x < fullWidth; x++)
                    {
                        int idx = (int)bs.ReadBits(bpp);
                        img[x, y] = paletteColors[idx];
                    }
                }
            }
            else if (texture.tex0.PSM == SCE_GS_PSM.SCE_GS_PSMCT32)
            {
                Span<Rgba32> pixels = MemoryMarshal.Cast<byte, Rgba32>(textureData);
                for (var y = 0; y < fullHeight; y++)
                {
                    for (var x = 0; x < fullWidth; x++)
                    {
                        img[x, y] = pixels[(y * fullWidth) + x];
                    }
                }
            }

            // Crop region with actual texture dimensions
            img.Mutate(e => e.Crop((int)texture.ClampSettings.MAXU + 1, (int)texture.ClampSettings.MAXV + 1));

            return img;
        }

        public void Serialize(BinaryStream bs)
        {
            long basePos = bs.Position;

            bs.Position = basePos + 0x30;
            int pgluTextureOffset = (int)(bs.Position - basePos);

            for (var i = 0; i < pgluTextures.Count; i++)
                pgluTextures[i].Write(bs);

            int textureInfoOffset = (int)(bs.Position - basePos);
            uint dataOffset = (uint)(textureInfoOffset + (GSTransfers.Count * 0x0C));
            dataOffset = MiscUtils.AlignValue(dataOffset, 0x10);
            for (var i = 0; i < GSTransfers.Count; i++)
            {
                GSTransfers info = GSTransfers[i];

                bs.Position = textureInfoOffset + (i * 0x0C);
                info.DataOffset = dataOffset;
                info.Write(bs);

                bs.Position = (int)dataOffset;

                if (info.Format == SCE_GS_PSM.SCE_GS_PSMT8 || info.Format == SCE_GS_PSM.SCE_GS_PSMT4)
                {
                    var img = info.IndexedImage;
                    ulong bpp = (ulong)GetBitsPerPixel(info.Format);
                    for (var y = 0; y < img.Height; y++)
                    {
                        var row = img.DangerousGetRowSpan(y);
                        for (var x = 0; x < img.Width; x++)
                        {
                            //bs.WriteBits(row[x], bpp);
                        }
                    }

                    bs.Align(0x10);
                }
                else
                {
                    if (info.IsPalette)
                    {
                        Debug.Assert(info.Format == SCE_GS_PSM.SCE_GS_PSMCT32, "Palette is not RGBA32");

                        for (var j = 0; j < info.Palette.Length; j++)
                        {
                            var col = info.TiledPalette[j];
                            bs.WriteByte(col.R);
                            bs.WriteByte(col.G);
                            bs.WriteByte(col.B);
                            bs.WriteByte((byte)Math.Clamp(col.A / 2, (byte)0x00, (byte)0x80));
                        }
                    }
                    else
                    {
                        Image<Rgba32> image = info.Image;
                        for (var y = 0; y < image.Height; y++)
                        {
                            for (var x = 0; x < image.Width; x++)
                            {
                                bs.WriteByte(image[x, y].R);
                                bs.WriteByte(image[x, y].G);
                                bs.WriteByte(image[x, y].B);
                                bs.WriteByte((byte)Math.Round(Range(image[x, y].A, 0x00, 0x80), MidpointRounding.AwayFromZero));
                            }
                        }
                    }
                }

                dataOffset = (uint)bs.Position;
            }

            // Write header
            bs.Position = 0;
            bs.WriteUInt32(Magic); // Tex1
            bs.WriteInt32(0);
            bs.WriteInt32(0);
            bs.WriteInt32(0);
            bs.WriteInt16(0); // Base TBP - should be zero
            bs.WriteUInt16(TotalBlockSize); // Total size in blocks
            bs.WriteInt16((short)pgluTextures.Count);
            bs.WriteInt16((short)GSTransfers.Count);
            bs.WriteInt32(pgluTextureOffset);
            bs.WriteInt32(textureInfoOffset);
        }

        // Credits tiledggd
        private static Rgba32[] MakeTiledPalette(Span<Rgba32> pal)
        {
            const int tileSizeW = 8;
            const int tileSizeH = 2;

            Rgba32[] outpal = new Rgba32[256];
            int ntx = 16 / tileSizeW,
                nty = 16 / tileSizeH;
            int i = 0;

            for (int ty = 0; ty < nty; ty++)
                for (int tx = 0; tx < ntx; tx++)
                    for (int y = 0; y < tileSizeH; y++)
                        for (int x = 0; x < tileSizeW; x++)
                            outpal[(ty * tileSizeH + y) * 16 + (tx * tileSizeW + x)] = pal[i++];

            return outpal;
        }

        static int GetBitsPerPixel(SCE_GS_PSM psm)
        {
            return psm switch
            {
                SCE_GS_PSM.SCE_GS_PSMCT32 => 32,
                SCE_GS_PSM.SCE_GS_PSMCT24 => 32, // RGB24, uses 24-bit per pixel with the upper 8 bit unused.
                SCE_GS_PSM.SCE_GS_PSMCT16 => 16, // RGBA16 unsigned, pack two pixels in 32-bit in little endian order.
                SCE_GS_PSM.SCE_GS_PSMCT16S => 16, // RGBA16 signed, pack two pixels in 32-bit in little endian order.
                SCE_GS_PSM.SCE_GS_PSMT8 => 8, // 8-bit indexed, packing 4 pixels per 32-bit.
                SCE_GS_PSM.SCE_GS_PSMT4 => 4, // 4-bit indexed, packing 8 pixels per 32-bit.
                SCE_GS_PSM.SCE_GS_PSMT8H => 4, // 8-bit indexed, but the upper 24-bit are unused.
                SCE_GS_PSM.SCE_GS_PSMT4HL => 4, // 4-bit indexed, but the upper 24-bit are unused.
                SCE_GS_PSM.SCE_GS_PSMT4HH => 4,
                SCE_GS_PSM.SCE_GS_PSMZ32 => 32,  // 32-bit Z buffer
                SCE_GS_PSM.SCE_GS_PSMZ24 => 32, // 24-bit Z buffer with the upper 8-bit unused
                SCE_GS_PSM.SCE_GS_PSMZ16 => 16, // 16-bit unsigned Z buffer, pack two pixels in 32-bit in little endian order.
                SCE_GS_PSM.SCE_GS_PSMZ16S => 16, // 16-bit signed Z buffer, pack two pixels in 32-bit in little endian order.
                _ => throw new InvalidOperationException($"Invalid pixel surface type '{psm}'")
            };
        }
        public double Range(double val, double min, double max)
            => min + val * (max - min);
    }
}
