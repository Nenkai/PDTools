using System;
using System.Collections.Generic;
using System.Text;

using System.IO;
using Syroot.BinaryData;

namespace PDTools.SpecDB.Core
{
    public class SpecDBDebugPrinter
    {
        private BinaryStream _stream;

        public List<EntryInfo> _entryInfos;
        public HuffmanLookupEntry[] HuffmanLookupTable { get; set; } = new HuffmanLookupEntry[0x100];
        public HuffmanDictEntry[] HuffmanDictEntries { get; set; }

        private StreamWriter _streamWriter;

        /* Step 1: Get DB Index from DB Row using top table which is ordered 
         * Step 2: Get entry info by index, get the second int, which is offset to short table then get the HuffManpart from the "short table"
         * 
         * */
        public void Load(string tablePath)
        {
            _stream = new BinaryStream(new FileStream(tablePath, FileMode.Open));

            _stream.Position = 0x08;
            uint rowCount = _stream.ReadUInt32();
            uint rowSize = _stream.ReadUInt32();

            _entryInfos = new List<EntryInfo>((int)rowCount);
            for (var i = 0; i < rowCount; i++)
            {
                var info = new EntryInfo();
                info.Read(_stream);
                _entryInfos.Add(info);
            }

            uint huffmanTableSize = _stream.ReadUInt32();
            uint huffmanCodeDictionaryEntryCount = _stream.ReadUInt32();

            for (var i = 0; i < 0x100; i++)
            {
                var entry = new HuffmanLookupEntry();
                entry.Read(_stream);
                HuffmanLookupTable[i] = entry;
            }

            HuffmanDictEntries = new HuffmanDictEntry[huffmanCodeDictionaryEntryCount];
            for (var i = 0; i < huffmanCodeDictionaryEntryCount; i++)
            {
                var entry = new HuffmanDictEntry();
                entry.Read(_stream);
                HuffmanDictEntries[i] = entry;
            }

        }

        public void Print()
        {
            _streamWriter = new StreamWriter("debug.txt");

            PrintHuffmanDictTable();
            PrintHuffmanLookupTable();

            PrintEntryInfos();

            _streamWriter.Dispose();
        }

        private void PrintEntryInfos()
        {
            _streamWriter.WriteLine("## Row Info");
            _streamWriter.WriteLine($"Row Count: {_entryInfos.Count}");
            foreach (var entryInfo in _entryInfos)
            {
                _streamWriter.WriteLine($"- ID: {entryInfo.ID} - Offset: {entryInfo.Offset:X8}");
            }

            _streamWriter.WriteLine();
        }

        private void PrintHuffmanLookupTable()
        {
            _streamWriter.WriteLine("## Huffman Lookup Table");
            for (int i = 0; i < HuffmanLookupTable.Length; i++)
            {
                HuffmanLookupEntry val = HuffmanLookupTable[i];
                _streamWriter.Write($"[0x{i:X2}] 0x{val.Data:X2} - (Code = {val.CodeBitSize} bits)");
                if (val.CodeBitSize == 0)
                    _streamWriter.Write(" (Too big for lookup table)");
                _streamWriter.WriteLine();
            }

            _streamWriter.WriteLine();

        }

        private void PrintHuffmanDictTable()
        {
            _streamWriter.WriteLine("## Huffman Dictionary");
            foreach (var entry in HuffmanDictEntries)
            {
                _streamWriter.WriteLine($"{entry.CodeBitSize} bits - Code:{entry.Code:X8} -> 0x{entry.Data:X2}");
            }

            _streamWriter.WriteLine();
        }
    }

    public class EntryInfo
    {
        public uint ID { get; set; }
        public uint Offset { get; set; }

        public void Read(BinaryStream bs)
        {
            ID = bs.ReadUInt32();
            Offset = bs.ReadUInt32();
        }
    }

    public class HuffmanLookupEntry
    {
        public byte CodeBitSize { get; set; }
        public byte Data { get; set; }

        public void Read(BinaryStream bs)
        {
            Data = bs.Read1Byte();
            CodeBitSize = bs.Read1Byte();
        }
    }

    public class HuffmanDictEntry
    {
        public byte CodeBitSize { get; set; }
        public byte Data { get; set; }
        public uint Code { get; set; }

        public void Read(BinaryStream bs)
        {
            CodeBitSize = bs.Read1Byte();
            Data = bs.Read1Byte();
            bs.Position += 2;
            Code = bs.ReadUInt32();
        }
    }
}
