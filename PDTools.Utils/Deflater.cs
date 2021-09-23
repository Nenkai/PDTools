using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IO;
using System.IO.Compression;

using Syroot.BinaryData;

namespace Grimoire.Utils
{
    public class Deflater
    {
        public const uint CompressMagic = 0xFF_F7_EE_C5;

        /// <summary>
        /// Safely decompress a file in a stream and saves it to the provided path.
        /// </summary>
        public static bool TryDeflate(Stream input, Stream output, int maxDecompressedSize = -1, bool closeStream = true)
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
        public static async Task<bool> TryDeflateAsync(Stream input, Stream output, int maxDecompressedSize = -1, bool closeStream = true)
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
    }
}
