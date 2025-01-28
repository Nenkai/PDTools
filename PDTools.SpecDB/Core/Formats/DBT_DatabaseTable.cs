using System;
using System.Buffers.Binary;
using System.Diagnostics;
using System.IO;
using System.Linq;

using Syroot.BinaryData.Core;
using Syroot.BinaryData.Memory;

namespace PDTools.SpecDB.Core.Formats;

/// <summary>
/// Database Table.
/// </summary>
public class DBT_DatabaseTable
{
    public const int HeaderSize = 0x10;

    public Endian Endian { get; }
    public byte[] Buffer { get; }

    public StreamWriter _debugWriter;

    public DBT_DatabaseTable(byte[] buffer, Endian endian)
    {
        Buffer = buffer;
        Endian = endian;
    }

    /// <summary>
    /// Flags, i.e compressed
    /// </summary>
    public ushort TableAttribute { get; set; }
    public ushort AlignedBytes { get; set; }

    /// <summary>
    /// Number of rows in the table
    /// </summary>
    public uint RowCount { get; set; }

    /// <summary>
    /// Size of one row in bytes
    /// </summary>
    public int RowSize { get; set; }

    /// <summary>
    /// Aka Huffman Table Info Offset address
    /// </summary>
    public int codeBookInfoBlockAdr { get; set; }

    /// <summary>
    /// Aka Huffman prefix lookup table address
    /// </summary>
    public int dataListBlockAdr { get; set; }

    /// <summary>
    /// Address of the huffman dictionary/code list block
    /// </summary>
    public int codeListBlockAdr { get; set; }

    /// <summary>
    /// Address of the row template information block
    /// </summary>
    public int templateInfoBlockAdr { get; set; }

    /// <summary>
    /// Address of row template data (if the table is compressed). These are template/base rows that will be used as uncompressed rows, for for patching (huffman)
    /// </summary>
    public int templateDataBlockAdr { get; set; }

    /// <summary>
    /// Address of either huffman codes if the table is compressed, or raw row data
    /// </summary>
    public int dataBlockAdr { get; set; }


    public int GetIndexOfID(int targetRowId)
    {
        SpanReader sr = new SpanReader(Buffer, Endian);
        int entryCount = (int)RowCount;

        int max = entryCount - 1;
        int min = -1;
        if (entryCount > 0)
        {
            while (true)
            {
                int mid = max / 2;

                sr.Position = HeaderSize + mid * 8;
                int currentRowId = sr.ReadInt32();

                if (currentRowId == targetRowId)
                    return mid;

                if (targetRowId <= currentRowId)
                {
                    entryCount = mid;
                    mid = min;
                }

                if (entryCount <= mid + 1)
                    break;

                max = mid + entryCount;
                min = mid;
            }
        }

        return -1;
    }

    public Span<byte> ExtractRow(scoped ref SpanReader rowHuffmanCodeReader)
    {
        ExtractHuffmanPart(ref rowHuffmanCodeReader, out Span<byte> entryDataBuffer);
        return ExtractDiffDictPart(entryDataBuffer);
    }


    Span<byte> ExtractDiffDictPart(scoped Span<byte> entryData)
    {
        DebugPrint($"ExtractDiffDictPart: data: {{{string.Join(",", entryData.ToArray().Select(e => $"{e:X2}"))}}}");

        Span<byte> rawEntryData = entryData[1..];
        ExtractCompressionType type = (ExtractCompressionType)(entryData[0] >> 6);
        int rowSize = RowSize;
        int dataIndex = entryData[0] & 0b11_1111;

        DebugPrint($"ExtractDiffDictPart: type:{type}, dataIndex: {dataIndex}");
        if (type == ExtractCompressionType.Uncompressed) // Copy row from template row data
        {
            var sr = new SpanReader(Buffer, Endian);
            sr.Position = templateDataBlockAdr + dataIndex * rowSize;
            return sr.ReadBytes(rowSize);
        }
        else if (type == ExtractCompressionType.CompressedWithoutTemplateRow) // Row from raw entry data
        {
            Span<byte> rowData = new byte[rowSize];
            rawEntryData.Slice(0, rowSize).CopyTo(rowData);
            return rowData;
        }
        else if (type == ExtractCompressionType.CompressedWithTemplateRowDifferences) // Get template row & alter it from bytes
        {
            var sr = new SpanReader(Buffer, Endian);
            sr.Position = templateDataBlockAdr + dataIndex * rowSize;
            Span<byte> rowData = sr.ReadBytes(rowSize);

            DebugPrint($"ExtractDiffDictPart: base row idx {dataIndex}={{{string.Join(",", rowData.ToArray().Select(e => $"0x{e:X2}"))}}}");

            // Type 2 "Header" is one bit for each row byte indicating whether to replace the byte.
            // For instance, 1111 means replace 4 bytes
            var replacementBytesOffset = 1 + rowSize / 8;

            // Align type 2 ptr
            if (rowSize % 8 != 0)
                replacementBytesOffset++;

            for (var i = 0; i < rowSize; i++)
            {
                var patchByte = (rawEntryData.Slice(i / 8)[0] >> i % 8 & 1) == 1;
                if (patchByte && replacementBytesOffset < entryData.Length)
                    rowData[i] = entryData[replacementBytesOffset++];
            }


            return rowData;
        }

        throw new Exception($"ExtractDiffDictPart Errored: got unsupported type {type}");
    }

    public int ExtractHuffmanPart(ref SpanReader sr, out Span<byte> huffmanDict)
    {
        byte huffmanPartCount = sr.Span[sr.Position];
        huffmanDict = new byte[huffmanPartCount];
        int basePos = sr.Position;

#if DEBUG
        DebugPrint($"ExtractHuffmanPart: Part Count={huffmanPartCount}");
#endif

        int bitOffset = 0;
        for (int i = 0; i < huffmanPartCount; i++)
        {
            int currentByte = bitOffset / 8;
            sr.Position = basePos + currentByte + 1;

            // Read up to 32 bits ahead, if it passes past the buffer, pad with zeros
            uint val = 0;
            val += !sr.IsEndOfSpan ? sr.ReadByte() : 0u;
            val += !sr.IsEndOfSpan ? (uint)sr.ReadByte() << 8 : 0u;
            val += !sr.IsEndOfSpan ? (uint)sr.ReadByte() << 16 : 0u;
            val += !sr.IsEndOfSpan ? (uint)sr.ReadByte() << 24 : 0u;
            val >>= bitOffset - currentByte * 8;

            DebugPrint($"ExtractHuffmanPart: val=0x{val:X8} (bitOffset={bitOffset})");

            Span<byte> b = huffmanDict.Slice(i);
            bitOffset += (int)ProcessHuffmanCode(val, ref b);
        }

        sr.Position = basePos;
        return huffmanPartCount;
    }

    public uint ProcessHuffmanCode(uint code, ref Span<byte> huffmanPart)
    {
        /* Check if our bits yields a code that is less than 9 bits
         * If so, we can match our lookup table that maps 0x00 to 0xFF */

        SpanReader sr = new SpanReader(Buffer, Endian);
        sr.Position = dataListBlockAdr + (byte)code * 2; // navigate to lookup table entry for < 8 bit code
        DebugPrint($"ProcessHuffmanCode: LookupOffset=0x{sr.Position:X4}");

        uint codeBitSize = sr.Span[sr.Position + 1];

        DebugPrint($"ProcessHuffmanCode: codeSize={codeBitSize}");

        // If the size is 0, we need to look up the actual dictionary as our code is more than 8 bits
        if (codeBitSize == 0)
        {
            // Search for said longer code
            codeBitSize = SearchHuffmanCode(code, ref huffmanPart);
            if (codeBitSize == 0)
                throw new Exception("SearchHuffmanCode returned 0 bits, table is corrupted");
        }
        else
        {
            // Grab byte from lookup table
            huffmanPart[0] = sr.Span[sr.Position];
            DebugPrint($"ProcessHuffmanCode: huffmanPart[0]: 0x{huffmanPart[0]:X2}");
        }

        return codeBitSize;
    }

    /// <summary>
    /// Search for a huffman code that is more than 8 bits
    /// </summary>
    /// <param name="codeBits"></param>
    /// <param name="outEntryData"></param>
    /// <returns></returns>
    public uint SearchHuffmanCode(uint codeBits, ref Span<byte> outEntryData)
    {
        for (uint bitIndex = 9; bitIndex < 32; bitIndex++)
        {
            uint targetCode = (uint)(codeBits & (1 << (int)bitIndex) - 1);
            int codeEntryCount = ReadInt32(Buffer.AsSpan(codeBookInfoBlockAdr + 4), Endian);

            DebugPrint($"SearchHuffmanCode: targetIndex={targetCode}");

            int max = codeEntryCount;
            int min = -1;
            int mid;
            do
            {
                mid = (max + min) / 2;

                Span<byte> searchEntry = Buffer.AsSpan(codeListBlockAdr + mid * 8);
                byte codeBitSize = searchEntry[0];
                int code = ReadInt32(searchEntry[4..], Endian);
                if (code == targetCode && codeBitSize == bitIndex)
                {
                    outEntryData[0] = searchEntry[1]; // Data
                    return bitIndex;
                }

                if (bitIndex > codeBitSize)
                    min = mid;
                else if (bitIndex < codeBitSize)
                    max = mid;
                else if (codeBitSize == bitIndex)
                {
                    if (targetCode > code)
                        min = mid;
                    else
                        max = mid;
                }
            } while (min + 1 != max);
        }

        return 0;
    }

    public static int ReadInt32(Span<byte> buffer, Endian endian)
    {
        return endian == Endian.Big ?
                  BinaryPrimitives.ReadInt32BigEndian(buffer)
                : BinaryPrimitives.ReadInt32LittleEndian(buffer);
    }

    private void DebugPrint(string message)
    {
#if DEBUG
        _debugWriter.WriteLine(message);
#endif
    }

    public enum ExtractCompressionType : byte
    {
        Uncompressed = 0,
        CompressedWithoutTemplateRow = 1,
        CompressedWithTemplateRowDifferences = 2,
    }
}
