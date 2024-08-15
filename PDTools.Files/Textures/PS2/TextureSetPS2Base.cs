using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;

using PDTools.Utils;

namespace PDTools.Files.Textures.PS2;

public abstract class TextureSetPS2Base
{
    protected GSMemory _gsMemory = new();
    protected byte[] _inputData;

    public ushort TotalBlockSize { get; set; }
    public List<PGLUtexture> pgluTextures { get; set; } = [];
    public List<GSTransfer> GSTransfers { get; set; } = [];

    protected void InitializeGSMemory()
    {
        // Initialize GS Memory. This will unswizzle each buffer as needed
        // Reminder: GS Memory always stores its data as PSMCT32
        for (int i = 0; i < GSTransfers.Count; i++)
        {
            GSTransfer transfer = GSTransfers[i];
            switch (transfer.Format)
            {
                case SCE_GS_PSM.SCE_GS_PSMCT16:
                    _gsMemory.WriteTexPSMCT16(transfer.BP, transfer.BW, 0, 0, transfer.Width, transfer.Height, MemoryMarshal.Cast<byte, ushort>(transfer.Data));
                    break;

                case SCE_GS_PSM.SCE_GS_PSMCT32:
                case SCE_GS_PSM.SCE_GS_PSMCT24:
                    Span<uint> transferData = MemoryMarshal.Cast<byte, uint>(transfer.Data);
                    _gsMemory.WriteTexPSMCT32(transfer.BP, transfer.BW, 0, 0, transfer.Width, transfer.Height, transferData);
                    break;

                case SCE_GS_PSM.SCE_GS_PSMT4:
                case SCE_GS_PSM.SCE_GS_PSMT4HL: // GT2K uses this (width/height 0?)
                case SCE_GS_PSM.SCE_GS_PSMT4HH: // same
                    _gsMemory.WriteTexPSMT4(transfer.BP, transfer.BW, 0, 0, transfer.Width, transfer.Height, transfer.Data);
                    break;
                case SCE_GS_PSM.SCE_GS_PSMT8:
                    _gsMemory.WriteTexPSMT8(transfer.BP, transfer.BW, 0, 0, transfer.Width, transfer.Height, transfer.Data);
                    break;

                default:
                    throw new NotImplementedException($"InitializeGSMemory: Transfer format {transfer.Format} not yet supported");
            }
        }
    }

    /// <summary>
    /// Gets image data of a texture.
    /// </summary>
    /// <param name="texture"></param>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    /// <exception cref="NotImplementedException"></exception>
    protected Image<Rgba32> GetImageData(PGLUtexture texture, TextureClutPatch textureClutPatch = null)
    {
        if (_gsMemory is null)
            throw new Exception("Not input mode");

        int fullWidth = (int)Math.Pow(2, texture.tex0.TW_TextureWidth);
        int fullHeight = (int)Math.Pow(2, texture.tex0.TH_TextureHeight);

        byte[] textureData;
        uint[] palette = null;

        ushort cbp = textureClutPatch is not null ? textureClutPatch.CBP_ClutBufferBasePointer : texture.tex0.CBP_ClutBlockPointer;
        byte csa = textureClutPatch is not null ? textureClutPatch.CSA_ClutEntryOffset : texture.tex0.CSA_ClutEntryOffset;

        Image<Rgba32> img = new Image<Rgba32>(fullWidth, fullHeight);

        // Grab texture data & palette (if available)
        SCE_GS_PSM paletteFormat = texture.tex0.CPSM_ClutPartPixelFormatSetup;
        switch (texture.tex0.PSM)
        {
            case SCE_GS_PSM.SCE_GS_PSMT4:
            case SCE_GS_PSM.SCE_GS_PSMT4HH: // GT2K UTex, TODO check what that actually does
            case SCE_GS_PSM.SCE_GS_PSMT4HL: // Same
                textureData = new byte[Tex1Utils.GetDataSize(fullWidth, fullHeight, texture.tex0.PSM)];

                _gsMemory.ReadTexPSMT4(texture.tex0.TBP0_TextureBaseAddress, texture.tex0.TBW_TextureBufferWidth,
                    0, 0,
                    fullWidth, fullHeight,
                    textureData);

                palette = new uint[8 * 2];

                switch (paletteFormat)
                {
                    case SCE_GS_PSM.SCE_GS_PSMCT32:
                        _gsMemory.ReadTexPSMCT32(cbp,
                            1,
                            0, 0,
                            8, 2, // Always 8x2 for PSMT4,
                            palette,
                            (csa * 32));
                        break;
                    case SCE_GS_PSM.SCE_GS_PSMCT16: // TODO: this doesn't work properly when csa > 0
                        ushort[] palette16 = new ushort[8 * 2];
                        _gsMemory.ReadTexPSMCT16(cbp,
                            1,
                            0, 0,
                            8, 2, // Always 8x2 for PSMT4
                            palette16,
                            csa * 32);

                        Console.WriteLine("Warning: CSA > 0 not properly supported for PSMCT16 yet");
                        PSMCT16To32(palette, palette16);
                        break;
                    default:
                        throw new NotImplementedException($"Invalid or not supported palette format {texture.tex0.CPSM_ClutPartPixelFormatSetup}");
                }
                break;

            case SCE_GS_PSM.SCE_GS_PSMT8:
                {
                    textureData = new byte[fullWidth * fullHeight];

                    _gsMemory.ReadTexPSMT8(texture.tex0.TBP0_TextureBaseAddress, texture.tex0.TBW_TextureBufferWidth,
                        0, 0,
                        fullWidth, fullHeight,
                        textureData);

                    palette = new uint[16 * 16];

                    switch (paletteFormat)
                    {
                        case SCE_GS_PSM.SCE_GS_PSMCT32:
                            _gsMemory.ReadTexPSMCT32(cbp,
                                1,
                                0, 0,
                                16, 16, // Always 16x16 for PSMT8
                                palette,
                                csa * 32);
                            break;
                        case SCE_GS_PSM.SCE_GS_PSMCT16:
                            ushort[] palette16 = new ushort[16 * 16];

                            _gsMemory.ReadTexPSMCT16(cbp,
                                1,
                                0, 0,
                                8, 2, // Always 16x16 for PSMT8
                                palette16,
                                csa * 32);
                            Console.WriteLine("Warning: CSA > 0 not properly supported for PSMCT16 yet");

                            PSMCT16To32(palette, palette16);
                            break;

                        default:
                            throw new NotImplementedException($"Invalid or not supported palette format {texture.tex0.CPSM_ClutPartPixelFormatSetup}");
                    }
                }
                break;

            case SCE_GS_PSM.SCE_GS_PSMCT32:
            case SCE_GS_PSM.SCE_GS_PSMCT24:
                textureData = new byte[fullWidth * fullHeight * 4];

                _gsMemory.ReadTexPSMCT32(texture.tex0.TBP0_TextureBaseAddress, texture.tex0.TBW_TextureBufferWidth,
                    0, 0,
                    fullWidth, fullHeight,
                    MemoryMarshal.Cast<byte, uint>(textureData));
                break;

            default:
                throw new NotImplementedException($"Not implemented format: {texture.tex0.PSM}");
        }

        // Normalize alpha and detile palette if needed
        switch (texture.tex0.PSM)
        {
            case SCE_GS_PSM.SCE_GS_PSMT4:
            case SCE_GS_PSM.SCE_GS_PSMT4HH: // TODO
            case SCE_GS_PSM.SCE_GS_PSMT4HL: // Same
            case SCE_GS_PSM.SCE_GS_PSMT8:
                {
                    var paletteColors = MemoryMarshal.Cast<uint, Rgba32>(palette);
                    for (int i = 0; i < paletteColors.Length; i++)
                        paletteColors[i].A = (byte)Tex1Utils.Normalize(paletteColors[i].A, 0x00, 0x80, 0x00, 0xFF); // Rescale alpha 0-128 to 0-256. PS2 things

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

                    break;
                }

            case SCE_GS_PSM.SCE_GS_PSMCT32:
            case SCE_GS_PSM.SCE_GS_PSMCT24:
                {
                    Span<Rgba32> pixels = MemoryMarshal.Cast<byte, Rgba32>(textureData);
                    for (var y = 0; y < fullHeight; y++)
                    {
                        for (var x = 0; x < fullWidth; x++)
                        {
                            img[x, y] = pixels[y * fullWidth + x];

                            byte a = texture.tex0.PSM == SCE_GS_PSM.SCE_GS_PSMCT24 ? (byte)0xFF : (byte)Tex1Utils.Normalize(img[x, y].A, 0x00, 0x80, 0x00, 0xFF);
                            img[x, y] = new Rgba32(img[x, y].R, img[x, y].G, img[x, y].B, a); // Rescale alpha 0-128 to 0-256. PS2 things
                        }
                    }

                    break;
                }

            default:
                throw new NotSupportedException($"Format {texture.tex0.PSM} not supported");
        }

        // Crop region with actual texture dimensions
        img.Mutate(e => e.Crop((int)texture.ClampSettings.MAXU + 1, (int)texture.ClampSettings.MAXV + 1));

        return img;
    }

    public void Dump()
    {
        for (var i = 0; i < pgluTextures.Count; i++)
        {
            PGLUtexture texture = pgluTextures[i];

            Console.WriteLine($"- Textures[{i}] - {texture.tex0.PSM} ({texture.ClampSettings.MAXU + 1}x{texture.ClampSettings.MAXV + 1}) - TBP:{texture.tex0.TBP0_TextureBaseAddress:X8} " +
                $"- CBP: {texture.tex0.CBP_ClutBlockPointer:X8}, CSA:{texture.tex0.CSA_ClutEntryOffset}");
        }


        for (var i = 0; i < GSTransfers.Count; i++)
            Console.WriteLine($"- Transfers[{i}] - {GSTransfers[i].Format} ({GSTransfers[i].Width}x{GSTransfers[i].Height}) - Offset:{GSTransfers[i].BP:X8}");

    }

    public abstract Image<Rgba32> GetTextureImage(int index, int varIndex = 0);

    // Credits tiledggd
    protected static Rgba32[] MakeTiledPalette(Span<Rgba32> pal)
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

    protected static void PSMCT16To32(uint[] palette, ushort[] palette16)
    {
        // Page 72, GS User's Manual
        // PSMCT16 stores the higher 5 bits of each color when converting to PSMCT32
        for (int i = 0; i < 16; i++)
        {
            byte r = (byte)(((palette16[i] >> 0) & 0b11111) << 3);
            byte g = (byte)(((palette16[i] >> 5) & 0b11111) << 3);
            byte b = (byte)(((palette16[i] >> 10) & 0b11111) << 3);
            byte a = (palette16[i] >> 15 == 1) ? (byte)0x80 : (byte)0x00;

            palette[i] = (uint)(r | g << 8 | b << 16 | a << 24);
        }
    }
}
