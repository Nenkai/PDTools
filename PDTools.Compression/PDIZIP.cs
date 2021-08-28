using Syroot.BinaryData;
using System;
using System.Buffers;
using System.IO;
using System.IO.Compression;
using System.Numerics;
using System.Security.Cryptography;

using ICSharpCode.SharpZipLib.Zip.Compression;

namespace PDTools.Compression
{
    public class PDIZIP
    {
        public const uint PDIZIP_MAGIC = 0xFF_F7_F3_2F;

        public static bool Inflate(Stream input, Stream output, bool skipMagic = false)
        {
            if (!skipMagic && input.ReadUInt32() == PDIZIP_MAGIC)
                return false;

            // Read PDIZIP Header
            uint expand_size = input.ReadUInt32();
            uint compressed_size = input.ReadUInt32();
            uint fragment_size = input.ReadUInt32();
            int method = input.ReadInt32();
            input.ReadInt32s(3);

            // Calculate the fragment (or chunk) count for the file
            uint fragmentCount = (compressed_size + fragment_size - 1) / fragment_size;

            // Set up inflater and buffers
            Inflater inflater = new Inflater(true);
            byte[] fragment_buffer = ArrayPool<byte>.Shared.Rent((int)fragment_size);
            byte[] expand_buffer = ArrayPool<byte>.Shared.Rent(0x20000);

            for (uint i = 0; i < fragmentCount; i++)
            {
                if (input.ReadUInt32() != PDIZIP_MAGIC)
                    throw new Exception("PDIZip Fragment magic did not match expected magic.");

                // Read Fragment header
                uint fragmentExpandSize = input.ReadUInt32();
                uint fragmentCompressedSize = input.ReadUInt32();
                uint Unk = input.ReadUInt32();

                // Only way to do it properly apparently, inflate streams read too much data
                input.Read(fragment_buffer, 0, (int)fragmentCompressedSize);
                inflater.SetInput(fragment_buffer, 0, (int)fragmentCompressedSize);

                while (!inflater.IsFinished)
                {
                    int inflated = inflater.Inflate(expand_buffer);
                    output.Write(expand_buffer, 0, inflated);
                }

                // Crypto stream can't seek, so just manually move..
                int left = (int)(fragment_size - fragmentCompressedSize) - 0x10;
                if (i == 0)
                    left -= 0x20;
                while (left-- > 0) input.ReadByte();

                inflater.Reset(); // Prepare for next chunk
            }

            ArrayPool<byte>.Shared.Return(fragment_buffer);
            ArrayPool<byte>.Shared.Return(expand_buffer);

            return output.Length == expand_size;
        }
    }
}
