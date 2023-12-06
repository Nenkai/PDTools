using System;
using System.Buffers.Binary;
using System.Diagnostics;
using System.IO;
using System.Linq;

using Syroot.BinaryData.Core;
using Syroot.BinaryData.Memory;

namespace PDTools.SpecDB.Core.Formats
{
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

        public int HuffmanTableInfoOffset { get; set; }
        public int HuffmanPrefixLookupTable { get; set; }

        public int HuffmanDictTable { get; set; }

        public int RowDataTableInfoOffset { get; set; }
        public int RowDataOffset { get; set; }

        public int HuffmanCodesOrRowDataOffset { get; set; }

        public int EntryCount
        {
            get
            {
                if (Endian == Endian.Little)
                    return BinaryPrimitives.ReadInt32LittleEndian(Buffer.AsSpan(0x08));
                else
                    return BinaryPrimitives.ReadInt32BigEndian(Buffer.AsSpan(0x08));
            }
        }

        public int RowDataLength
        {
            get
            {
                if (Endian == Endian.Little)
                    return BinaryPrimitives.ReadInt32LittleEndian(Buffer.AsSpan(0x0C));
                else
                    return BinaryPrimitives.ReadInt32BigEndian(Buffer.AsSpan(0x0C));
            }
        }

        public int VersionHigh
        {
            get
            {
                if (Endian == Endian.Little)
                    return BinaryPrimitives.ReadInt16LittleEndian(Buffer.AsSpan(0x04));
                else
                    return BinaryPrimitives.ReadInt16BigEndian(Buffer.AsSpan(0x04));
            }
        }

        public int GetIndexOfID(int targetRowId)
        {
            SpanReader sr = new SpanReader(Buffer, Endian);
            int entryCount = EntryCount;

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

        public Span<byte> ExtractRow(ref SpanReader rowHuffmanCodeReader)
        {
            ExtractHuffmanPart(ref rowHuffmanCodeReader, out Span<byte> entryDataBuffer);
            return ExtractDiffDictPart(entryDataBuffer);
        }


        Span<byte> ExtractDiffDictPart(Span<byte> entryData)
        {
            DebugPrint($"ExtractDiffDictPart: data: {{{string.Join(",", entryData.ToArray().Select(e => $"{e:X2}"))}}}");

            Span<byte> rawEntryData = entryData.Slice(1);
            ExtractCompressionType type = (ExtractCompressionType)(entryData[0] >> 6);
            int rowLength = RowDataLength;
            int dataIndex = entryData[0] & 0b11_1111;

            DebugPrint($"ExtractDiffDictPart: type:{type}, dataIndex: {dataIndex}");
            if (type == ExtractCompressionType.Uncompressed) // Copy row from shared full row data
            {
                var sr = new SpanReader(Buffer, Endian);
                sr.Position = RowDataOffset + dataIndex * rowLength;
                return sr.ReadBytes(rowLength);
            }
            else if (type == ExtractCompressionType.CompressedWithoutBase) // Row from raw entry data
            {
                return rawEntryData.Slice(0, rowLength);
            }
            else if (type == ExtractCompressionType.CompressedWithBaseRowDifferences) // Apply data to base row from dict
            {
                var sr = new SpanReader(Buffer, Endian);
                sr.Position = RowDataOffset + dataIndex * rowLength;
                Span<byte> rowData = sr.ReadBytes(rowLength);

                DebugPrint($"ExtractDiffDictPart: base row idx {dataIndex}={{{string.Join(",", rowData.ToArray().Select(e => $"0x{e:X2}"))}}}");

                // Type 2 "Header" is one bit for each row byte indicating whether to replace a byte
                var replacementBytesOffset = 1 + rowLength / 8;

                // Align type 2 ptr
                if (rowLength % 8 != 0)
                    replacementBytesOffset++;

                for (var i = 0; i < rowLength; i++)
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
            sr.Position = HuffmanPrefixLookupTable + (byte)code * 2; // navigate to lookup table entry for < 8 bit code
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
                int codeEntryCount = ReadInt32(Buffer.AsSpan(HuffmanTableInfoOffset + 4), Endian);

                DebugPrint($"SearchHuffmanCode: targetIndex={targetCode}");

                int max = codeEntryCount;
                int min = -1;
                int mid;
                do
                {
                    mid = (max + min) / 2;

                    Span<byte> searchEntry = Buffer.AsSpan(HuffmanDictTable + mid * 8);
                    byte codeBitSize = searchEntry[0];
                    int code = ReadInt32(searchEntry.Slice(4), Endian);
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

        public int ReadInt32(Span<byte> buffer, Endian endian)
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
            CompressedWithoutBase = 1,
            CompressedWithBaseRowDifferences = 2,
        }
    }
}
