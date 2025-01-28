using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

using System.Buffers;
using Syroot.BinaryData;
using System.Security.Cryptography;

namespace PDTools.GrimSonyDLSBuilder;

public class PaceFileInfo
{
    public const int VersionMajor = 0;
    public const int VersionMinor = 0;

    public PaceFileInfoParams Params { get; set; } = new();

    public List<PaceFileInfoPiece> Pieces { get; set; } = [];

    public static PaceFileInfo FromStream(Stream stream)
    {
        using var bs = new BinaryStream(stream, ByteConverter.Big);

        int baseOffset = (int)bs.Position;
        uint versionFlags = bs.ReadUInt32();
        uint sizeToEndOfFileFromHere = bs.ReadUInt32();

        // Verify md5
        bs.Position = baseOffset;
        byte[] toHash = bs.ReadBytes((int)sizeToEndOfFileFromHere - 0x18);

        byte[] computed = SHA256.HashData(toHash);
        byte[] expected = bs.ReadBytes(0x20);

        if (!computed.AsSpan().SequenceEqual(expected))
            throw new InvalidDataException("SHA256 of file info file did not match.");

        var file = new PaceFileInfo();

        bs.Position = baseOffset + 0x08;
        while (bs.Position < bs.Length)
        {
            uint fieldInfo = bs.ReadUInt32();

            FileInfoFieldType fieldType = (FileInfoFieldType)(fieldInfo >> 16);
            ushort fieldSize = (ushort)(fieldInfo & 0xFFFF);

            if (((ushort)fieldType & 0x8000) != 0)
            {
                int numPieces = bs.ReadInt32();
                for (var i = 0; i < numPieces; i++)
                {
                    var piece = new PaceFileInfoPiece();
                    piece.Hash = bs.ReadBytes(0x20);
                    file.Pieces.Add(piece);
                }
                break;
            }

            switch (fieldType)
            {
                case FileInfoFieldType.Field_Size:
                    file.Params.Size = bs.ReadUInt64();
                    break;

                case FileInfoFieldType.Field_Name:
                    file.Params.Name = bs.ReadString(fieldSize);
                    break;

                case FileInfoFieldType.Field_TrackerUrl:
                    file.Params.TrackerUrl = bs.ReadString(fieldSize);
                    break;
            }

        }

        return file;
    }

    public static PaceFileInfo CreateFromTarget(string targetFile, PaceFileInfoParams @params)
    {
        if (!File.Exists(targetFile))
            throw new FileNotFoundException("Target file not found.");

        using var fs = new FileStream(targetFile, FileMode.Open, FileAccess.Read);
        if (fs.Length == 0)
            throw new FileNotFoundException("File is empty, no data.");

        var file = new PaceFileInfo();
        file.Params = @params;
        file.Params.Size = (ulong)fs.Length;

        const int PIECE_SIZE = 0x80000;

        byte[] buffer = ArrayPool<byte>.Shared.Rent(PIECE_SIZE);

        long rem = fs.Length;
        while (rem > 0)
        {
            int currentPieceSize = (int)Math.Min(PIECE_SIZE, rem);
            fs.ReadExactly(buffer, 0, currentPieceSize);
            
            byte[] pieceHash = SHA256.HashData(buffer.AsSpan(0, currentPieceSize));

            var piece = new PaceFileInfoPiece();
            piece.Hash = pieceHash;
            file.Pieces.Add(piece);

            rem -= currentPieceSize;
        }

        return file;
    }

    public void SaveFileInfo(string outputFile)
    {
        using var fs = new FileStream(outputFile, FileMode.Create, FileAccess.ReadWrite);
        using var bs = new BinaryStream(fs, ByteConverter.Big);

        uint flags = (PaceFileInfo.VersionMajor) << 28 | ((PaceFileInfo.VersionMinor & 0b1111) << 24) | (0 & 0b1111 << 0);
        bs.WriteUInt32(flags);
        bs.WriteUInt32(0); // Skip size

        WriteSizeField(bs, Params.Size);
        WriteNameField(bs, Params.Name);
        WriteTrackerUrlField(bs, Params.TrackerUrl);
        WritePiecesField(bs, Pieces);

        int length = (int)bs.Position;
        bs.Position = 4;
        bs.WriteInt32(length + 0x18);

        bs.Position = 0;
        byte[] toHash = bs.ReadBytes(length);
        bs.WriteBytes(SHA256.HashData(toHash));
    }

    private static void WriteSizeField(BinaryStream bs, ulong size)
    {
        bs.WriteUInt32((ushort)FileInfoFieldType.Field_Size << 16 | sizeof(ulong));
        bs.WriteUInt64(size);
    }

    private static void WriteNameField(BinaryStream bs, string name)
    {
        bs.WriteUInt32((uint)((ushort)FileInfoFieldType.Field_Name << 16 | name.Length));
        bs.WriteString(name, StringCoding.Raw);
    }

    private static void WriteTrackerUrlField(BinaryStream bs, string trackerUrl)
    {
        bs.WriteUInt32((uint)((ushort)FileInfoFieldType.Field_TrackerUrl << 16 | trackerUrl.Length));
        bs.WriteString(trackerUrl, StringCoding.Raw);
    }

    public static void WritePiecesField(BinaryStream bs, List<PaceFileInfoPiece> pieces)
    {
        bs.WriteUInt32((((uint)0x8001 << 16) | 0x20));
        bs.WriteUInt32((uint)pieces.Count);
        for (var i = 0; i < pieces.Count; i++)
            bs.WriteBytes(pieces[i].Hash);
    }


    public enum FileInfoFieldType : ushort
    {
        Field_Size = 1,
        Field_Name = 2,
        Field_TrackerUrl = 3,
        Field_4 = 4,
        Field_4001 = 0x4001,
        Field_4002 = 0x4002,
        Field_4003 = 0x4003,
    }
}
