using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Buffers.Binary;

using Syroot.BinaryData.Core;
using Syroot.BinaryData.Memory;

namespace PDTools.SpecDB.Core.Formats
{
    /// <summary>
    /// Parts/Table Information.
    /// </summary>
    public class TBI_PartsInfo
    {
        public Endian Endian { get; }
        public byte[] Buffer { get; set; }
        public int EntryOffset;
        public int EndOffset;
        public int OffsetUnk;
        public int OffsetUnk2;
        public int OffsetUnk3;

        public const int HeaderSize = 0x20;

        public int KeyCount
        {
            get
            {
                if (Endian == Endian.Little)
                    return BinaryPrimitives.ReadInt32LittleEndian(Buffer.AsSpan(0x08));
                else
                    return BinaryPrimitives.ReadInt32BigEndian(Buffer.AsSpan(0x08));
            }
        }

        public int Version
        {
            get
            {
                if (Endian == Endian.Little)
                    return BinaryPrimitives.ReadInt32LittleEndian(Buffer.AsSpan(0x14));
                else
                    return BinaryPrimitives.ReadInt32BigEndian(Buffer.AsSpan(0x14));
            }
        }

        public TBI_PartsInfo(byte[] buffer, Endian endian)
        {
            Buffer = buffer;
            Endian = endian;
        }

        public void Read(string path)
        {
            Buffer = File.ReadAllBytes(path);
            SpanReader sr = new SpanReader(Buffer);

            if (sr.ReadStringRaw(4) != "GTST")
                return;

            sr.Position = 0x08;

            int entryCount = sr.Position = 0x14;
            sr.Position = HeaderSize;
            EntryOffset = sr.Position;

            int entrySize = (Version == 1 ? 0x08 : 0x10) * entryCount;
            EndOffset = HeaderSize + entrySize;
            OffsetUnk = HeaderSize + entrySize + 0x28;
            OffsetUnk2 = HeaderSize + entrySize + 0x228;
        }

        public int FindCarIDByIndex(int target)
        {
            int entryCount = KeyCount;

            if (entryCount > 0)
            {
                int mid;
                int max = entryCount + -1;
                int min = -1;
                do
                {
                    mid = max / 2;
                    int uVar2 = GetCarIDFromIndex(mid);
                    if (uVar2 == target)
                    {
                        return mid;
                    }
                    if ((uint)target <= uVar2)
                    {
                        entryCount = mid;
                        mid = min;
                    }
                    max = mid + entryCount;
                    min = mid;
                } while (mid + 1 < entryCount);
            }
            return -1;
        }

        public int GetCarIDFromIndex(int index)
        {
            SpanReader sr = new SpanReader(Buffer);
            if (Version == 1)
            {
                sr.Position = EntryOffset + index * 0x08;
                return sr.ReadInt32();
            }
            else
                return GetEntryFromTBIV2(ref sr, index);

        }

        private int GetEntryFromTBIV2(ref SpanReader sr, int index)
        {
            sr.Position = EntryOffset + index * 0x10;
            return sr.ReadInt32();
        }

    }
}
