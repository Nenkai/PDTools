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

    /* So. Tex1 might seem like a simple format, but it can get complicated really quick.
     * If you wanna follow along, grab 010 Editor and this template
     * https://github.com/Nenkai/GT-File-Specifications-Documentation/blob/master/Formats/GT4/GT4_Tex1_TexSet.bt
     * 
     * PGLUTextures are just the textures present in the set and the registers 
     * that are passed in to GS, any tbp field (including mipmap) is remapped at runtime. 
     * 
     * The GS Transfers are the hard part. 
     * 
     * But before explaining the transfers, it's important to be familiar with the block/page system,
     * so refer to Page 161<->175 of the GS's Users Manual (Docs&Training\HardwareManuals in PS2 SDK).
     * 
     * The important registers to keep in mind are TBP and CBP (in tex0). These are block pointers/offsets.
     * 
     * Blocks kinda work as a separate coordinate system. For texture sets, some optimization is made as to where the textures go.
     * When you have a texture that's for instance 350x350, the height and width are raised to the next power of 2, so 512x512.
     * That leaves a space with what's rendered and what isn't, so extra data can be put there, it can be the image's palette, or another texture
     * So don't be surprised if you see the CBP register of a texture in the middle of what would appear to be the main texture's.
     * 
     * Now, for GS transfers.
     * 
     * Suppose you have one basic texture with a palette, PDI's builder simply builds two transfers - one with the image data, the other with the palette.
     * Simple enough, right?
     * The problem is when larger textures or more than one texture exists within the set.
     * 
     * They're swizzled into buffers converted from i.e 4bit/8bit to PSMCT32 (32 bit) so the GS can load them faster.
     * To read them (and convert to png), I used GSTextureConvert.
     * https://ps2linux.no-ip.info/playstation2-linux.com/projects/ezswizzle/
     * 
     * You can also use TextureSwizzling.pdf for some notes, along with:
     * - Docs&Training\Starting Guides\Graphics Synthesizer Starting Guide.pdf (PS2 SDK) - Page 
     * - Source in Shell\Tools\shellTexture\
     * - ee\sample\graphics\textrans\bitconv
     * 
     * For an example, look at advertise/us/premium.img (GT4 Online).
     * There's 3 transfers, 64x1216, 32x16 and 8x8.
     * 
     * So summarize, tex1 allows for rather complex 4 optimizations:
     * - Textures, or palettes, can be inside the non-rendered area of other textures, to save on blocks
     * - Multiple texture buffers of different formats swizzled into PSMCT32 for faster upload to GS
     * - Texture data is sometimes reused when a different palette is used, to save on size
     * - When a different palette is used for certain textures, the CSA register is set, which presumably avoids using an extra block for a palette.
     */

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

            short baseTbp = bs.ReadInt16(); // Realistically always 0, only remapped at runtime
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

        public void Dump()
        {
            for (var i = 0; i < pgluTextures.Count; i++)
            {
                //PGLUtexture texture = pgluTextures.OrderByDescending(e => e.ClampSettings.MAXU * e.ClampSettings.MAXV).ToList()[i];
                PGLUtexture texture = pgluTextures[i];

                Console.WriteLine($"- Textures[{i}] - {texture.tex0.PSM} ({texture.ClampSettings.MAXU + 1}x{texture.ClampSettings.MAXV + 1}) - TBP:{texture.tex0.TBP0_TextureBaseAddress:X8} " +
                    $"- CBP: {texture.tex0.CBP_ClutBlockPointer:X8}, CSA:{texture.tex0.CSA_ClutEntryOffset}");
            }


            for (var i = 0; i < GSTransfers.Count; i++)
                Console.WriteLine($"- Transfers[{i}] - {GSTransfers[i].Format} ({GSTransfers[i].Width}x{GSTransfers[i].Height}) - Offset:{GSTransfers[i].BP:X8}");
            
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

            //PGLUtexture texture = pgluTextures.OrderByDescending(e => e.ClampSettings.MAXU * e.ClampSettings.MAXV).ToList()[index];
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
                        0, (int)texture.tex0.CSA_ClutEntryOffset,
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
                        0, (int)texture.tex0.CSA_ClutEntryOffset,
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
                int bpp = Tex1Utils.GetBitsPerPixel(texture.tex0.PSM);
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

        public void Serialize(Stream stream)
        {
            var bs = new BinaryStream(stream, ByteConverter.Little);

            long basePos = bs.Position;

            bs.Position = basePos + 0x30;
            uint pgluTextureOffset = (uint)(bs.Position - basePos);

            for (var i = 0; i < pgluTextures.Count; i++)
                pgluTextures[i].Write(bs);

            uint transferInfoOffset = (uint)(bs.Position - basePos);
            uint dataOffset = (uint)(transferInfoOffset + (GSTransfers.Count * 0x0C));
            dataOffset = MiscUtils.AlignValue(dataOffset, 0x10);
            for (var i = 0; i < GSTransfers.Count; i++)
            {
                GSTransfers transfer = GSTransfers[i];

                bs.Position = basePos + transferInfoOffset + (i * 0x0C);
                transfer.DataOffset = dataOffset;
                transfer.Write(bs);

                bs.Position = basePos + (int)dataOffset;
                bs.Write(transfer.Data);
                dataOffset = (uint)(bs.Position - basePos);
            }

            uint fileSize = (uint)(bs.Position - basePos);

            // Write header
            bs.Position = basePos;
            bs.WriteUInt32(Magic); // Tex1
            bs.WriteUInt32(0); // Reloc ptr
            bs.WriteUInt32(0); // Unk ptr
            bs.WriteUInt32(fileSize); // File size
            bs.WriteUInt16(0); // Base TBP - should be zero
            bs.WriteUInt16(TotalBlockSize); // Total size in blocks
            bs.WriteUInt16((ushort)pgluTextures.Count);
            bs.WriteUInt16((ushort)GSTransfers.Count);
            bs.WriteUInt32(pgluTextureOffset);
            bs.WriteUInt32(transferInfoOffset);
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
    }
}
