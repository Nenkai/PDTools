using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IO;
using System.IO.Compression;

using Syroot.BinaryData;

using ICSharpCodeDeflater = ICSharpCode.SharpZipLib.Zip.Compression.Deflater;

namespace PDTools.Compression
{
    public class PS2ZIP
    {
        public const uint CompressMagic = 0xFF_F7_EE_C5;

        /// <summary>
        /// Safely decompress a file in a stream and saves it to the provided path.
        /// </summary>
        public static bool TryInflate(Stream input, Stream output, int maxDecompressedSize = -1, bool closeStream = true)
        {
            if (input.Length <= 8)
                return false;

            var bs = new BinaryStream(input);
          
            if (bs.ReadUInt32() != CompressMagic)
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

            if (await bs.ReadUInt32Async() != CompressMagic)
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
    }
}
