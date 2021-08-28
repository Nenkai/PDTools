using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IO;
using System.IO.Compression;
using System.Buffers.Binary;

using Syroot.BinaryData;

using ICSharpCodeDeflater = ICSharpCode.SharpZipLib.Zip.Compression.Deflater;

namespace PDTools.Compression
{
    public class PS2ZIP
    {
        public const uint PS2ZIP_MAGIC = 0xFF_F7_EE_C5;

        /// <summary>
        /// Checks if compression is valid for the buffer.
        /// </summary>
        /// <param name="data"></param>
        /// <param name="outSize"></param>
        /// <returns></returns>
        public unsafe static bool CheckCompression(Span<byte> data, ulong outSize)
        {
            if (outSize > uint.MaxValue)
                return false;

            // Inflated is always little
            uint zlibMagic = BinaryPrimitives.ReadUInt32LittleEndian(data);
            uint sizeComplement = BinaryPrimitives.ReadUInt32LittleEndian(data[4..]);

            if ((long)zlibMagic != PS2ZIP_MAGIC)
                return false;

            if ((uint)outSize + sizeComplement != 0)
                return false;

            const int headerSize = 8;
            if (data.Length <= headerSize) // Header size, if it's under, data is missing
                return false;

            return true;
        }

        /// <summary>
        /// Safely decompress a file in a stream and saves it to the provided path.
        /// </summary>
        public static bool TryInflate(Stream input, Stream output, int maxDecompressedSize = -1, bool closeStream = true)
        {
            if (input.Length <= 8)
                return false;

            var bs = new BinaryStream(input);
          
            if (bs.ReadUInt32() != PS2ZIP_MAGIC)
                return false;

            int sizeComplement = -bs.ReadInt32();
            if (maxDecompressedSize != -1 && sizeComplement > maxDecompressedSize)
                return false;

            try
            {
                var ds = new DeflateStream(bs, CompressionMode.Decompress);
                ds.CopyTo(output);
            }
            catch
            {
                return false;
            }

            if (output.Length != sizeComplement)
                return false;

            return true;
        }

        /// <summary>
        /// Safely decompress a file in a stream and saves it to the provided path asynchronously.
        /// </summary>
        public static async Task<bool> TryInflateAsync(Stream input, Stream output, int maxDecompressedSize = -1, bool closeStream = true)
        {
            if (input.Length <= 8)
                return false;

            var bs = new BinaryStream(input);

            if (await bs.ReadUInt32Async() != PS2ZIP_MAGIC)
                return false;

            int sizeComplement = -(await bs.ReadInt32Async());
            if (maxDecompressedSize != -1 && sizeComplement > maxDecompressedSize)
                return false;

            try
            {
                var ds = new DeflateStream(bs, CompressionMode.Decompress);
                await ds.CopyToAsync(output);
            }
            catch
            {
                return false;
            }

            if (output.Length != sizeComplement)
                return false;

            return true;
        }

        public static byte[] Deflate(byte[] input)
        {
            using (var ms = new MemoryStream(input.Length))
            using (var bs = new BinaryStream(ms))
            {
                bs.WriteUInt32(0xFFF7EEC5);
                bs.WriteInt32(-input.Length);

                var d = new ICSharpCodeDeflater(ICSharpCodeDeflater.DEFAULT_COMPRESSION, true);
                d.SetInput(input);
                d.Finish();

                byte[] output = new byte[input.Length];
                int count = d.Deflate(output);
                bs.Write(output, 0, count);
                return ms.ToArray();
            }
        }

        public static byte[] Inflate(byte[] data)
        {
            using (var ms = new MemoryStream(data))
            using (var bs = new BinaryStream(ms))
            {
                bs.ReadUInt32(); // Magic
                int outSize = -bs.ReadInt32();

                byte[] deflatedData = new byte[outSize];
                using (var ds = new DeflateStream(bs, CompressionMode.Decompress))
                    ds.Read(deflatedData, 0, deflatedData.Length);

                return deflatedData;
            }
        }

        /// <summary>
        /// Decompresses a file (in memory, unsuited for large files).
        /// </summary>
        /// <param name="data"></param>
        /// <param name="outSize"></param>
        /// <param name="deflatedData"></param>
        /// <returns></returns>
        public unsafe static bool TryInflateInMemory(Span<byte> data, ulong outSize, out byte[] deflatedData)
        {
            deflatedData = Array.Empty<byte>();
            if (outSize > uint.MaxValue)
                return false;

            // Inflated is always little
            uint zlibMagic = BinaryPrimitives.ReadUInt32LittleEndian(data);
            uint sizeComplement = BinaryPrimitives.ReadUInt32LittleEndian(data[4..]);

            if ((long)zlibMagic != PS2ZIP_MAGIC)
                return false;

            if ((uint)outSize + sizeComplement != 0)
                return false;

            const int headerSize = 8;
            if (data.Length <= headerSize) // Header size, if it's under, data is missing
                return false;

            deflatedData = new byte[(int)outSize];
            fixed (byte* pBuffer = &data.Slice(headerSize)[0]) // Vol Header Size
            {
                using var ums = new UnmanagedMemoryStream(pBuffer, data.Length - headerSize);
                using var ds = new DeflateStream(ums, CompressionMode.Decompress);
                ds.Read(deflatedData, 0, (int)outSize);
            }

            return true;
        }
    }
}
