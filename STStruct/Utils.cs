using System;
using System.Collections.Generic;
using System.Text;

using Syroot.BinaryData;
using Syroot.BinaryData.Memory;

namespace PDTools.STStruct;

public static class Utils
{
    public static uint Read7BitUInt32(this ref SpanReader sr)
    {
        uint value = sr.ReadByte();
        if ((value & 0x80) != 0)
        {
            uint mask = 0x80;
            do
            {
                value = ((value - mask) << 8) + sr.ReadByte();
                mask <<= 7;
            } while ((value & mask) != 0);
        }

        return value;
    }

    public static void EncodeAndAdvance(this BinaryStream bs, uint value)
    {
        uint mask = 0x80;
        Span<byte> buffer = [];

        if (value <= 0x7F)
        {
            bs.WriteByte((byte)value);
            return;
        }
        else if (value <= 0x3FFF)
        {
            Span<byte> tempBuf = BitConverter.GetBytes(value).AsSpan();
            tempBuf.Reverse();
            buffer = tempBuf.Slice(2, 2);
        }
        else if (value <= 0x1FFFFF)
        {
            Span<byte> tempBuf = BitConverter.GetBytes(value).AsSpan();
            tempBuf.Reverse();
            buffer = tempBuf.Slice(1, 3);
        }
        else if (value <= 0xFFFFFFF)
        {
            buffer = BitConverter.GetBytes(value);
            buffer.Reverse();
        }
        else if (value <= 0xFFFFFFFF)
        {
            buffer = BitConverter.GetBytes(value);
            buffer.Reverse();
            buffer = new byte[] { 0, buffer[0], buffer[1], buffer[2], buffer[3] };
        }
        else
            throw new Exception("????");

        for (int i = 1; i < buffer.Length; i++)
        {
            buffer[0] += (byte)mask;
            mask >>= 1;
        }

        bs.Write(buffer.ToArray());
    }
}
