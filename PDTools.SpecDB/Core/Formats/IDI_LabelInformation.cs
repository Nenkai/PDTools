using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Buffers.Binary;

using Syroot.BinaryData.Core;
using Syroot.BinaryData.Memory;


namespace PDTools.SpecDB.Core.Formats
{
    /// <summary>
    /// Label/ID Information.
    /// </summary>
    public class IDI_LabelInformation
    {
        public Endian Endian { get; }
        public byte[] Buffer { get; }
        public const int HeaderSize = 0x10;
        public const int EntrySize = 0x08;

        public int KeyCount
        {
            get
            {
                if (Endian == Endian.Little)
                    return BinaryPrimitives.ReadInt32LittleEndian(Buffer.AsSpan(0x04));
                else
                    return BinaryPrimitives.ReadInt32BigEndian(Buffer.AsSpan(0x04));
            }
        }

        public int TableIndex
        {
            get
            {
                if (Endian == Endian.Little)
                    return BinaryPrimitives.ReadInt32LittleEndian(Buffer.AsSpan(0x0C));
                else
                    return BinaryPrimitives.ReadInt32BigEndian(Buffer.AsSpan(0x0C));
            }
        }

        public IDI_LabelInformation(byte[] buffer, Endian endian)
        {
            Buffer = buffer;
            Endian = endian;
        }

        public int SearchLabelID(string label)
        {
            SpanReader sr = new SpanReader(Buffer, Endian);
            sr.Position = 4;
            int keyCount = sr.ReadInt32();

            if (keyCount > 0)
            {
                int max = keyCount - 1;
                int i = keyCount;
                int min = -1;

                // Binary searching - Keys are ordered by length.
                int mid;
                do
                {
                    mid = max / 2; // Start at the middle

                    // Depending of the result of the string comparison, we travel forwards or backwards as fast lookup
                    int compResult = CompareIDString(label, mid, label.Length);
                    if (compResult == 0) // Found match
                    {
                        if (mid < 0 || mid > keyCount)
                            return -1;

                        sr.Position = HeaderSize + mid * EntrySize + 4;
                        return sr.ReadInt32();
                    }

                    if (compResult < 1) // String Length is under?, we look forwards
                    {
                        i = mid;
                        mid = min;
                    }

                    max = mid + i;
                    min = mid;
                } while (i != mid + 1);
            }

            return -1;
        }

        public int CompareIDString(string label, int index, int labelLength)
        {
            SpanReader sr = new SpanReader(Buffer, Endian);

            //  iVar2 = (uint)*(ushort *)(buf + *(int *)(buf + index * 8 + 0x10) + *(int *)(buf + 4) * 8 + 0x10) - 1;

            // *(int *)(buf + 4) * 8 + 0x10)
            int keyCount = KeyCount;
            int keyMapOffset = HeaderSize + keyCount * EntrySize;

            // *(int *)(buf + index * 8 + 0x10)
            sr.Position = HeaderSize + index * EntrySize;
            int keyNameOffset = sr.ReadInt32();

            sr.Position = keyMapOffset + keyNameOffset;

            short stringLength = sr.ReadInt16();

            labelLength++; // Null terminated
            if (stringLength == labelLength)
            {
                string labelEntry = sr.ReadString0();
                return label.CompareTo(labelEntry);
            }

            return labelLength - stringLength;
        }

    }
}
