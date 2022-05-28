using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Buffers;

using Syroot.BinaryData;
using System.IO;
using ImpromptuNinjas.ZStd;

namespace PDTools.Compression
{
    public class ZStdZIP
    {
        public const uint ZSTDTiny_Magic = 0xFFF7ED85;
        public const uint ZSTDStandard_Magic = 0xFFF7972F;

        public static void DecompressZStd_Tiny(Stream stream, Stream outputStream, bool isFragmented)
        {
            int uncompressed_size = -stream.ReadInt32();

            if (!isFragmented)
            {
                const int bufferSize = 0x20000;
                using (var decompressor = new ZStdDecompressStream(stream, bufferSize))
                {
                    var rem = uncompressed_size;
                    byte[] buffer = ArrayPool<byte>.Shared.Rent(bufferSize);
                    while (rem > 0)
                    {
                        int len = (int)Math.Min((long)rem, buffer.Length);
                        var read = decompressor.Read(buffer, 0, len);
                        outputStream.Write(buffer, 0, read);

                        rem -= read;
                    }

                    ArrayPool<byte>.Shared.Return(buffer);
                }
            }
            else
            {
                using (var decompressor = new ZStdDecompressor())
                {
                    while (true)
                    {
                        uint magic = stream.ReadUInt32();
                        if (magic != 0xFFF7F32E)
                            break;

                        int chunk_uncompressed_size = stream.ReadInt32();
                        int chunk_compressed_size = stream.ReadInt32();
                        uint crc_checksum = stream.ReadUInt32();

                        byte[] chunkBuffer = ArrayPool<byte>.Shared.Rent(chunk_uncompressed_size);
                        byte[] inputBuffer = ArrayPool<byte>.Shared.Rent(chunk_compressed_size);
                        stream.Read(inputBuffer.AsSpan(0, chunk_compressed_size));

                        decompressor.Decompress(
                             chunkBuffer.AsSpan(0, chunk_uncompressed_size),
                             inputBuffer.AsSpan(0, chunk_compressed_size)
                             );

                        outputStream.Write(chunkBuffer.AsSpan(0, chunk_uncompressed_size));

                        stream.Align(0x10000);

                        ArrayPool<byte>.Shared.Return(chunkBuffer);
                        ArrayPool<byte>.Shared.Return(inputBuffer);
                    }
                }
            }
        }

        public static void DecompressZStd_Standard(Stream stream, Stream outputStream, bool isFragmented)
        {
            uint uncompressed_size_lo = stream.ReadUInt32();
            uint uncompressed_size_hi = stream.ReadUInt16();
            ulong uncompressed_size = (ulong)uncompressed_size_hi << 32 | uncompressed_size_lo;

            uint compressed_size_lo = stream.ReadUInt32();
            uint compressed_size_hi = stream.ReadUInt16();
            ulong compressed_size = (ulong)compressed_size_hi << 32 | compressed_size_lo;

            stream.Position += 0x0C;

            int flags = stream.ReadInt32();
            if ((flags & 1) != 0)
                isFragmented = true;

            if (!isFragmented)
            {
                const int bufferSize = 0x20000;
                using (var decompressor = new ZStdDecompressStream(stream, bufferSize))
                {
                    var rem = uncompressed_size;
                    byte[] buffer = ArrayPool<byte>.Shared.Rent(bufferSize);

                    while (rem > 0)
                    {
                        int len = (int)Math.Min((long)rem, buffer.Length);

                        var read = decompressor.Read(buffer, 0, len);
                        outputStream.Write(buffer, 0, read);

                        rem -= (uint)read;
                    }

                    ArrayPool<byte>.Shared.Return(buffer);
                }
            }
            else
            {
                using (var decompressor = new ZStdDecompressor())
                {
                    while (true)
                    {
                        uint magic = stream.ReadUInt32();
                        if (magic != 0xFFF7F32E)
                            break;

                        int chunk_uncompressed_size = stream.ReadInt32();
                        int chunk_compressed_size = stream.ReadInt32();
                        uint crc_checksum = stream.ReadUInt32();

                        byte[] chunkBuffer = ArrayPool<byte>.Shared.Rent(chunk_uncompressed_size);
                        byte[] inputBuffer = ArrayPool<byte>.Shared.Rent(chunk_compressed_size);

                        stream.Read(inputBuffer.AsSpan(0, chunk_compressed_size));

                        decompressor.Decompress(
                            chunkBuffer.AsSpan(0, chunk_uncompressed_size),
                            inputBuffer.AsSpan(0, chunk_compressed_size)
                            );

                        outputStream.Write(chunkBuffer.AsSpan(0, chunk_uncompressed_size));

                        stream.Align(0x10000);

                        ArrayPool<byte>.Shared.Return(chunkBuffer);
                        ArrayPool<byte>.Shared.Return(inputBuffer);
                    }
                }
            }
        }
    }
}
