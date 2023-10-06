using Syroot.BinaryData.Memory;
using System.IO;
using System.Security.Cryptography;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using System;
using System.Linq;

using ICSharpCodeInflater = ICSharpCode.SharpZipLib.Zip.Compression.Inflater;

using PDTools.Crypto;
using PDTools.Hashing;

namespace PDTools.GT4ElfBuilderTool
{
    public class GTImageLoader
    {
        /// <summary>
        /// Used in GT4O, encrypted body
        /// </summary>
        private static readonly byte[] k = new byte[16]
        {
            // "PolyphonyDigital" when decrypted
            0x05, 0x3A, 0x39, 0x2C, 0x25, 0x3D, 0x3A, 0x3B,
            0x2C, 0x11, 0x3C, 0x32, 0x3C, 0x21, 0x34, 0x39,
        };


        public int EntryPoint { get; set; }
        public List<ElfSegment> Segments { get; set; }

        // Mainly intended/implemented for GT4 Online's CORE.GT4 as it has some extra encryption
        public bool Load(byte[] file)
        {
            byte[] inflated = ProcessFileHeaderAndDecompress(file);
            if (inflated is null)
                return false;

            Console.WriteLine("# Step 2: Read actual elf relocation header");
            // Read Header
            SpanReader sr2 = new SpanReader(inflated);
            short rsaValue1Length = sr2.ReadInt16();
            byte[] rsaValueToGenerateSha512Hash_1 = sr2.ReadBytes(rsaValue1Length);

            short rsaValue2Length = sr2.ReadInt16();
            byte[] rsaValueToGenerateSha512Hash_2 = sr2.ReadBytes(rsaValue2Length);

            int hashStartPos = sr2.Position;
            int nSection = sr2.ReadInt32();
            EntryPoint = sr2.ReadInt32();
            Segments = new List<ElfSegment>(nSection);

            Console.WriteLine("----");
            Console.WriteLine($"- RSA Value 1 (0x{rsaValue1Length:X4}): {Convert.ToHexString(rsaValueToGenerateSha512Hash_1)}");
            Console.WriteLine($"- RSA Value 2 (0x{rsaValue2Length:X4}): {Convert.ToHexString(rsaValueToGenerateSha512Hash_2)}");
            Console.WriteLine($"- Number of Sections: {nSection}");
            Console.WriteLine($"- Entrypoint: 0x{EntryPoint:X8}");
            Console.WriteLine("----");

            for (int i = 0; i < nSection; i++)
            {
                if (i == 1)
                    Console.WriteLine($"# Segment {i + 1} (likely .text)");
                else if (i == 2)
                    Console.WriteLine($"# Segment {i + 1} (likely .data)");
                else
                    Console.WriteLine($"# Segment {i + 1}");

                var segment = new ElfSegment();
                segment.TargetOffset = sr2.ReadInt32();
                segment.Size = sr2.ReadInt32();
                segment.Data = sr2.ReadBytes(segment.Size);

                Console.WriteLine($"- Target Memory Offset: 0x{segment.TargetOffset:X8}");
                Console.WriteLine($"- Size: 0x{segment.Size:X8}");
                Console.WriteLine("----");

                Segments.Add(segment);
            }


            Console.WriteLine();
            Console.WriteLine("# Step 3: Attempt optional authentication by computing RSA numbers to a SHA-512 hash");


            // Values are reversed
            // Doesn't always work i.e GT4P, refer to Egdc add operation comment, and the minus on -Egdc
            int authValue = AuthenticateELFBody(rsaValueToGenerateSha512Hash_1.Reverse().ToArray(), 
                                                rsaValueToGenerateSha512Hash_2.Reverse().ToArray(), 
                                                inflated.AsSpan(hashStartPos));

            /* This was a test on GT4 EU, couldn't get it to work though..
            using (RSACryptoServiceProvider RSA = new RSACryptoServiceProvider(1024))
            {
                var @params = new RSAParameters()
                {
                    Exponent = new BigInteger(82201).ToByteArray(),
                    Modulus = rsaValueToGenerateSha512Hash_1,
                };
                RSA.ImportParameters(@params);

                if (RSA.VerifyData(inflated.AsSpan(hashStartPos), rsaValueToGenerateSha512Hash_2, HashAlgorithmName.SHA512, RSASignaturePadding.Pkcs1))
                {
                    ;
                }
            };*/

            if (authValue != -1)
                Console.WriteLine($"ELF SHA-512 Matches using provided RSA numbers! Build specific value used: {authValue} ({KnownExponents[authValue]})");
            else
                Console.WriteLine("Could not authenticate/verify executable with RSA computed/encrypted SHA-512 hash");

            Console.WriteLine("Done loading CORE file.");
            Console.WriteLine();
            return true;
        }

        public void BuildELF(string outputFileName)
        {
            Console.WriteLine("Building ELF file...");
            ElfBuilder elfBuilder = new ElfBuilder();
            elfBuilder.BuildFromInfo(outputFileName, this);
        }

        private static byte[] ProcessFileHeaderAndDecompress(byte[] file)
        {
            SpanReader sr;

            Console.WriteLine("# Step 1: Processing CORE file header");

            bool enc = file[0] != 1 && file[1] != 1;
            if (enc)
            {
                // GT4O - Encryption on-top of it
                Console.WriteLine("Executable appears to be encrypted - Assuming GT4 Online");

                // Crc of the file is at the end
                uint crcFile = CRC32C.crc32(file.AsSpan(0), file.Length - 4);
                Console.WriteLine($"CRC of encrypted body: 0x{crcFile:X8}");

                sr = new SpanReader(file);
                sr.Position = file.Length - 4;
                uint crcTarget = sr.ReadUInt32();

                if (crcFile != crcTarget)
                {
                    Console.WriteLine("CRC did not match");
                    return null;
                }

                // Begin decrypt
                sr.Position = 0;
                byte[] iv = sr.ReadBytes(8);
                byte[] key = GetKey();

                Console.WriteLine($"Decrypting with Salsa IV: {Convert.ToHexString(iv)}..");
                var s = new Salsa20(key, key.Length);
                s.SetIV(iv);
                s.Decrypt(file.AsSpan(8), file.Length - 12);
                Console.WriteLine("Decrypted.");
            }
            else
            {
                sr = new SpanReader(file);
            }

            // Decompress part - Entering compression header 

            // These flags are priority or index of the source to use for CORE.GT4
            // In order is disk, mem card 1, mem card 2, host
            ushort loadSourceFlags = (ushort)(sr.ReadByte() | (ushort)(sr.ReadByte() << 8));
            int rawSize = sr.ReadInt32();

            Console.WriteLine($"Load Source Flags: 0x{loadSourceFlags:X4}");
            Console.WriteLine($"Decompressed Size: 0x{rawSize:X8}");

            const int HeaderSize = 0x06;
            int deflatedSize = file.Length - (HeaderSize + (enc ? 8 + 4 : 0)); // Header (6) + IV (8) + CRC at the bottom (4)

            Console.WriteLine("Decompressing..");
            byte[] deflateData = sr.ReadBytes(deflatedSize);
            byte[] inflatedData = new byte[rawSize];

            ICSharpCodeInflater d = new ICSharpCodeInflater(true);
            d.SetInput(deflateData);
            d.Inflate(inflatedData);
            Console.WriteLine("Decompressed.");
            Console.WriteLine();

            return inflatedData;
        }

        public static Dictionary<int, string> KnownExponents = new Dictionary<int, string>
        {
            { 65537, "GT3 (JP)" },
            { 66001, "GT3 (US)" },
            { 67001, "GT3 (EU)" },

            { 69001, "GT Concept 2001 (JP)" },
            { 71003, "GT Concept 2002 Tokyo-Geneva (EU)" },
            { 72001, "GT Concept 2002 Tokyo-Geneva (EU)" },

            { 77001, "GT4P (JP)" },
            { 78001, "GT4P (AS)" },
            { 79001, "GT4P (KR)" },
            { 80001, "GT4P (EU)" },

            { 81001, "GT4O (US)" },

            { 82001, "GT4 (JP)" },
            { 82101, "GT4 (US)" },
            { 82201, "GT4 (EU)" },
            { 82301, "GT4 (AS)" },
            { 82401, "GT4 (KR)" },

            { 82501, "GT4 Press Copy (CN)" },
 
            { 83201, "GT4O (JP)" }, // GT4O (JP)

            { 90001, "Tourist Trophy (JP)" },
            { 90301, "Tourist Trophy (US)" },
            { 90401, "Tourist Trophy (EU)" },
        };

        private static int AuthenticateELFBody(byte[] value1, byte[] value2, Span<byte> elfBody)
        {
            using (var computedHash = SHA512.Create())
            {
                var hashedInputBytes = computedHash.ComputeHash(elfBody.ToArray());
                Console.WriteLine($"Expected SHA-512 hash is {Convert.ToHexString(hashedInputBytes)}");
                foreach (var exponent in KnownExponents)
                {
                    var rsaCtx = new RSAContext();
                    rsaCtx.InitMaybe_0x1030428(value1, value2);
                    var expectedNumber = rsaCtx.ComputeOrDecrypt_1030FE8(exponent.Key);

                    // Reverse it
                    var expectedHash = expectedNumber.ToByteArray().AsSpan(0, 0x40);
                    expectedHash.Reverse();

                    if (hashedInputBytes.AsSpan().SequenceEqual(expectedHash))
                        return exponent.Key;
                }
            }

            return -1;
        }

        /// <summary>
        /// Decrypts the key with a cheap xor/bit flip (0x55)
        /// </summary>
        /// <returns></returns>
        private static byte[] GetKey()
        {
            byte[] key = new byte[16];
            for (int i = 0; i < 16; i++)
                key[i] = (byte)(k[i] ^ 0x55);
            return key;
        }
    }
}
