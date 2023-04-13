using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

using System.Threading;

using Syroot.BinaryData;
using Syroot.BinaryData.Memory;

using System.Buffers;

using PDTools.Crypto;

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
        public static string ReadFixedString(this ref SpanReader sr, int size)
        {
            int basePos = sr.Position;
            int maxPos = basePos + size;
            byte val;

            int endStrPos = basePos;
            do
            {
                val = sr.ReadByte();
                endStrPos++;
            }
            while (val != 0 && sr.Position < maxPos);

            if (endStrPos - 1 <= basePos)
            {
                sr.Position = maxPos;
                return string.Empty;
            }

#if NETCOREAPP2_1_OR_GREATER
            ReadOnlySpan<byte> chars = sr.Span.Slice(basePos, (endStrPos - basePos) - 1);
             sr.Position = maxPos;

            return sr.Encoding.GetString(chars);
#else
            unsafe
            {
                fixed (byte* bp = sr.Span)
                {
                    string str = sr.Encoding.GetString(bp, (endStrPos - basePos) - 1);
                    sr.Position = maxPos;
                    return str;
                }
            }
#endif
        }

        public static string AlignString(this string value, int align)
        {
            var sb = new StringBuilder(value);
            while (sb.Length % align != 0)
            {
                sb.Append('\0');
            }
            return sb.ToString();
        }

        /// <summary>
        /// Copies a stream to another, while also doing a crc of the data.
        /// </summary>
        /// <param name="src"></param>
        /// <param name="target"></param>
        /// <param name="bufferSize"></param>
        /// <returns></returns>
        public static uint CopyToChecksum(this Stream src, Stream target, int bufferSize)
        {
            uint crc = 0;

            long len = src.Length;
            byte[] buffer = ArrayPool<byte>.Shared.Rent(bufferSize);
            while (len > 0)
            {
                int toWrite = (int)Math.Min(len, bufferSize);
                src.Read(buffer, 0, buffer.Length);
                target.Write(buffer, 0, toWrite);

                crc = CRC32.CRC32_0x04C11DB7(buffer.AsSpan(0, toWrite), crc);
                len -= toWrite;
            }

            return crc;
        }

        /// <summary>
        /// Copies a stream to another asynchronously, while also doing a crc of the data.
        /// </summary>
        /// <param name="src"></param>
        /// <param name="target"></param>
        /// <param name="bufferSize"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public static async Task<uint> CopyToAsyncChecksum(this Stream src, Stream target, int bufferSize, CancellationToken token = default)
        {
            uint crc = 0;

            long len = src.Length;
            byte[] buffer = ArrayPool<byte>.Shared.Rent(bufferSize);
            while (len > 0)
            {
                int toWrite = (int)Math.Min(len, bufferSize);
                await src.ReadAsync(buffer, 0, buffer.Length, token);
                await target.WriteAsync(buffer, 0, toWrite, token);

                crc = CRC32.CRC32_0x04C11DB7(buffer.AsSpan(0, toWrite), crc);
            }

            return crc;
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

        public static int MemoryCompare(this Span<byte> src, ReadOnlySpan<byte> input, int size)
        {
            if (size < 0)
                throw new ArgumentException("Size must not be below 0.", nameof(size));

            if (size >= src.Length)
                throw new ArgumentException("Size must not above source length.", nameof(size));

            if (size > src.Length)
                throw new ArgumentException("Size must not above input length.", nameof(size));

            for (int i = 0; i < size; i++)
            {
                if (src[i] != input[i])
                {
                    if (src[i] < input[i])
                        return -1;
                    else
                        return 1;

                }
            }

            return 0;
        }

        public static int CountCharOccurenceFromIndex(this string source, int index, char toCheck)
        {
            if (index > source.Length)
                return 0;

            int c = 0;
            for (int i = index; i < source.Length; i++)
            {
                if (source[i] == toCheck)
                    c++;
            }

            return c;
        }
    }
}
