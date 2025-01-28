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

namespace PDTools.Files.Textures;

public class TextureSet3
{
    public const string MAGIC = "TXS3";
    public const string MAGIC_LE = "3SXT";

    public List<Texture> Textures { get; set; } = [];
    public List<PGLUTextureInfo> TextureInfos { get; set; } = [];

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
        for (int i = 0; i < Textures.Count; i++)
        {
            Texture texture = Textures[i];
            texture.ConvertTextureToStandardFormat(outputName);
        }
    }

    public void AddTexture(Texture texture)
    {
        Textures.Add(texture);

        if (texture is CellTexture)
        {
            var pglu = texture.TextureRenderInfo as PGLUCellTextureInfo;
            pglu.ImageId = (uint)Textures.Count - 1;
        }
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
        bs.WriteInt16((short)Textures.Count); // Image Params Count;
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
        bs.Position = imageInfoOffset + (Textures.Count * 0x20);

        int mainHeaderSize = (int)bs.Position;

        // Write texture names
        int lastNamePos = (int)bs.Position;
        for (int i = 0; i < Textures.Count; i++)
        {
            Texture texture = Textures[i];
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
        for (int i = 0; i < Textures.Count; i++)
        {
            int imageOffset = 0, endImageOffset = 0;
            if (writeImageData)
            {
                imageOffset = (int)bs.Position;
                bs.Write(Textures[i].ImageData.Span);
                endImageOffset = (int)bs.Position;
            }

            bs.Position = txsBasePos + imageInfoOffset + (i * 0x20);

            var textureInfo = Textures[i].TextureRenderInfo as PGLUCellTextureInfo;
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

        short imageInfoCount, pgluTexturesCount;
        long pgluTexturesOffset, imageInfoOffset;

        if (consoleType == TextureConsoleType.PS4) // 64 bit
        {
            long relocPtr = bs.ReadInt64();
            long relocPtr2 = bs.ReadInt64();
            int unk = bs.ReadInt32(); // Unknown, 2

            pgluTexturesCount = bs.ReadInt16();
            imageInfoCount = bs.ReadInt16();
            pgluTexturesOffset = bs.ReadInt64();
            imageInfoOffset = bs.ReadInt64();
            DataPointer = bs.ReadInt64();

            bs.ReadInt64();
            bs.ReadInt64();
            bs.ReadInt64();
            bs.ReadInt16();
            bs.Position += 14;
            bs.ReadInt64();
            bs.Position += 8;
        }
        else
        {
            RelocPtr = bs.ReadUInt32(); // Original Position, if bundled
            bs.Position += 4;
            bs.Position += 4; // Sometimes 1

            // TODO: Implement proper image count reading - right now we only care about the real present images
            pgluTexturesCount = bs.ReadInt16();
            imageInfoCount = bs.ReadInt16();
            pgluTexturesOffset = bs.ReadInt32();
            imageInfoOffset = bs.ReadInt32();
            DataPointer = bs.ReadUInt32();
        }

        if (imageInfoCount > 0)
        {
            for (int i = 0; i < imageInfoCount; i++)
            {
                bs.Position = BaseTextureSetPosition + (imageInfoOffset - RelocPtr) + (i * 0x20);

                Texture texture = consoleType switch
                {
                    TextureConsoleType.PS3 => new CellTexture(),
                    TextureConsoleType.PS4 => new OrbisTexture(),
                    _ => throw new NotImplementedException("Unimplemented console texture type")
                };

                texture.ReadTextureDetails(bs);

                Textures.Add(texture);
            }
        }

        if (pgluTexturesCount > 0)
        {
            for (int i = 0; i < pgluTexturesCount; i++)
            {
                bs.Position = BaseTextureSetPosition + (pgluTexturesOffset - RelocPtr) + (i * 0x44);

                Texture texture = Textures[i];

                PGLUTextureInfo textureInfo = consoleType switch
                {
                    TextureConsoleType.PS3 => new PGLUCellTextureInfo(),
                    TextureConsoleType.PS4 => new PGLUOrbisTextureInfo(),
                    _ => throw new NotImplementedException("Unimplemented console texture type")
                };

                textureInfo.Read(bs);
                TextureInfos.Add(textureInfo);

                bs.Position = BaseTextureSetPosition + texture.ImageOffset;
                texture.ImageData = new byte[texture.ImageSize];
                bs.ReadExactly(texture.ImageData.Span);

                /*
                bs.Position += 0x40;
                int imageNameOffset = bs.ReadInt32();
                if (imageNameOffset != 0)
                {
                    bs.Position = imageNameOffset;
                    texture.Name = bs.ReadString(StringCoding.ZeroTerminated);
                }
                */
            }
        }
    }

    public byte[] GetExternalImageDataOfTexture(Stream stream, Texture texture, long basePos = 0)
    {
        stream.Position = basePos + (texture.ImageOffset - DataPointer);

        var bytes = stream.ReadBytes((int)texture.ImageSize);
        var ms = new MemoryStream();
        (texture as CellTexture).CreateDDSData(bytes, ms);

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