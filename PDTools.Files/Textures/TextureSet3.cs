using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Linq;
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
                textureInfo.BufferInfo = Buffers[(int)textureInfo.BufferId];

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
    /// Format depends on extension. png, jpg...
    /// </summary>
    /// <param name="outputName"></param>
    public void ConvertToStandardFormat(string outputName)
    {
        Console.WriteLine($"Processing {outputName} with {TextureInfos.Count} texture(s)...");

        for (int i = 0; i < TextureInfos.Count; i++)
        {
            PGLUTextureInfo texture = TextureInfos[i];
            string texturePath = outputName;

            string actualName = !string.IsNullOrEmpty(texture.Name) ? texture.Name : 
                TextureInfos.Count > 1 ? $"{i}.png" :
                $"{Path.GetFileNameWithoutExtension(outputName)}.png";

            if (TextureInfos.Count > 1)
                texturePath = Path.Combine(Path.GetDirectoryName(outputName), Path.GetFileNameWithoutExtension(outputName), actualName);
            else
                texturePath = Path.Combine(Path.GetDirectoryName(texturePath), actualName);

            Console.WriteLine($"- Converting '{texturePath}'...");

            using var img = texture.GetAsImage();

            texturePath = Path.ChangeExtension(texturePath, ".png"); // Incase texture.Name doesn't have an extension, or we are outputting to dds.

            Directory.CreateDirectory(Path.GetDirectoryName(texturePath));
            img.Save(texturePath);
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

        // Unk offset, Set to header end
        // FIXME: This might not work for images in models or course packs
        bs.WriteInt32(0x100);

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
        for (int i = 0; i < TextureInfos.Count; i++)
        {
            int imageOffset = 0, endImageOffset = 0;
            if (writeImageData)
            {
                imageOffset = (int)bs.Position;
                bs.Write(Buffers[i].ImageData.Span);
                endImageOffset = (int)bs.Position;
            }

            bs.Position = txsBasePos + imageInfoOffset + (i * 0x20);

            var textureInfo = TextureInfos[i] as PGLUCellTextureInfo;
            bs.WriteInt32(imageOffset);
            bs.WriteInt32(endImageOffset - imageOffset); // Size
            bs.WriteByte(2);
            bs.WriteByte((byte)textureInfo.FormatBits);
            bs.WriteByte((byte)(textureInfo.MipmapLevelLast));
            bs.WriteByte(1);
            bs.WriteUInt16(textureInfo.Width);
            bs.WriteUInt16(textureInfo.Height);
            bs.WriteUInt16(1);
            bs.WriteUInt16(0);
            bs.Position += 12; // Pad
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
            bs.WriteInt32(mainHeaderSize);
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