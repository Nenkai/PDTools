using System.Buffers.Binary;
using System.Runtime.InteropServices;

using PDTools.Crypto;
using PDTools.Compression;

namespace PDTools.ScapesDataHelper;

public class ScapesExifDataExtractor
{
    private static readonly byte[] JxlDataEncKey = [
        0x5F, 0xA1, 0xB3, 0x5E, 0xD9, 0x38, 0x1F, 0xAE,
        0x66, 0xAD, 0x9A, 0x72, 0x06, 0x5D, 0xC4, 0xCC,
        0x6E, 0x61, 0xBE, 0xEF, 0x6D, 0x6F, 0x72, 0x69,
        0x34, 0xD3, 0xB6, 0xD4, 0xDD, 0x94, 0x50, 0x06
    ];

    private static readonly byte[] JxlHeaderEncKey = [
        0x06, 0xDD, 0x0D, 0xED, 0x2D, 0xA7, 0x5B, 0x15,
        0xE3, 0x67, 0x6B, 0xD4, 0xEE, 0xC8, 0x04, 0xB0,
        0xBE, 0xAD, 0xDE, 0x9D, 0xEB, 0xA5, 0xA2, 0x75,
        0x16, 0x08, 0xAA, 0xD8, 0xC3, 0x7B, 0x8E, 0xE4
    ];

    public static byte[]? ExtractExifData(Stream stream)
    {
        var jxlBoxMap = JxlBoxInfoMap.FromStream(stream);
        if (!jxlBoxMap.Boxes.TryGetValue(0x65786966, out (long Offset, uint Size) Exif)) // 'exif'
            return null;

        stream.Position = Exif.Offset;

        ExifInfo info = ExifInfo.FromStream(stream);
        if (!info.TryGetProperty((ushort)ExifTag.ExifOffset, out ExifProperty actualExif))
            return null;

        var subIfd = (ImageFileDirectory)actualExif.Value;
        if (subIfd is null)
            return null;

        if (!subIfd.Properties.TryGetValue((ushort)ExifTag.PDIExif, out ExifProperty pdiExif))
            return null;

        if (pdiExif?.Value is not byte[] bulk)
            return null;

        if (bulk.Length == 0)
            return bulk;

        Span<byte> header = bulk.AsSpan(0, 0x18);
        Span<byte> data;
        if (bulk[0] == 1)
        {
            var headerDecryptor = new ChaCha20(JxlHeaderEncKey, new byte[12], 0);
            headerDecryptor.DecryptBytes(header[1..], header.Length - 1);

            ulong nonceLow = MemoryMarshal.Cast<byte, ulong>(header[0x10..])[0];
            byte[] nonce = new byte[12];
            BinaryPrimitives.WriteUInt64LittleEndian(nonce.AsSpan(4), nonceLow);

            data = bulk.AsSpan(0x18, bulk.Length - 0x18);
            var dataDecryptor = new ChaCha20(JxlDataEncKey, nonce, 0);
            dataDecryptor.DecryptBytes(data, data.Length);
        }
        else
            data = bulk.AsSpan(1);

        ulong fileSize = MemoryMarshal.Cast<byte, ulong>(header[0x08..])[0];
        if (!PS2ZIP.TryInflateInMemory(data, fileSize, out byte[] deflatedData))
            return null;

        return deflatedData;
    }

    public static byte[]? ExtractExifData(byte[] jxlFileBytes)
    {
        using var ms = new MemoryStream(jxlFileBytes);
        return ExtractExifData(ms);
    }
}
