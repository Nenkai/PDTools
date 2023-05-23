using System;

using System.Runtime.InteropServices;

namespace PDTools.GT4ElfBuilderTool
{
    public class PDIBigNumber
    {
        public int RefCount_0x04;
        public int CurrentLength_0x08;
        public int Capacity_0x0C;
        public int field_10;
        public uint[] OperatingBufferPtr_0x14;

        public void InitFromBuffer(Span<byte> data) // 102FCC0
        {
            RefCount_0x04 = 0;
            CurrentLength_0x08 = 0;
            Capacity_0x0C = 0;
            field_10 = 0;
            OperatingBufferPtr_0x14 = null;

            ResizeBuffer_10300C8((data.Length + 3) / 4, true);

            // Initial buffer is reversed
            for (int i = 0; i < data.Length; i++)
            {
                var target = OperatingBufferPtr_0x14.AsSpan(i / 4); // don't ask.. seems to be that way

                uint currentByte = (uint)data[(data.Length - 1) - i] << (8 * (i % 4));
                target[0] |= currentByte;
            }

            CurrentLength_0x08 = Capacity_0x0C;
            if (Capacity_0x0C > 0)
            {
                // Set length to last non-zeros
                for (var i = CurrentLength_0x08 - 1; i >= 0; i--)
                {
                    if (OperatingBufferPtr_0x14[i] != 0)
                        break;

                    CurrentLength_0x08 = i;
                }
            }
        }

        public void ResizeBuffer_10300C8(int size, bool clearRest)
        {
            if (Capacity_0x0C < size)
            {
                int i = 1;
                if (size > 1)
                {
                    for (i = 2; i < size; i *= 2)
                        ;
                }

                uint[] buf = new uint[i];
                if (OperatingBufferPtr_0x14 != null)
                {
                    // memcpy(arrayBUffer, OperatingBuffer, Length * 4)
                    MemCpyInt(buf, OperatingBufferPtr_0x14, CurrentLength_0x08);
                }

                if (clearRest)
                {
                    // memset(&arrayBuffer[PositionInt], 0, (i - PositionInt) * 4);
                    MemsetInt(buf.AsSpan(CurrentLength_0x08), 0, i - CurrentLength_0x08);
                }

                Capacity_0x0C = i;
                OperatingBufferPtr_0x14 = buf;
            }
        }

        public void RotateLeft_1031E18()
        {
            uint currentBit = 0;
            for (var i = 0; i < CurrentLength_0x08; i++)
            {
                uint currentBitsInt = OperatingBufferPtr_0x14[i];
                OperatingBufferPtr_0x14[i] = (currentBitsInt << 1) | currentBit;

                currentBit = currentBitsInt >> 31; // Move next
            }

            if (currentBit != 0) // Remaining bits?
                InsertValue_1030198(CurrentLength_0x08, 1);

        }

        public void InsertValue_1030198(int index, uint value)
        {
            if (index >= CurrentLength_0x08)
            {
                // Create new one to fit
                if (value > 0)
                {
                    ResizeBuffer_10300C8(index + 1, true);
                    OperatingBufferPtr_0x14[index] = value;
                    CurrentLength_0x08 = index + 1;
                }
            }
            else
            {
                OperatingBufferPtr_0x14[index] = value;

                // Shrink length if last value is 0
                if (value == 0)
                {
                    if (CurrentLength_0x08 > 0 && OperatingBufferPtr_0x14[CurrentLength_0x08 - 1] == 0)
                    {
                        for (var i = CurrentLength_0x08 - 1; i >= 0; i--)
                        {
                            if (OperatingBufferPtr_0x14[i] != 0)
                                break;

                            CurrentLength_0x08 = i;
                        }
                    }
                }
            }
        }

        // Static
        public static int Equals_102FF50(PDIBigNumber leftBuf, PDIBigNumber rightBuf)
        {
            if (leftBuf.field_10 == rightBuf.field_10)
            {
                int result = CompareBuffers_102FED0(leftBuf, rightBuf);
                if (leftBuf.field_10 != 0)
                    return -result;

                return result;
            }
            else
            {
                if (leftBuf.field_10 == 0)
                    return rightBuf.field_10 != 0 ? 1 : 0;

                return -1;
            }
        }

        public static int CompareBuffers_102FED0(PDIBigNumber leftBuf, PDIBigNumber rightBuf)
        {
            int left = leftBuf.CurrentLength_0x08;
            int right = rightBuf.CurrentLength_0x08;

            if (left >= right)
            {
                if (right < left)
                    return 1;
                else
                {
                    int max = left - 1;
                    if (max >= 0)
                    {
                        for (var i = max; i >= 0; i--)
                        {
                            uint leftVal = leftBuf.OperatingBufferPtr_0x14[i];
                            uint rightVal = leftBuf.OperatingBufferPtr_0x14[i];

                            if (leftVal < rightVal)
                                return -1;
                            else if (rightVal < leftVal)
                                return 1;
                        }
                    }
                    else
                    {
                        return 0;
                    }
                }
            }

            return -1;
        }

        static void MemCpyInt(Span<uint> output, Span<uint> input, int length)
        {
            input.Slice(0, length).CopyTo(output.Slice(0, length));
        }

        static void MemsetInt(Span<uint> ptr, uint value, int num)
        {
            ptr.Slice(0, num).Fill(value);
        }
    }
}
