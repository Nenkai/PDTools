using System;

using System.Runtime.InteropServices;

namespace PDTools.GT4ElfBuilderTool
{
    public class BufferReverser
    {
        private int field_4;
        private int PositionInt;
        private int SizeInt;
        private int field_10;
        private int[] OperatingBuffer;

        public void InitReverse(Span<byte> data) // 102FCC0
        {
            field_4 = 0;
            PositionInt = 0;
            SizeInt = 0;
            field_10 = 0;
            OperatingBuffer = null;

            InitInternalBuffer((data.Length + 3) >> 2, true);

            // This does the reverse
            for (int i = 0; i < data.Length; i++)
            {
                var target = OperatingBuffer.AsSpan(i / 4); // don't ask.. seems to be that way

                int currentByte = data[(data.Length - 1) - i] << (8 * (i & 3));
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

        private void InitInternalBuffer(int size, bool clearBuffer) // 10300C8
        {
            if (SizeInt < size)
            {
                int i = 1;
                if (size > 1)
                {
                    for (i = 2; i < size; i *= 2)
                        ;
                }

                int[] buf = new int[i];
                if (OperatingBuffer != null)
                {

                }

                if (clearBuffer)
                    buf.AsSpan().Fill(0); // memset

                SizeInt = i;
                OperatingBuffer = buf;
            }
        }
    }
}
