using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Buffers.Binary;

using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.Formats;

using Syroot.BinaryData.Core;
using Syroot.BinaryData;

using PDTools.Files.Textures.PS3;
using PDTools.Files.Textures.PS4;
using PDTools.Files.Textures.PSP;
using SixLabors.Fonts;

namespace PDTools.Files.Textures;

public class TextureSet3
{
    public const string MAGIC = "TXS3";
    public const string MAGIC_LE = "3SXT";

    public List<TextureSet3Buffer> Buffers { get; set; } = [];
    public List<PGLUTextureInfo> TextureInfos { get; set; } = [];
    public List<TextureSet3ClutInfoBase> ClutInfos { get; set; } = [];

    public bool WriteNames { get; set; }

    public bool LittleEndian { get; set; }

    /// <summary>
    /// The on-disk container this texture set was read from: "TXS3" (PS3), "3SXT" (PSP) or
    /// "Tpp1" (PSP compact). Used to tag dumped filenames so the original container + pixel
    /// format can be reproduced on rebuild (neither is recoverable from a PNG).
    /// </summary>
    public string ContainerTag { get; set; }

    public long DataPointer { get; set; }

    /// <summary>
    /// Original relocation pointer
    /// May be slightly offset (0x200) if the texture set is in a CourseData/PAC due to header
    /// </summary>
    public uint RelocPtr { get; set; }

    public long BaseTextureSetPosition { get; set; }

    public TextureSet3()
    {

    }

    public void FromStream(Stream stream, TextureConsoleType consoleType)
    {
        BaseTextureSetPosition = stream.Position;

        BinaryStream bs = new BinaryStream(stream);
        string magic = bs.ReadString(4);
        if (magic == "TXS3")
            bs.ByteConverter = ByteConverter.Big;
        else if (magic == "3SXT")
            bs.ByteConverter = ByteConverter.Little;
        else
            throw new InvalidDataException("Could not parse TXS3 from stream, not a valid TXS3 image file.");

        ContainerTag = magic;

        int fileSize = bs.ReadInt32();

        if (consoleType == TextureConsoleType.PS4) // 64 bit
        {
            ReadPS4Header(bs);
        }
        else if (consoleType == TextureConsoleType.PS3)
        {
            ReadPS3Header(bs);
        }
        else if (consoleType == TextureConsoleType.PSP)
        {
            ReadPSPHeader(bs);
        }
        else
            throw new NotSupportedException($"Console type {consoleType} not supported");
    }

    private void ReadPSPHeader(BinaryStream bs)
    {
        // Total Header size is 0x40

        RelocPtr = bs.ReadUInt32(); // Original Position, if bundled
        bs.Position += 4;
        bs.Position += 4; // Sometimes 1

        short pgluTexturesCount = bs.ReadInt16();
        short bufferInfoCount = bs.ReadInt16();
        int pgluTexturesOffset = bs.ReadInt32();
        int bufferInfosOffset = bs.ReadInt32();
        uint relocSize = bs.ReadUInt32();
        ushort unkCount_0x24 = bs.ReadUInt16();
        ushort clutMapEntryCount = bs.ReadUInt16();
        uint unkOffset_0x28 = bs.ReadUInt32();
        uint clutMapOffset = bs.ReadUInt32();
        uint unkOffset_0x30 = bs.ReadUInt32();
        ushort unkCount_0x34 = bs.ReadUInt16();
        bs.ReadUInt16();
        uint unkOffset_0x38 = bs.ReadUInt32();

        if (bufferInfoCount > 0)
        {
            for (int i = 0; i < bufferInfoCount; i++)
            {
                bs.Position = BaseTextureSetPosition + (bufferInfosOffset - RelocPtr) + (i * 0x20);

                TextureSet3Buffer bufferInfo = new GETextureBuffer();
                bufferInfo.Read(bs);
                Buffers.Add(bufferInfo);

                bs.Position = BaseTextureSetPosition + (bufferInfo.ImageOffset - RelocPtr);
                bufferInfo.ImageData = new byte[bufferInfo.ImageSize];
                bs.ReadExactly(bufferInfo.ImageData.Span);
            }
        }

        if (clutMapEntryCount > 0)
        {
            for (int i = 0; i < clutMapEntryCount; i++)
            {
                bs.Position = BaseTextureSetPosition + (clutMapOffset - RelocPtr) + (i * 0x0C);

                GEClutBufferInfo clutInfo = new GEClutBufferInfo();
                clutInfo.Read(bs);
                ClutInfos.Add(clutInfo);

                bs.Position = BaseTextureSetPosition + (clutInfo.ClutBufferOffset - RelocPtr);
                clutInfo.ClutData = new byte[GEUtils.BitsPerPixel((eSCE_GE_TPF)clutInfo.ClutType) / 8 * clutInfo.NumColors];
                bs.ReadExactly(clutInfo.ClutData);
            }
        }

        if (pgluTexturesCount > 0)
        {
            for (int i = 0; i < pgluTexturesCount; i++)
            {
                bs.Position = BaseTextureSetPosition + (pgluTexturesOffset - RelocPtr) + (i * 0x98);

                PGLUGETextureInfo textureInfo = new PGLUGETextureInfo();
                textureInfo.Read(bs, BaseTextureSetPosition);
                TextureInfos.Add(textureInfo);

                textureInfo.BufferInfo = (GETextureBuffer)Buffers[(int)textureInfo.BufferId];

                if (textureInfo.ClutMapEntryIndex != -1)
                    textureInfo.ClutBufferInfo = (GEClutBufferInfo)ClutInfos[textureInfo.ClutMapEntryIndex];
            }
        }
    }

    private void ReadPS3Header(BinaryStream bs)
    {
        RelocPtr = bs.ReadUInt32(); // Original Position, if bundled
        bs.Position += 4;
        bs.Position += 4; // Sometimes 1

        // TODO: Implement proper image count reading - right now we only care about the real present images
        short pgluTexturesCount = bs.ReadInt16();
        short bufferInfoCount = bs.ReadInt16();
        int pgluTexturesOffset = bs.ReadInt32();
        int bufferInfosOffset = bs.ReadInt32();
        DataPointer = bs.ReadUInt32();

        if (bufferInfoCount > 0)
        {
            for (int i = 0; i < bufferInfoCount; i++)
            {
                bs.Position = BaseTextureSetPosition + (bufferInfosOffset - RelocPtr) + (i * 0x20);

                TextureSet3Buffer bufferInfo = new CellTextureBuffer();
                bufferInfo.Read(bs);
                Buffers.Add(bufferInfo);
            }
        }

        if (pgluTexturesCount > 0)
        {
            for (int i = 0; i < pgluTexturesCount; i++)
            {
                bs.Position = BaseTextureSetPosition + (pgluTexturesOffset - RelocPtr) + (i * 0x44);

                TextureSet3Buffer texture = Buffers[i];

                PGLUTextureInfo textureInfo = new PGLUCellTextureInfo();
                textureInfo.Read(bs, BaseTextureSetPosition);
                TextureInfos.Add(textureInfo);

                // PS3 texture infos are parallel to buffer infos (texture i <-> buffer i); the
                // image data is read into Buffers[i] below, so link the same buffer. (The PS3 Read
                // doesn't populate BufferId, so the old Buffers[BufferId] pointed every texture at
                // buffer 0 - in a multi-texture set every texture dumped as the first.)
                textureInfo.BufferId = (uint)i;
                textureInfo.BufferInfo = texture;

                bs.Position = BaseTextureSetPosition + texture.ImageOffset;
                texture.ImageData = new byte[texture.ImageSize];
                bs.ReadExactly(texture.ImageData.Span);
            }
        }
    }

    private void ReadPS4Header(BinaryStream bs)
    {
        long relocPtr = bs.ReadInt64();
        long relocPtr2 = bs.ReadInt64();
        int unk = bs.ReadInt32(); // Unknown, 2

        short pgluTexturesCount = bs.ReadInt16();
        short bufferInfoCount = bs.ReadInt16();
        long pgluTexturesOffset = bs.ReadInt64();
        long bufferInfosOffset = bs.ReadInt64();
        DataPointer = bs.ReadInt64();

        // TODO
        bs.ReadInt64();
        bs.ReadInt64();
        bs.ReadInt64();
        bs.ReadInt16();
        bs.Position += 14;
        bs.ReadInt64();
        bs.Position += 8;

        if (bufferInfoCount > 0)
        {
            for (int i = 0; i < bufferInfoCount; i++)
            {
                bs.Position = BaseTextureSetPosition + (bufferInfosOffset - RelocPtr) + (i * 0x30);

                TextureSet3Buffer bufferInfo = new OrbisTextureBuffer();
                bufferInfo.Read(bs);
                Buffers.Add(bufferInfo);

                bs.Position = BaseTextureSetPosition + bufferInfo.ImageOffset;
                bufferInfo.ImageData = new byte[bufferInfo.ImageSize];
                bs.ReadExactly(bufferInfo.ImageData.Span);
            }
        }

        if (pgluTexturesCount > 0)
        {
            for (int i = 0; i < pgluTexturesCount; i++)
            {
                bs.Position = BaseTextureSetPosition + (pgluTexturesOffset - RelocPtr) + (i * 0x48);

                PGLUTextureInfo textureInfo = new PGLUOrbisTextureInfo();
                textureInfo.Read(bs, BaseTextureSetPosition);
                TextureInfos.Add(textureInfo);
                textureInfo.BufferInfo = Buffers[(int)textureInfo.BufferId];
            }
        }
    }

    public void BuildTextureSetFile(string outputName)
    {
        using var ms = new FileStream(outputName, FileMode.Create);
        using var bs = new BinaryStream(ms, ByteConverter.Big);

        if (LittleEndian)
            bs.ByteConverter = ByteConverter.Little;

        WriteToStream(bs);
    }

    /// <summary>
    /// Writes a PSP (3SXT) texture set: single-texture (8888 / IDTEX4 / IDTEX8 / DXT1 / DXT5) or, when
    /// the set holds more than one texture, a multi-texture container (non-paletted 8888 / DXT only).
    /// </summary>
    public void BuildPSPTextureSetFile(string outputName)
    {
        using var ms = new FileStream(outputName, FileMode.Create);
        using var bs = new BinaryStream(ms, ByteConverter.Little);
        if (TextureInfos.Count > 1)
            BuildPSPMultiTextureSet(bs);
        else
            BuildPSPTextureSet(bs);
    }

    /// <summary>
    /// Writes a multi-texture PSP (3SXT) "texture set" file (e.g. env2.txs = 2x DXT1). Layout matches
    /// PD: header, N texture infos, N GE command lists, N buffer infos, N names, then the image data
    /// region (first image aligned to 0x80, the rest packed 0x10-aligned). Non-paletted only for now
    /// (8888 / DXT1 / DXT5) - no multi-texture IDTEX/CLUT sets exist in GT PSP.
    /// </summary>
    private void BuildPSPMultiTextureSet(BinaryStream bs)
    {
        int n = TextureInfos.Count;
        var texs = new PGLUGETextureInfo[n];
        var bufs = new GETextureBuffer[n];
        var fmts = new eSCE_GE_TPF[n];
        for (int i = 0; i < n; i++)
        {
            texs[i] = (PGLUGETextureInfo)TextureInfos[i];
            bufs[i] = (GETextureBuffer)texs[i].BufferInfo;
            fmts[i] = bufs[i].FormatBits;
            if (fmts[i] is eSCE_GE_TPF.SCE_GE_TPF_IDTEX4 or eSCE_GE_TPF.SCE_GE_TPF_IDTEX8)
                throw new NotSupportedException("Multi-texture PSP sets with paletted (IDTEX) textures aren't supported yet (only 8888 / DXT).");
            if (fmts[i] is not (eSCE_GE_TPF.SCE_GE_TPF_8888 or eSCE_GE_TPF.SCE_GE_TPF_DXT1 or eSCE_GE_TPF.SCE_GE_TPF_DXT5))
                throw new NotSupportedException($"Unsupported multi-texture PSP format {fmts[i]}.");
        }

        const int texOff = 0x40, texInfoSize = 0x98, bufInfoSize = 0x20;
        const int cmdCount = 17;                  // non-paletted command list length
        int cmdLen = cmdCount * 4;
        int cmdStart = texOff + (n * texInfoSize);
        int bufStart = cmdStart + (n * cmdLen);
        int nameStart = bufStart + (n * bufInfoSize);

        // Names (concatenated, null-terminated), before the image data.
        var nameBytes = new byte[n][];
        var nameOff = new int[n];
        int pos = nameStart;
        for (int i = 0; i < n; i++)
        {
            nameOff[i] = pos;
            nameBytes[i] = Encoding.ASCII.GetBytes(texs[i].Name ?? string.Empty);
            pos += nameBytes[i].Length + 1;
        }

        // Image data region: first image 0x80-aligned (PD's relocSize), the rest 0x10-aligned.
        var imgOff = new int[n];
        int cur = (pos + 0x7F) & ~0x7F;
        for (int i = 0; i < n; i++)
        {
            if (i > 0)
                cur = (cur + 0xF) & ~0xF;
            imgOff[i] = cur;
            cur += bufs[i].ImageData.Length;
        }
        int relocSize = imgOff[0];
        int fileSize = (cur + 0x7F) & ~0x7F;   // 128-aligned (PD; GPB packs textures on a 128 grid)

        // ── Header (0x40) ──
        bs.WriteString("3SXT", StringCoding.Raw);
        bs.WriteInt32(fileSize);
        bs.WriteUInt32(0);          // relocation pointer (0 = standalone)
        bs.WriteInt32(0);
        bs.WriteInt32(0);
        bs.WriteInt16((short)n);    // texture info count
        bs.WriteInt16((short)n);    // buffer info count
        bs.WriteInt32(texOff);
        bs.WriteInt32(bufStart);
        bs.WriteInt32(relocSize);   // @0x20 = start of image data region
        bs.WriteInt16(0);
        bs.WriteInt16(0);           // clut map entry count (none)
        bs.WriteInt32(0);
        bs.WriteInt32(0);           // clut map offset (none)
        bs.WriteInt32(0);
        bs.WriteInt32(0);
        bs.WriteInt32(0);
        bs.WriteInt32(0);

        // ── Texture infos (N x 0x98) ──
        for (int i = 0; i < n; i++)
        {
            var buffer = bufs[i];
            eSCE_GE_TPF format = fmts[i];
            int width = buffer.Width, height = buffer.Height;
            int mipWidth = texs[i].MipInfos[0].Width;
            int texWidth = (int)BitOperations.RoundUpToPowerOf2((uint)width); // pow2(W), not stride (see BuildPSPTextureSet)
            int paddedHeight = (int)BitOperations.RoundUpToPowerOf2((uint)height);
            int log2W = BitOperations.Log2((uint)texWidth);
            int log2H = BitOperations.Log2((uint)paddedHeight);
            float uScale = (float)width / texWidth;
            float vScale = (float)height / paddedHeight;
            uint tmodeReg = 0xc2000000u | (uint)texs[i].TMODE.hsm;
            uint tpfReg = 0xc3000100u | (byte)format;

            bs.WriteInt32(cmdStart + (i * cmdLen)); // subParamsOff
            bs.WriteInt32(0);
            bs.WriteSingle(uScale);
            bs.WriteSingle(vScale);
            bs.WriteUInt32(0x80000000);
            bs.WriteUInt32(0x80000000);
            bs.WriteUInt32(0xc0000100);
            bs.WriteUInt32(0xc1000000);
            bs.WriteUInt32(tmodeReg);
            bs.WriteUInt32(tpfReg);
            bs.WriteUInt32(0xc4000000); // CLOAD (no clut)
            bs.WriteUInt32(0xc500ff00); // CLUT (no clut)
            bs.WriteUInt32(0xc6000101);
            bs.WriteUInt32(0xc7000101);
            bs.WriteUInt32(0xc8000000);
            bs.WriteUInt32(0xc9008100);
            bs.WriteUInt32(0xca000000);
            bs.WriteInt32(0);
            bs.WriteUInt32(0);
            bs.WriteUInt16((ushort)mipWidth);
            bs.WriteByte((byte)log2W);
            bs.WriteByte((byte)log2H);
            for (int m = 1; m < 8; m++) { bs.WriteUInt32(0); bs.WriteUInt32(0); }
            bs.WriteInt16(0);
            bs.WriteInt16(-1);          // clut map entry index (-1 = none)
            bs.WriteInt16(0);
            bs.WriteUInt16((ushort)i);  // buffer id
            bs.WriteUInt32(0);
            bs.WriteInt32(nameOff[i]);
        }

        // ── GE command lists (N x 17 u32) ──
        for (int i = 0; i < n; i++)
        {
            eSCE_GE_TPF format = fmts[i];
            int width = bufs[i].Width, height = bufs[i].Height;
            int mipWidth = texs[i].MipInfos[0].Width;
            int texWidth = (int)BitOperations.RoundUpToPowerOf2((uint)width); // pow2(W), not stride (see BuildPSPTextureSet)
            int paddedHeight = (int)BitOperations.RoundUpToPowerOf2((uint)height);
            int log2W = BitOperations.Log2((uint)texWidth);
            int log2H = BitOperations.Log2((uint)paddedHeight);
            float uScale = (float)width / texWidth;
            float vScale = (float)height / paddedHeight;
            uint suRaw = 0x48000000u | (BitConverter.SingleToUInt32Bits(uScale) >> 8);
            uint svRaw = 0x49000000u | (BitConverter.SingleToUInt32Bits(vScale) >> 8);
            uint tmodeReg = 0xc2000000u | (uint)texs[i].TMODE.hsm;
            uint tpfReg = 0xc3000100u | (byte)format;

            bs.WriteUInt32(0xa0000000);                                     // TBP0
            bs.WriteUInt32(0xa8000000u | (uint)mipWidth);                  // TBW0
            bs.WriteUInt32(0xb8000000u | ((uint)log2H << 8) | (uint)log2W); // TSIZE0
            bs.WriteUInt32(suRaw);
            bs.WriteUInt32(svRaw);
            bs.WriteUInt32(0x4a800000);
            bs.WriteUInt32(0x4b800000);
            bs.WriteUInt32(0xc0000100);
            bs.WriteUInt32(tmodeReg);
            bs.WriteUInt32(tpfReg);
            bs.WriteUInt32(0xc6000101);
            bs.WriteUInt32(0xc7000101);
            bs.WriteUInt32(0xc8000000);
            bs.WriteUInt32(0xc9008100);
            bs.WriteUInt32(0xca000000);
            bs.WriteUInt32(0xcb000000); // TFLUSH
            bs.WriteUInt32(0x0b000000); // RET
        }

        // ── Buffer infos (N x 0x20) ──
        for (int i = 0; i < n; i++)
        {
            bool isDxt = fmts[i] is eSCE_GE_TPF.SCE_GE_TPF_DXT1 or eSCE_GE_TPF.SCE_GE_TPF_DXT3 or eSCE_GE_TPF.SCE_GE_TPF_DXT5;
            bs.WriteInt32(imgOff[i]);
            bs.WriteInt32(bufs[i].ImageData.Length);
            bs.WriteByte((byte)(isDxt ? 0 : 8));
            bs.WriteByte((byte)fmts[i]);
            bs.WriteByte(1);            // mip count
            bs.WriteByte(0);           // (env2 multi-texture buffers use 0 here, vs 1 for single)
            bs.WriteUInt16((ushort)bufs[i].Width);
            bs.WriteUInt16((ushort)bufs[i].Height);
            bs.WriteUInt16(1);
            bs.WriteUInt16(0);
            bs.WriteInt32(0);
            bs.WriteInt32(0);
            bs.WriteInt32(0);   // full buffer-info stride is 0x20 (multi-texture infos are contiguous)
        }

        // ── Names ──
        while (bs.Position < nameStart)
            bs.WriteByte(0);
        for (int i = 0; i < n; i++)
            bs.WriteString(texs[i].Name ?? string.Empty, StringCoding.ZeroTerminated);

        // ── Image data ──
        for (int i = 0; i < n; i++)
        {
            while (bs.Position < imgOff[i])
                bs.WriteByte(0);
            bs.Write(bufs[i].ImageData.Span);
        }
        while (bs.Position < fileSize)
            bs.WriteByte(0);
    }

    private void BuildPSPTextureSet(BinaryStream bs)
    {
        if (TextureInfos.Count != 1)
            throw new NotSupportedException("BuildPSPTextureSet is the single-texture path; multi-texture sets are built by BuildPSPMultiTextureSet.");

        var tex = (PGLUGETextureInfo)TextureInfos[0];
        var buffer = (GETextureBuffer)tex.BufferInfo;
        eSCE_GE_TPF format = buffer.FormatBits;

        bool paletted = format is eSCE_GE_TPF.SCE_GE_TPF_IDTEX4 or eSCE_GE_TPF.SCE_GE_TPF_IDTEX8;
        bool isDxt = format is eSCE_GE_TPF.SCE_GE_TPF_DXT1 or eSCE_GE_TPF.SCE_GE_TPF_DXT3 or eSCE_GE_TPF.SCE_GE_TPF_DXT5;
        bool supportedDxt = format is eSCE_GE_TPF.SCE_GE_TPF_DXT1 or eSCE_GE_TPF.SCE_GE_TPF_DXT5;
        if (format != eSCE_GE_TPF.SCE_GE_TPF_8888 && !paletted && !supportedDxt)
            throw new NotSupportedException($"PSP building supports 8888 / IDTEX4 / IDTEX8 / DXT1 / DXT5 (got {format}).");

        GEClutBufferInfo clut = tex.ClutBufferInfo;
        if (paletted && (clut is null || clut.ClutData is null))
            throw new InvalidDataException("Paletted PSP texture is missing its CLUT.");

        int width = buffer.Width;
        int height = buffer.Height;
        // The Create* helpers set MipInfos[0].Width to the buffer's row stride in texels (the 16-byte
        // minimum can exceed the texture width for narrow IDTEX). TSIZE0/UMIN use the texture's own
        // pow2 DIMENSION, not the stride - PD sets TSIZE0 = pow2(W) and TBW0 = stride separately;
        // using the stride for TSIZE0 mis-sizes small IDTEX (e.g. 8px -> reported 32px) -> invisible.
        int mipWidth = tex.MipInfos[0].Width;
        int texWidth = (int)BitOperations.RoundUpToPowerOf2((uint)width);
        int paddedHeight = (int)BitOperations.RoundUpToPowerOf2((uint)height);
        int log2W = BitOperations.Log2((uint)texWidth);
        int log2H = BitOperations.Log2((uint)paddedHeight);
        byte[] image = buffer.ImageData.ToArray();

        const int texOff = 0x40;
        const int texInfoSize = 0x98;
        int subParamsOff = texOff + texInfoSize;
        int cmdCount = paletted ? 21 : 17; // paletted adds CBP, CBW, CLUT, CLOAD
        int bufOff = subParamsOff + (cmdCount * 4);
        const int bufInfoSize = 0x20;
        int imgOff = (bufOff + bufInfoSize + 0xF) & ~0xF;

        byte[] clutData = paletted ? clut.ClutData : [];
        int numColors = paletted ? clut.NumColors : 0;

        int clutInfoOff = 0, clutDataOff = 0, nameOff;
        if (paletted)
        {
            clutInfoOff = imgOff + image.Length;
            clutDataOff = (clutInfoOff + 0x0C + 0xF) & ~0xF;
            nameOff = clutDataOff + clutData.Length;
        }
        else
        {
            nameOff = imgOff + image.Length;
        }

        byte[] name = Encoding.ASCII.GetBytes(tex.Name ?? string.Empty);
        // 3SXT fileSize is padded to 128 (every real PD texture is; the GPB packs each texture on a
        // 128-byte grid). A 64-aligned size (e.g. tip_up at 0x240) leaves the texture non-128 in the
        // GPB + trailing 0x5E, which breaks the game's loader for THIS and all following textures.
        int fileSize = (nameOff + name.Length + 1 + 0x7F) & ~0x7F;

        float uScale = (float)width / texWidth;
        float vScale = (float)height / paddedHeight;
        uint suRaw = 0x48000000u | (BitConverter.SingleToUInt32Bits(uScale) >> 8);
        uint svRaw = 0x49000000u | (BitConverter.SingleToUInt32Bits(vScale) >> 8);

        uint tmodeReg = 0xc2000000u | (uint)tex.TMODE.hsm;   // TMODE: high-speed (swizzled) bit
        uint tpfReg = 0xc3000100u | (byte)format;            // TPF: 8888=3, IDTEX4=4, IDTEX8=5, DXT1=8
        uint cloadReg = 0xc4000000u | (uint)((numColors + 7) / 8); // CLOAD: 8-entry blocks, rounded up (PD uses the exact colour count)
        uint clutReg = paletted ? 0xc500ff03u : 0xc500ff00u; // CLUT: cpf C8888 when paletted

        // ── Header (0x40) ──
        bs.WriteString("3SXT", StringCoding.Raw);
        bs.WriteInt32(fileSize);
        bs.WriteUInt32(0);          // relocation pointer (0 = standalone)
        bs.WriteInt32(0);
        bs.WriteInt32(0);
        bs.WriteInt16(1);           // texture info count
        bs.WriteInt16(1);           // buffer info count
        bs.WriteInt32(texOff);
        bs.WriteInt32(bufOff);
        bs.WriteInt32(fileSize);    // relocation size
        bs.WriteInt16(0);
        bs.WriteInt16((short)(paletted ? 1 : 0)); // clut entry count
        bs.WriteInt32(0);
        bs.WriteInt32(paletted ? clutInfoOff : 0); // clut map offset
        bs.WriteInt32(0);
        bs.WriteInt32(0);
        bs.WriteInt32(0);
        bs.WriteInt32(0);

        // ── Texture info (0x98) ──
        bs.WriteInt32(subParamsOff);
        bs.WriteInt32(0);
        bs.WriteSingle(uScale);     // UMIN (used width / buffer width)
        bs.WriteSingle(vScale);     // VMIN (used height / buffer height)
        bs.WriteUInt32(0x80000000); // UMAX
        bs.WriteUInt32(0x80000000); // VMAX
        bs.WriteUInt32(0xc0000100); // TMAP
        bs.WriteUInt32(0xc1000000); // TSHADE
        bs.WriteUInt32(tmodeReg);   // TMODE
        bs.WriteUInt32(tpfReg);     // TPF
        bs.WriteUInt32(cloadReg);   // CLOAD
        bs.WriteUInt32(clutReg);    // CLUT
        bs.WriteUInt32(0xc6000101); // FILTER
        bs.WriteUInt32(0xc7000101); // TWRAP
        bs.WriteUInt32(0xc8000000); // LEVEL
        bs.WriteUInt32(0xc9008100); // TFUNC
        bs.WriteUInt32(0xca000000); // TEC
        bs.WriteInt32(0);           // runtime clut offset
        bs.WriteUInt32(0);          // mip0 relocation pointer
        bs.WriteUInt16((ushort)mipWidth);
        bs.WriteByte((byte)log2W);
        bs.WriteByte((byte)log2H);
        for (int i = 1; i < 8; i++) { bs.WriteUInt32(0); bs.WriteUInt32(0); } // unused mips
        bs.WriteInt16(0);
        bs.WriteInt16((short)(paletted ? 0 : -1)); // clut map entry index
        bs.WriteInt16(0);
        bs.WriteUInt16(0);          // buffer id
        bs.WriteUInt32(0);
        bs.WriteInt32(nameOff);

        // ── GE command list (subParams) ──
        if (paletted)
        {
            bs.WriteUInt32(0xb0000000); // CBP - clut address (relocated at runtime)
            bs.WriteUInt32(0xb1000000); // CBW - clut address upper bits
        }
        bs.WriteUInt32(0xa0000000);                                     // TBP0 - texture address (relocated at runtime)
        bs.WriteUInt32(0xa8000000u | (uint)mipWidth);                  // TBW0 - buffer width
        bs.WriteUInt32(0xb8000000u | ((uint)log2H << 8) | (uint)log2W); // TSIZE0
        bs.WriteUInt32(suRaw);      // SU
        bs.WriteUInt32(svRaw);      // SV
        bs.WriteUInt32(0x4a800000); // TU
        bs.WriteUInt32(0x4b800000); // TV
        bs.WriteUInt32(0xc0000100); // TMAP
        bs.WriteUInt32(tmodeReg);   // TMODE
        bs.WriteUInt32(tpfReg);     // TPF
        bs.WriteUInt32(0xc6000101); // TFILTER
        bs.WriteUInt32(0xc7000101); // TWRAP
        bs.WriteUInt32(0xc8000000); // TLEVEL
        bs.WriteUInt32(0xc9008100); // TFUNC
        bs.WriteUInt32(0xca000000); // TEC
        if (paletted)
        {
            bs.WriteUInt32(clutReg);   // CLUT
            bs.WriteUInt32(cloadReg);  // CLOAD
        }
        bs.WriteUInt32(0xcb000000); // TFLUSH
        bs.WriteUInt32(0x0b000000); // RET

        // ── Buffer info (0x20) ──
        bs.WriteInt32(imgOff);
        bs.WriteInt32(image.Length);
        // byte8 = the swizzle flag: 8 when high-speed/swizzled, 0 when linear (narrow IDTEX) or DXT.
        // (The old `isDxt ? 0 : 8` wrongly marked LINEAR IDTEX as swizzled.)
        bs.WriteByte((byte)(tex.TMODE.hsm == SCE_GE_TMODE_HSM.SCE_GE_TMODE_HSM_HIGHSPEED ? 8 : 0));
        bs.WriteByte((byte)format);
        bs.WriteByte(1);            // mip count
        bs.WriteByte(1);
        bs.WriteUInt16((ushort)width);
        bs.WriteUInt16((ushort)height);
        bs.WriteUInt16(1);
        bs.WriteUInt16(0);
        bs.WriteInt32(0);
        bs.WriteInt32(0);

        // ── Image data ──
        while (bs.Position < imgOff)
            bs.WriteByte(0);
        bs.Write(image);

        // ── CLUT (info block + palette data) ──
        if (paletted)
        {
            while (bs.Position < clutInfoOff)
                bs.WriteByte(0);
            bs.WriteByte(0);
            bs.WriteByte((byte)clut.ClutType);
            bs.WriteUInt16((ushort)numColors);
            bs.WriteInt32(clutDataOff);
            bs.WriteInt32(0);

            while (bs.Position < clutDataOff)
                bs.WriteByte(0);
            bs.Write(clutData);
        }

        // ── Texture name ──
        while (bs.Position < nameOff)
            bs.WriteByte(0);
        bs.WriteString(tex.Name ?? string.Empty, StringCoding.ZeroTerminated);
        while (bs.Position < fileSize)
            bs.WriteByte(0);
    }

    /// <summary>
    /// Writes a single-texture Tpp1 PSP container (8888 / IDTEX4 / IDTEX8). The texture's image
    /// data must already be encoded at Tpp1's 16-byte-aligned stride (see PGLUGETextureInfo.CreateTpp1).
    /// </summary>
    public void BuildTpp1File(string outputName)
    {
        using var ms = new FileStream(outputName, FileMode.Create);
        using var bs = new BinaryStream(ms, ByteConverter.Little);
        BuildTpp1Stream(bs);
    }

    private void BuildTpp1Stream(BinaryStream bs)
    {
        if (TextureInfos.Count != 1)
            throw new NotSupportedException("Tpp1 building supports exactly one texture.");

        var tex = (PGLUGETextureInfo)TextureInfos[0];
        var buffer = (GETextureBuffer)tex.BufferInfo;
        eSCE_GE_TPF format = buffer.FormatBits;

        bool paletted = format is eSCE_GE_TPF.SCE_GE_TPF_IDTEX4 or eSCE_GE_TPF.SCE_GE_TPF_IDTEX8;
        if (format != eSCE_GE_TPF.SCE_GE_TPF_8888 && !paletted)
            throw new NotSupportedException($"Tpp1 building supports 8888 / IDTEX4 / IDTEX8 (got {format}).");

        GEClutBufferInfo clut = tex.ClutBufferInfo;
        if (paletted && (clut is null || clut.ClutData is null))
            throw new InvalidDataException("Paletted Tpp1 texture is missing its CLUT.");

        int width = buffer.Width;
        int height = buffer.Height;
        int stride = tex.MipInfos[0].Width; // row stride in texels (16-byte aligned)
        int log2W = BitOperations.Log2(BitOperations.RoundUpToPowerOf2((uint)width));
        int log2H = BitOperations.Log2(BitOperations.RoundUpToPowerOf2((uint)height));
        byte[] image = buffer.ImageData.ToArray();
        byte[] clutData = paletted ? clut.ClutData : [];
        int numColors = paletted ? clut.NumColors : 0;

        const int entryTableOffset = 0x30;
        const int subParamsOffset = 0x38;
        int cmdListLen = (paletted ? 10 : 6) * 4; // 0x28 / 0x18
        const int imageOffset = 0x80;              // command list is always padded up to 0x80
        int clutOffset = imageOffset + image.Length;
        int dataBytes = image.Length + clutData.Length;
        int field10 = ((dataBytes + 0x1FFF) / 0x2000) << 16; // ceil(dataBytes / 0x2000) << 16
        int fileSize = clutOffset + clutData.Length;

        uint tmodeReg = 0xC2000000u | (uint)tex.TMODE.hsm;

        // ── Header (0x30) ──
        bs.WriteString("Tpp1", StringCoding.Raw);
        bs.WriteInt32(0);
        bs.WriteInt32(0);
        bs.WriteInt32(cmdListLen);     // 0x0C: command list length
        bs.WriteInt32(field10);        // 0x10: ceil(data / 0x2000) << 16
        bs.WriteInt32(1);              // 0x14: texture count
        bs.WriteInt32(entryTableOffset); // 0x18
        bs.WriteInt32(0);              // 0x1C
        while (bs.Position < entryTableOffset)
            bs.WriteByte(0);

        // ── Texture entry (0x30) ──
        bs.WriteInt32(subParamsOffset);
        bs.WriteUInt16((ushort)width);
        bs.WriteUInt16((ushort)height);

        // ── GE command list (0x38) ──
        bs.WriteUInt32(0xC3000000u | (byte)format);                     // TPF
        bs.WriteUInt32(tmodeReg);                                       // TMODE
        bs.WriteUInt32(0xB8000000u | ((uint)log2H << 8) | (uint)log2W); // TSIZE0
        bs.WriteUInt32(0xA0000000u | (uint)imageOffset);                // TBP0
        bs.WriteUInt32(0xA8000000u | (uint)stride);                     // TBW0
        if (paletted)
        {
            bs.WriteUInt32(0xB0000000u | (uint)clutOffset);             // CBP
            bs.WriteUInt32(0xB1000000u);                                // CBW
            bs.WriteUInt32(0xC500FF03u);                                // CLUT (C8888)
            bs.WriteUInt32(0xC4000000u | (uint)((numColors + 7) / 8));  // CLOAD (8-entry blocks, rounded up)
        }
        bs.WriteUInt32(0x0B000000u);                                    // RET

        // ── Image data + CLUT ──
        while (bs.Position < imageOffset)
            bs.WriteByte(0);
        bs.Write(image);
        if (paletted)
            bs.Write(clutData);
        while (bs.Position < fileSize)
            bs.WriteByte(0);
    }

    /// <summary>
    /// Reads a Tpp1 PSP texture. Tpp1 is a compact single-texture container used by GT PSP; the
    /// payload is the same PSP GE texture data as a 3SXT texture set, just with a leaner header
    /// (a GE command list whose TBP0/TBW0/CBP/CLOAD point at the image and CLUT data).
    /// </summary>
    public void FromTpp1Stream(Stream stream)
    {
        long basePos = stream.Position;
        BaseTextureSetPosition = basePos;
        LittleEndian = true;

        var bs = new BinaryStream(stream, ByteConverter.Little);
        string magic = bs.ReadString(4);
        if (magic != "Tpp1")
            throw new InvalidDataException("Could not parse Tpp1 from stream, not a valid Tpp1 texture.");

        ContainerTag = "Tpp1";

        long fileLength = stream.Length - basePos;

        bs.Position = basePos + 0x14;
        int textureCount = bs.ReadInt32();
        int entryTableOffset = bs.ReadInt32();

        for (int i = 0; i < textureCount; i++)
        {
            bs.Position = basePos + entryTableOffset + (i * 8);
            int subParamsOffset = bs.ReadInt32();
            ushort width = bs.ReadUInt16();
            ushort height = bs.ReadUInt16();

            // Parse the GE command list (raw little-endian u32s: command = high byte, arg = low 24 bits).
            bs.Position = basePos + subParamsOffset;
            eSCE_GE_TPF format = eSCE_GE_TPF.SCE_GE_TPF_8888;
            var hsm = SCE_GE_TMODE_HSM.SCE_GE_TMODE_HSM_NORMAL;
            var clutFormat = SCE_GE_CLUT_CPF.SCE_GE_CLUT_CPF_8888;
            int imageOffset = 0, stride = 0, clutOffset = 0, clutBlocks = 0;
            bool reachedEnd = false;
            while (!reachedEnd)
            {
                uint value = bs.ReadUInt32();
                int command = (int)(value >> 24);
                int arg = (int)(value & 0xFFFFFF);
                switch (command)
                {
                    case 0xC3: format = (eSCE_GE_TPF)(arg & 0xFF); break;        // TPF (pixel format)
                    case 0xC2: hsm = (SCE_GE_TMODE_HSM)(arg & 1); break;         // TMODE (swizzle flag)
                    case 0xA0: imageOffset = arg; break;                         // TBP0 (image data offset)
                    case 0xA8: stride = arg; break;                             // TBW0 (buffer width in texels)
                    case 0xB0: clutOffset = arg; break;                          // CBP (CLUT data offset)
                    case 0xC4: clutBlocks = arg & 0xFF; break;                   // CLOAD (CLUT block count)
                    case 0xC5: clutFormat = (SCE_GE_CLUT_CPF)(arg & 0x3); break; // CLUT (CLUT pixel format)
                    case 0x0B: reachedEnd = true; break;                         // RET
                }
            }

            bool paletted = format is eSCE_GE_TPF.SCE_GE_TPF_IDTEX4 or eSCE_GE_TPF.SCE_GE_TPF_IDTEX8;
            int numColors = clutBlocks * 8;
            int clutBytesPerColor = clutFormat == SCE_GE_CLUT_CPF.SCE_GE_CLUT_CPF_8888 ? 4 : 2;

            int imageEnd = (paletted && clutOffset != 0) ? clutOffset : (int)fileLength;
            int imageSize = imageEnd - imageOffset;

            var texture = new PGLUGETextureInfo();
            texture.TPF.tpf = format;
            texture.TMODE.hsm = hsm;
            texture.MipInfos[0] = new GEMipInfo { Width = (ushort)stride };

            bs.Position = basePos + imageOffset;
            byte[] imageData = bs.ReadBytes(imageSize);

            var buffer = new GETextureBuffer
            {
                Width = width,
                Height = height,
                FormatBits = format,
                LastMipmapLevel = 1,
                ImageData = imageData,
                ImageSize = (uint)imageSize,
            };
            texture.BufferInfo = buffer;

            if (paletted && clutOffset != 0)
            {
                bs.Position = basePos + clutOffset;
                byte[] clutData = bs.ReadBytes(numColors * clutBytesPerColor);
                texture.ClutBufferInfo = new GEClutBufferInfo
                {
                    ClutType = clutFormat,
                    NumColors = (ushort)numColors,
                    ClutData = clutData,
                };
            }

            texture.BufferId = (uint)Buffers.Count;
            TextureInfos.Add(texture);
            Buffers.Add(buffer);
        }
    }

    /// <summary>
    /// Dumps every texture in the set to an editable image beside <paramref name="outputName"/>,
    /// tagging the filename with the container + pixel format (e.g. "cobra_67.3SXT.IDTEX8.png") so a
    /// faithful rebuild is possible. Multi-texture sets dump into a "&lt;name&gt;.txs/" folder.
    /// </summary>
    /// <param name="outputName">Reference path; the dump name/extension are derived from it.</param>
    /// <param name="asDds">When true, PS3 (Cell) textures are written as real .dds (lossless for DXT,
    /// the inverse of <see cref="PGLUCellTextureInfo.GetDDS"/>); PSP 3SXT/Tpp1 have no DDS equivalent
    /// and always dump as PNG.</param>
    public void ConvertToStandardFormat(string outputName, bool asDds = false)
    {
        Console.WriteLine($"Processing {outputName} with {TextureInfos.Count} texture(s)...");

        for (int i = 0; i < TextureInfos.Count; i++)
        {
            PGLUTextureInfo texture = TextureInfos[i];

            // DDS output only applies to PS3 (Cell) textures - PSP 3SXT/Tpp1 have no DDS equivalent,
            // so they always dump as PNG even when DDS is requested.
            bool useDds = asDds && texture is PGLUCellTextureInfo;
            string ext = useDds ? ".dds" : ".png";

            // Single-texture sets derive the base name from the OUTPUT path (i.e. the input file)
            // so the flagged dump name is predictable for round-tripping. Multi-texture sets fall
            // back to the embedded name / index to disambiguate.
            string baseName = TextureInfos.Count == 1 ? Path.GetFileNameWithoutExtension(outputName) :
                !string.IsNullOrEmpty(texture.Name) ? Path.GetFileNameWithoutExtension(texture.Name) :
                $"{i}";

            // Tag the file with the container + pixel format so a faithful rebuild is possible
            // (e.g. "cobra_67.3SXT.IDTEX8.png"). Neither is recoverable from the image itself.
            string flag = string.IsNullOrEmpty(ContainerTag) ? "" : $".{ContainerTag}.{texture.GetPixelFormatName()}";
            string fileName = $"{baseName}{flag}{ext}";

            // Multi-texture sets dump into a "<name>.txs/" folder (tagged so the rebuild knows it's a
            // multi-texture container, and so the config _path points at the set, not a single image).
            string texturePath = TextureInfos.Count > 1
                ? Path.Combine(Path.GetDirectoryName(outputName), Path.GetFileNameWithoutExtension(outputName) + ".txs", fileName)
                : Path.Combine(Path.GetDirectoryName(outputName), fileName);

            Console.WriteLine($"- Converting '{texturePath}'...");

            Directory.CreateDirectory(Path.GetDirectoryName(texturePath));
            if (useDds)
            {
                // Write the texture as a real .dds (lossless for DXT - no BC re-encode on rebuild).
                File.WriteAllBytes(texturePath, ((PGLUCellTextureInfo)texture).GetDDS());
            }
            else
            {
                using var img = texture.GetAsImage();
                img.Save(texturePath);
            }
        }
    }

    public void AddTexture(PGLUTextureInfo texture)
    {
        ArgumentNullException.ThrowIfNull(texture, nameof(texture));
        ArgumentNullException.ThrowIfNull(texture.BufferInfo, nameof(texture.BufferInfo));

        TextureInfos.Add(texture);
        Buffers.Add(texture.BufferInfo);

        texture.BufferId = (uint)Buffers.Count - 1;
        
    }

    /// <summary>
    /// Writes the texture set to a stream.
    /// </summary>
    /// <param name="bs">Stream to write to.</param>
    /// <param name="txsBasePos">Base position for the texture set</param>
    /// <param name="writeImageData">Whether to write the image data. If not, writing the image data and relinking offsets/finishing up the TXS3 header should be done at your own discretion.</param>
    public void WriteToStream(BinaryStream bs, int txsBasePos = 0, bool writeImageData = true)
    {
        BaseTextureSetPosition = txsBasePos;

        BuildPS3TextureSet(bs, txsBasePos, writeImageData);
    }

    private void BuildPS3TextureSet(BinaryStream bs, int txsBasePos, bool writeImageData = true)
    {
        if (!LittleEndian)
            bs.WriteString(MAGIC, StringCoding.Raw);
        else
            bs.WriteString(MAGIC_LE);

        bs.Position = txsBasePos + 0x14;
        bs.WriteInt16((short)Buffers.Count); // Image Params Count;
        bs.WriteInt16((short)TextureInfos.Count); // Image Info Count;
        bs.WriteInt32(txsBasePos + 0x40); // PGLTexture Offset (render params)

        int imageInfoOffset = txsBasePos + 0x40 + (0x44 * TextureInfos.Count);
        bs.WriteInt32(imageInfoOffset);

        // DataPointer (@0x20): 0 in every real PD gpb2 texture. The standalone reader keys off the
        // buffer's ImageOffset directly, so this is metadata - but match PD (was 0x100).
        bs.WriteInt32(0);

        // Write textures's render params
        bs.Position = txsBasePos + 0x40;
        foreach (var textureInfo in TextureInfos)
            textureInfo.Write(bs);

        // Skip the texture info for now
        bs.Position = imageInfoOffset + (Buffers.Count * 0x20);

        int mainHeaderSize = (int)bs.Position;

        // Write texture names
        int lastNamePos = (int)bs.Position;
        for (int i = 0; i < TextureInfos.Count; i++)
        {
            PGLUTextureInfo texture = TextureInfos[i];
            if (WriteNames && !string.IsNullOrEmpty(texture.Name))
            {
                bs.WriteString(texture.Name, StringCoding.ZeroTerminated);

                // Update name offset
                bs.Position = txsBasePos + 0x40 + (i * 0x44);
                bs.Position += 0x40; // Skip to name offset field
                bs.WriteInt32(lastNamePos);
            }

            bs.Position = lastNamePos;
        }

        int endPos = (int)bs.Position;
        if (writeImageData)
        {
            bs.Align(0x80, grow: true);
            endPos = (int)bs.Position;
        }

        // Actually write the textures now and their linked information
        int lastImageEnd = (int)bs.Position;
        for (int i = 0; i < TextureInfos.Count; i++)
        {
            int imageOffset = 0, endImageOffset = 0;
            if (writeImageData)
            {
                imageOffset = (int)bs.Position;
                bs.Write(Buffers[i].ImageData.Span);
                endImageOffset = (int)bs.Position;
                lastImageEnd = endImageOffset;
            }

            bs.Position = txsBasePos + imageInfoOffset + (i * 0x20);

            var textureInfo = TextureInfos[i] as PGLUCellTextureInfo;

            // Match PD's buffer-info constants (the bytes around the format/mip). Originals use:
            //   @+8 = 0 for uncompressed (A8R8G8B8/D8R8G8B8), 2 for compressed (DXT1/23/45)
            //   @+10 = mip level COUNT (1 for single-mip)   @+11 = 2
            // The old (2, fmt, mipLast=0, 1) layout differs from every real PD texture.
            CELL_GCM_TEXTURE_FORMAT baseFmt = textureInfo.FormatBits & ~CELL_GCM_TEXTURE_FORMAT.CELL_GCM_TEXTURE_LN;
            bool compressed = baseFmt == CELL_GCM_TEXTURE_FORMAT.CELL_GCM_TEXTURE_COMPRESSED_DXT1
                           || baseFmt == CELL_GCM_TEXTURE_FORMAT.CELL_GCM_TEXTURE_COMPRESSED_DXT23
                           || baseFmt == CELL_GCM_TEXTURE_FORMAT.CELL_GCM_TEXTURE_COMPRESSED_DXT45;

            bs.WriteInt32(imageOffset);
            bs.WriteInt32(endImageOffset - imageOffset); // Size
            bs.WriteByte((byte)(compressed ? 2 : 0));
            bs.WriteByte((byte)textureInfo.FormatBits);
            bs.WriteByte((byte)(textureInfo.MipmapLevelLast));
            bs.WriteByte(2);
            bs.WriteUInt16(textureInfo.Width);
            bs.WriteUInt16(textureInfo.Height);
            bs.WriteUInt16(1);
            bs.WriteUInt16(0);
            bs.Position += 12; // Pad
        }

        // PD appends a 3-byte zero trailer after the last image, so fileSize = imageEnd + 3 and
        // the field at @0xC = imageEnd + 2 (NOT the small main-header size). Reproduce that exactly
        // for standalone files - every real PD gpb2 texture has this and a wrong @0xC value is a
        // likely loader crash.
        if (writeImageData && txsBasePos == 0)
        {
            bs.Position = lastImageEnd;
            bs.WriteByte(0);
            bs.WriteByte(0);
            bs.WriteByte(0);
        }

        // Finish up main header
        bs.Position = txsBasePos + 4;
        if (txsBasePos != 0)
        {
            bs.WriteInt32(0);
            bs.WriteInt32(txsBasePos);
            bs.WriteInt32(0);
        }
        else
        {
            bs.WriteInt32((int)bs.Length);
            bs.WriteInt32(0);
            bs.WriteInt32(writeImageData ? lastImageEnd + 2 : mainHeaderSize);
        }

        bs.Position = endPos;
    }

    public byte[] GetExternalImageDataOfTexture(Stream stream, PGLUTextureInfo texture, long basePos = 0)
    {
        stream.Position = basePos + (texture.BufferInfo.ImageOffset - DataPointer);

        var bytes = stream.ReadBytes((int)texture.BufferInfo.ImageSize);
        var ms = new MemoryStream();
        (texture as PGLUCellTextureInfo).CreateDDSData(bytes, ms);

        return ms.ToArray();
    }

    public void FromFile(string file, TextureConsoleType consoleType = TextureConsoleType.PS3)
    {
        using var fs = new FileStream(file, FileMode.Open);
        FromStream(fs, consoleType);
    }


    public enum TextureConsoleType
    {
        PSP,
        PS3,
        PS4,
    };
}