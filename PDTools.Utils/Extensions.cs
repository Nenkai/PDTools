using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Syroot.BinaryData;
using Syroot.BinaryData.Memory;

namespace PDTools.Utils
{
    public static class Extensions
    {
        /// <summary>
        /// Reads a string of a fixed size buffer. If the string within the buffer is null terminated before the size provided, the remaining nulled bytes will be trimmed.
        /// </summary>
        /// <param name="sr"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        public static string ReadFixedString(this SpanReader sr, int size)
        {
            int basePos = sr.Position;
            int endPos = basePos + size;
            byte val;

            do
                val = sr.ReadByte();
            while (val != 0 || sr.Position >= endPos);

            
            ReadOnlySpan<byte> chars = sr.Span.Slice(basePos, sr.Position - 1);
            return sr.Encoding.GetString(chars);
        }

        /// <summary>
        /// Reads a string of a fixed size buffer. If the string within the buffer is null terminated before the size provided, the remaining nulled bytes will be trimmed.
        /// </summary>
        /// <param name="sr"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        public static string ReadFixedString(this BinaryStream bs, int size)
        {
            long basePos = bs.BaseStream.Position;
            long endPos = basePos + size;
            byte val;

            do
                val = bs.Read1Byte();
            while (val != 0 || bs.BaseStream.Position >= endPos);

            int strLen = (int)((bs.BaseStream.Position - basePos) - 1);
            bs.BaseStream.Position = basePos;
            string str = bs.ReadString(strLen, Encoding.ASCII);
            bs.BaseStream.Position = basePos + size;

            return str;
        }
    }
}
