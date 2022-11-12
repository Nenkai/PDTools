using System;

using System.Runtime.InteropServices;

namespace PDTools.GT4ElfBuilderTool
{
    public class BufferReverser
    {
        public int RefCount;
        public int PositionInt;
        private int SizeInt;
        public int field_10;
        public uint[] OperatingBuffer;

        public void InitReverse(Span<byte> data) // 102FCC0
        {
            RefCount = 0;
            PositionInt = 0;
            SizeInt = 0;
            field_10 = 0;
            OperatingBuffer = null;

            InitInternalBuffer((data.Length + 3) / 4, true);

            // This does the reverse
            for (int i = 0; i < data.Length; i++)
            {
                var target = OperatingBuffer.AsSpan(i / 4); // don't ask.. seems to be that way

                uint currentByte = (uint)data[(data.Length - 1) - i] << (8 * (i % 4));
                target[0] |= currentByte;
            }

            PositionInt = SizeInt;
            if (SizeInt > 0)
            {
                if (OperatingBuffer[SizeInt - 1] == 0)
                {

                }
            }
        }

        public void InitInternalBuffer(int size, bool clearBuffer) // 10300C8
        {
            if (SizeInt < size)
            {
                int i = 1;
                if (size > 1)
                {
                    for (i = 2; i < size; i *= 2)
                        ;
                }

                uint[] buf = new uint[i];
                if (OperatingBuffer != null)
                {
                    // memcpy(arrayBUffer, OperatingBuffer, PositionInt * 4)
                    OperatingBuffer.AsSpan(PositionInt).CopyTo(buf);
                }

                if (clearBuffer)
                {
                    // memset(&arrayBuffer[PositionInt], 0, (i - PositionInt) * 4);
                    buf.AsSpan(PositionInt * sizeof(uint), i - PositionInt).Fill(0);
                }

                SizeInt = i;
                OperatingBuffer = buf;
            }
        }
    }
}
