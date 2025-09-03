using Syroot.BinaryData;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using PDTools.Utils;
using PDTools.SpecDB.Core;
using static PDTools.SpecDB.Core.Formats.DBT_DatabaseTable;

namespace PDTools.SpecDB.Core;

public class CompressedTableWriter
{
    public Table Table { get; set; }

    public CompressedTableWriter(Table table)
    {
        Table = table;
    }

    public void SaveDBTableCompressed(string outputPath)
    {
        // Create a dummy if there's no rows
        if (Table.Rows.Count == 0)
        {
            BitStream hdrStream = new BitStream(endian: Table.BigEndian ? BitStreamSignificantBitOrder.LSB : BitStreamSignificantBitOrder.MSB);
            hdrStream.WriteByteData(Encoding.UTF8.GetBytes("GTDB"));
            hdrStream.WriteInt16(0x01);
            hdrStream.WriteInt16(0x08);
            hdrStream.WriteInt32(Table.Rows.Count);
            hdrStream.WriteInt32(Table.TableMetadata.GetColumnSize());

            hdrStream.WriteInt32(0x208);
            hdrStream.WriteInt32(0);
            hdrStream.WriteInt32(8);
            hdrStream.WriteInt32(0);

            File.WriteAllBytes(outputPath, hdrStream.GetBuffer().ToArray());
            return;
        }

        /* Step 1: Serialize all the rows.
         * We'll need them to build our huffman tree. */
        List<byte[]> rowDataList = [];
        for (int i = 0; i < Table.Rows.Count; i++)
        {
            RowData row = Table.Rows[i];

            using var ms = new MemoryStream();
            using var mbs = new BinaryStream(ms, Table.BigEndian ? ByteConverter.Big : ByteConverter.Little);
            foreach (var data in row.ColumnData)
                data.Serialize(mbs);

            mbs.Flush();
            rowDataList.Add(ms.ToArray());
        }

        /* Step 2: Determine how the rows should be compressed.
         * The headers, types, etc are also encoded with huffman, therefore to determine the frequencies we need to take care of that first. */
        List<byte[]> writtenUncompressedRows = [];
        List<RowComparisonData> rowCompressInfo = [];
        var huff = new DatabaseTableHuffmanTree();

        // Start writing each row
        for (var i = 0; i < Table.Rows.Count; i++)
        {
            /* The first row will always be written uncompressed, further rows may use huffman codes and use a base uncompressed row
             * and patch bytes with the huffman codes translated to a byte
             * If no base row is suitable, then the current row will also be written uncompressed 
             * A maximum of 64 uncompressed rows are allowed through code search - 6 bits */
            if (i == 0)
            {
                byte typeHdr = MakeEntryHeader(ExtractCompressionType.Uncompressed, 0);
                huff.IncrementFrequencyOfByte(typeHdr);

                writtenUncompressedRows.Add(rowDataList[0]);
                huff.IncrementFrequencyOfBytes(rowDataList[0]);

                rowCompressInfo.Add(new RowComparisonData(ExtractCompressionType.Uncompressed) { EntryHeader = typeHdr });
            }
            else
            {
                byte[] current = rowDataList[i];

                // Is it possible to use any existing uncompressed row for byte replacements?
                (int ExactMatchIndex, int BaseRowToUse, byte[] DiffBitsAndData) result = Compare(writtenUncompressedRows, current);

                // Found a candidate?
                if (result.ExactMatchIndex != -1)
                {
                    // Duplicate of an existing row
                    byte typeHdr = MakeEntryHeader(ExtractCompressionType.Uncompressed, (byte)result.ExactMatchIndex);
                    huff.IncrementFrequencyOfByte(typeHdr);

                    rowCompressInfo.Add(new RowComparisonData(ExtractCompressionType.Uncompressed) { EntryHeader = typeHdr });
                }
                else if (result.BaseRowToUse != -1)
                {
                    byte typeHdr = MakeEntryHeader(ExtractCompressionType.CompressedWithTemplateRowDifferences, (byte)result.BaseRowToUse);  // Type 2
                    huff.IncrementFrequencyOfByte(typeHdr);
                    huff.IncrementFrequencyOfBytes(result.DiffBitsAndData);

                    rowCompressInfo.Add(new RowComparisonData(ExtractCompressionType.CompressedWithTemplateRowDifferences)
                    {
                        EntryHeader = typeHdr,
                        DiffBitsAndData = result.DiffBitsAndData
                    });
                }
                else if (result.ExactMatchIndex == -1 && result.BaseRowToUse == -1)
                {
                    /* This is a rare case where tables with a lot of columns implies a lot of differences (i.e ALLOW_ENTRY)
                     * the space taken by the bits for differences take too many huff codes along with the actual bytes to patch the base row with
                     * So add the row as uncompressed. */
                    Debug.WriteLine(writtenUncompressedRows.Count < 64, "Ran out of space to write any uncompressed rows for a table with too many columns to fit differences");

                    writtenUncompressedRows.Add(current);

                    byte typeHdr = MakeEntryHeader(ExtractCompressionType.Uncompressed, (byte)(writtenUncompressedRows.Count - 1));
                    huff.IncrementFrequencyOfByte(typeHdr);
                    huff.IncrementFrequencyOfBytes(rowDataList[0]);

                    rowCompressInfo.Add(new RowComparisonData(ExtractCompressionType.Uncompressed)
                    {
                        EntryHeader = typeHdr,
                    });
                }
                else
                {
                    // We need to add the row as uncompressed
                    if (writtenUncompressedRows.Count >= 64)
                    {
                        // We can't, already exceeded total size, have a compressed row without base to patch
                        byte typeHdr = MakeEntryHeader(ExtractCompressionType.CompressedWithoutTemplateRow, 0);
                        huff.IncrementFrequencyOfByte(typeHdr);
                        huff.IncrementFrequencyOfBytes(current);

                        rowCompressInfo.Add(new RowComparisonData(ExtractCompressionType.Uncompressed)
                        {
                            EntryHeader = typeHdr,
                            RowDataWithoutBase = current,
                        });
                    }
                    else
                    {
                        // Adding new uncompressed row
                        writtenUncompressedRows.Add(current);

                        byte typeHdr = MakeEntryHeader(ExtractCompressionType.Uncompressed, (byte)(writtenUncompressedRows.Count - 1));
                        huff.IncrementFrequencyOfByte(typeHdr);
                        huff.IncrementFrequencyOfBytes(rowDataList[0]);

                        rowCompressInfo.Add(new RowComparisonData(ExtractCompressionType.Uncompressed)
                        {
                            EntryHeader = typeHdr,
                        });
                    }
                }
            }
        }

        /* We now have our final frequency table, order it */
        huff.ReorderFrequencyTable();

        /* Step 3: Build huffman tree, huffman lookup table (for <8 bit codes) and dictionary (above 8 bits) */
        huff.BuildTreeAndLookupTable();

        /* Step 4: Start writing the GT Database Table header */
        BitStream bs = new BitStream(endian: Table.BigEndian ? BitStreamSignificantBitOrder.LSB : BitStreamSignificantBitOrder.MSB);
        bs.WriteByteData(Encoding.UTF8.GetBytes("GTDB"));
        bs.WriteInt16(0x01); // Attributes
        bs.WriteInt16(0x08); // AlignedBytes
        bs.WriteInt32(Table.Rows.Count);
        int columnSize = Table.TableMetadata.GetColumnSize();
        bs.WriteInt32(columnSize);

        int entriesOffset = bs.Position;

        /* Step 5: Write row data type + codes and entries (id + offset to codes) */
        BitStream huffmanCodesWritter = new BitStream(1024, BitStreamSignificantBitOrder.MSB); // Always MSB

        for (var i = 0; i < rowCompressInfo.Count; i++)
        {
            int entryHuffmanCodesOffset = huffmanCodesWritter.Position;

            var info = rowCompressInfo[i];
            if (info.Type == ExtractCompressionType.CompressedWithTemplateRowDifferences)
            {
                Debug.Assert(1 + (info.DiffBitsAndData?.Length ?? 0) < 256, "Too many codes for CompressedWithBaseRowDifferences");
                huffmanCodesWritter.WriteByte((byte)(1 + (byte)(info.DiffBitsAndData?.Length ?? 0)));
                huff.EncodeValueToStream(ref huffmanCodesWritter, info.EntryHeader);

                for (var j = 0; j < info.DiffBitsAndData.Length; j++)
                    huff.EncodeValueToStream(ref huffmanCodesWritter, info.DiffBitsAndData[j]);
            }
            else if (info.Type == ExtractCompressionType.CompressedWithoutTemplateRow) // Unlikely to ever be hit, but here as a fallback
            {
                huffmanCodesWritter.WriteByte((byte)(1 + (byte)(info.RowDataWithoutBase?.Length ?? 0)));
                huff.EncodeValueToStream(ref huffmanCodesWritter, info.EntryHeader);

                for (var j = 0; j < info.RowDataWithoutBase.Length; j++)
                    huff.EncodeValueToStream(ref huffmanCodesWritter, info.RowDataWithoutBase[j]);
            }
            else
            {
                huffmanCodesWritter.WriteByte(1);
                huff.EncodeValueToStream(ref huffmanCodesWritter, info.EntryHeader);
            }

            huffmanCodesWritter.AlignToNextByte();

            bs.Position = entriesOffset + i * 8;
            bs.WriteInt32(Table.Rows[i].ID);
            bs.WriteInt32(entryHuffmanCodesOffset);
        }

        /* Step 6: Write huffman dictionary & lookup table */
        int huffmanTableInfoOffset = bs.Position;
        bs.Position += 4 + 4;

        // Write huffman dictionary, only searched for codes that are more than 8 bits
        for (var i = 0; i < 0x100; i++)
        {
            if (huff.LookupTable[i] != null)
            {
                bs.WriteByte(huff.LookupTable[i].Data);
                bs.WriteByte(huff.LookupTable[i].BitSizeCode);
            }
            else
            {
                bs.Position += 0x02;
            }
        }

        // Lookup table when codes are 0-8 bits
        foreach (Node node in huff.Leaves.Values)
        {
            bs.WriteByte(node.BitSizeCode);
            bs.WriteByte(node.Data.Value);
            bs.Position += 2;
            bs.WriteUInt32(node.Code);
        }

        /* Step 7: Write huffman table offsets and counts */
        int uncompressedRowDataTableInfoOffset = bs.Position;

        bs.Position = huffmanTableInfoOffset;
        bs.WriteUInt32((uint)(uncompressedRowDataTableInfoOffset - huffmanTableInfoOffset));
        bs.WriteUInt32((uint)huff.Leaves.Values.Count);

        /* Step 8: Write uncompressed rows info & data */
        bs.Position = uncompressedRowDataTableInfoOffset;
        bs.WriteInt32(8 + writtenUncompressedRows.Count * columnSize);
        bs.WriteInt32(writtenUncompressedRows.Count);

        for (var i = 0; i < writtenUncompressedRows.Count; i++)
            bs.WriteByteData(writtenUncompressedRows[i]);

        /* Step 9: Write row codes at the beginning of the file */
        bs.WriteByteData(huffmanCodesWritter.GetBuffer());

        /* Step 10: Done! */
        File.WriteAllBytes(outputPath, bs.GetBuffer().ToArray());
    }

    /// <summary>
    /// Compare rows and finds a duplicate, or a suitable one to patch bytes with
    /// </summary>
    /// <param name="uncompressedRows"></param>
    /// <param name="current"></param>
    /// <returns></returns>
    private static (int ExactMatchIndex, int BaseRowToUse, byte[] Differences) Compare(List<byte[]> uncompressedRows, byte[] current)
    {
        // Try finding an exact match first
        for (var r = 0; r < uncompressedRows.Count; r++)
        {
            byte[] baseRow = uncompressedRows[r];
            if (baseRow.AsSpan().SequenceEqual(current))
            {
                // Match!
                return (r, -1, default);
            }
        }

        // Find an uncompressed row that's closest to what we have
        int maxMatchingBytes = 0;
        int baseRowToUse = -1;
        for (var r = 0; r < uncompressedRows.Count; r++)
        {
            int matchingBytes = 0;
            for (var x = 0; x < uncompressedRows[r].Length; x++)
            {
                if (uncompressedRows[r][x] == current[x])
                {
                    matchingBytes++;

                    if (matchingBytes > maxMatchingBytes)
                    {
                        maxMatchingBytes = matchingBytes;
                        baseRowToUse = r;
                    }
                }
            }
        }

        if (baseRowToUse != -1)
        {
            // Build type 2 (differences) header where every bit tells whether to skip a row byte
            BitStream bitStream = new BitStream(0x100, endian: BitStreamSignificantBitOrder.MSB); // Always MSB
            var baseRow = uncompressedRows[baseRowToUse];
            for (var x = 0; x < baseRow.Length; x++)
                bitStream.WriteBoolBit(baseRow[x] != current[x]); // Patch byte

            bitStream.AlignToNextByte();
            for (var x = 0; x < baseRow.Length; x++)
            {
                if (baseRow[x] != current[x])
                    bitStream.WriteByte(current[x]); // Write actual bytes to patch
            }

            var differences = bitStream.GetBuffer().ToArray();
            if (differences.Length > 0xFF)
                return (-1, -1, null); // Diff data is too big to fit

            return (-1, baseRowToUse, differences);
        }

        return (-1, baseRowToUse, default);
    }

    private static byte MakeEntryHeader(ExtractCompressionType type, byte rowIndex)
    {
        return (byte)((byte)type << 6 | rowIndex & 0b111111);
    }
}

public class RowComparisonData
{
    public RowComparisonData(ExtractCompressionType type)
    {
        Type = type;
    }

    public ExtractCompressionType Type { get; set; }
    public byte EntryHeader { get; set; }
    public byte[] DiffBitsAndData { get; set; }
    public byte[] RowDataWithoutBase { get; set; }
}
