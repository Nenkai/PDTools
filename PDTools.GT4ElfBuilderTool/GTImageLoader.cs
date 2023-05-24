using PDTools.Crypto;

using Syroot.BinaryData.Memory;

using System.IO;
using System.Security.Cryptography;

using System.Numerics;
using System.Runtime.InteropServices;

using ICSharpCodeInflater = ICSharpCode.SharpZipLib.Zip.Compression.Inflater;
using System.Collections.Generic;
using System;

namespace PDTools.GT4ElfBuilderTool
{
    public class GTImageLoader
    {
        /* 0x00 - 8 bytes IV
         * 0x08 - Unk byte
         * 0x09 - Unk byte
         * 0x10 - Raw Size 
         */

        public int EntryPoint { get; set; }
        public List<ElfSegment> Segments { get; set; }

        // Mainly intended/implemented for GT4 Online's CORE.GT4 as it has some extra encryption
        public void Load(byte[] file)
        {
            SpanReader sr;
            if (file[0] != 1 && file[1] != 1)
            {
                // GT4O - Encryption on-top of it
                Console.WriteLine("Executable appears to be encrypted - Assuming GT4 Online");

                // Crc of the file is at the end
                uint crcFile = CRC32C.crc32(file.AsSpan(0), file.Length - 4);
                Console.WriteLine($"CRC: {crcFile:X8}");

                sr = new SpanReader(file);
                sr.Position = file.Length - 4;
                uint crcTarget = sr.ReadUInt32();

                if (crcFile != crcTarget)
                {
                    Console.WriteLine("CRC did not match");
                    return;
                }

                // Begin decrypt
                sr.Position = 0;
                byte[] iv = sr.ReadBytes(8);
                byte[] key = GetKey();
                var s = new Salsa20(key, key.Length);
                s.SetIV(iv);
                s.Decrypt(file.AsSpan(8), file.Length - 12);
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

            int deflatedSize = file.Length - (8 + 12); // IV + Header + CRC at the bottom
            byte[] deflateData = sr.ReadBytes(deflatedSize);
            byte[] inflatedData = new byte[rawSize];

            ICSharpCodeInflater d = new ICSharpCodeInflater(true);
            d.SetInput(deflateData);
            d.Inflate(inflatedData);

            // Read Header
            SpanReader sr2 = new SpanReader(inflatedData);
            short sha512Hash1Len = sr2.ReadInt16();
            byte[] sha512Hash1 = sr2.ReadBytes(sha512Hash1Len);

            short sha512Hash2Len = sr2.ReadInt16();
            byte[] sha512Hash2 = sr2.ReadBytes(sha512Hash2Len);

            int nSection = sr2.ReadInt32();
            EntryPoint = sr2.ReadInt32();
            Segments = new List<ElfSegment>(nSection);

            for (int i = 0; i < nSection; i++)
            {
                var segment = new ElfSegment();
                segment.TargetOffset = sr2.ReadInt32();
                segment.Size = sr2.ReadInt32();
                segment.Data = sr2.ReadBytes(segment.Size);

                Segments.Add(segment);
            }

            byte[] elfData = inflatedData.Skip(0x104).ToArray();
            using (var hash = SHA512.Create())
            {
                // Used to check against the two 0x80 blobs, but those i don't know how they generate the hash
                var hashedInputBytes = hash.ComputeHash(elfData);
                RSAStuff(sha512Hash1, sha512Hash2, 81001);
            }

        }

        static void RSAStuff(byte[] number1, byte[] number2, int primeNumber)
        {
            var hash2r = new PDIBigNumber();
            hash2r.InitFromBuffer(number2);

            var hash1r = new PDIBigNumber();
            hash1r.InitFromBuffer(number1);

            // Part 1 - 0x1030428 (Init RSA?)
            byte[] reversedHash1 = MemoryMarshal.Cast<uint, byte>(hash1r.OperatingBufferPtr_0x14).ToArray();
            BigInteger hashBigNumber1 = new BigInteger(reversedHash1, isUnsigned: true);

            BigInteger baseVal = new BigInteger(1);
            baseVal <<= (0x400);

            var subtracted = baseVal - hashBigNumber1;
            var gcd1 = Egcd(subtracted, hashBigNumber1).LeftFactor; // 1032760
            var gcd1_alt = Egcd(subtracted, hashBigNumber1).LeftFactor + baseVal; // 1032760
            var gcd2 = -Egcd(hashBigNumber1, baseVal).LeftFactor; // 1032760
            var gcd2_alt = Egcd(hashBigNumber1, baseVal).LeftFactor + baseVal;

            // Part 2 - 0x10316A8 (Perform RSA?)
            byte[] reversedHash2 = MemoryMarshal.Cast<uint, byte>(hash2r.OperatingBufferPtr_0x14).ToArray();
            BigInteger hashBigNumber2 = new BigInteger(reversedHash2, isUnsigned: true);

            var multiplied = baseVal * hashBigNumber2;
            var modHash1And2 = multiplied % hashBigNumber1;

            // 1030B98
            var prime = new BigInteger(0x13C69);
            long numBits = prime.GetBitLength();
            for (var i = 0; i < numBits; i++)
            {
                if (((prime >> i) & 1) != 0)
                {
                    sub_1030B98(/* context, output, */ modHash1And2)
                    ;
                }
            }

            void sub_1030B98(BigInteger unk)
            {
                var v1 = truncate((subtracted * modHash1And2), 0x100);
                var v2 = truncate(v1 * gcd2, 0x80); /* ?? */
                var v3 = truncate(v2 * hashBigNumber1, 0x100);

                var v4 = v3 + v1;   
            }


        }

        public static BigInteger truncate(BigInteger big, int size)
        {
            var arr = big.ToByteArray(isUnsigned: big.Sign >= 0).AsSpan(0, size);
            return new BigInteger(arr, big.Sign >= 0);
        }

        public static (BigInteger LeftFactor,
               BigInteger RightFactor,
               BigInteger Gcd) Egcd(BigInteger left, BigInteger right)
        {
            BigInteger leftFactor = 0;
            BigInteger rightFactor = 1;

            BigInteger u = 1;
            BigInteger v = 0;
            BigInteger gcd = 0;

            while (left != 0)
            {
                BigInteger q = right / left;
                BigInteger r = right % left;

                BigInteger m = leftFactor - u * q;
                BigInteger n = rightFactor - v * q;

                right = left;
                left = r;
                leftFactor = u;
                rightFactor = v;
                u = m;
                v = n;

                gcd = right;
            }

            return (LeftFactor: leftFactor,
                    RightFactor: rightFactor,
                    Gcd: gcd);
        }

        public void Build(string outputFileName)
        {
            Console.WriteLine("Building ELF file...");
            ElfBuilder elfBuilder = new ElfBuilder();
            elfBuilder.BuildFromInfo(outputFileName, this);
        }

        private static readonly byte[] k = new byte[16]
        {
            // "PolyphonyDigital"
            0x05, 0x3A, 0x39, 0x2C, 0x25, 0x3D, 0x3A, 0x3B,
            0x2C, 0x11, 0x3C, 0x32, 0x3C, 0x21, 0x34, 0x39,
        };

        private static byte[] GetKey()
        {
            byte[] key = new byte[16];
            for (int i = 0; i < 16; i++)
                key[i] = (byte)(k[i] ^ 0x55);
            return key;
        }

        static void Part1(PDIBigNumber hash1)
        {
            PDIBigNumber buf = new PDIBigNumber();
            buf.ResizeBuffer_10300C8(0x40, true);
            buf.CurrentLength_0x08 = 0x21;
            //buf.Size

            CreateContext(hash1);

            CopyObfuscate_1032120(buf, hash1);


        }

        static void CreateContext(PDIBigNumber hashBuffer)
        {
            var initialBuf = new PDIBigNumber();
            initialBuf.CurrentLength_0x08 = 1;
            initialBuf.ResizeBuffer_10300C8(1, false);
            initialBuf.OperatingBufferPtr_0x14[0] = 1; // Start with a single toggled bit

            int bitCounter = 0;
            while (true)
            {
                int compareResult = 0;

                bool initialBufIsEmpty = false;
                if (initialBuf is null || initialBuf.CurrentLength_0x08 == 0)
                    initialBufIsEmpty = true;

                if (initialBufIsEmpty)
                {

                }
                else
                {
                    bool hashBufIsEmpty = false;
                    if (hashBuffer is null || hashBuffer.CurrentLength_0x08 == 0)
                        hashBufIsEmpty = true;

                    if (hashBufIsEmpty)
                    {
                        // TODO
                    }
                    else
                    {
                        compareResult = PDIBigNumber.Equals_102FF50(initialBuf, hashBuffer);
                    }
                }

                if (compareResult >= 0)
                    break;

                initialBuf.RotateLeft_1031E18();
                bitCounter++;
            }

            // Copy
            if (hashBuffer != null)
                CopyObfuscate_1032120(initialBuf, hashBuffer);

            sub_1032760(null, initialBuf, hashBuffer);
            ;

        }

        static void sub_1032760(PDIBigNumber a1, PDIBigNumber a2, PDIBigNumber a3)
        {
            var initialBuf = new PDIBigNumber();
            initialBuf.CurrentLength_0x08 = 1;
            initialBuf.ResizeBuffer_10300C8(1, false);
            initialBuf.OperatingBufferPtr_0x14[0] = 1; // Start with a single toggled bit

            while (true)
            {
                sub_1033330(null, a3, a2, null);
            }

            
        }

        static void sub_1033330(PDIBigNumber ret1, PDIBigNumber a2, PDIBigNumber a3, PDIBigNumber ret2)
        {
            if (a2 != null)
            {

            }
        }

        static void CopyObfuscate_1032120(PDIBigNumber one, PDIBigNumber two)
        {
            if (one.field_10 == two.field_10)
            {
                if (PDIBigNumber.CompareBuffers_102FED0(two, one) > 0)
                {

                }
                else
                {
                    SubtractBuffers_1032050(one, two);
                }
            }
        }

        static void SubtractBuffers_1032050(PDIBigNumber left, PDIBigNumber right)
        {
            byte flag = 0;
            if (left.CurrentLength_0x08 > 0)
            {
                for (var i = 0; i < left.CurrentLength_0x08; i++)
                {
                    uint rightVal = (i < right.CurrentLength_0x08 ? right.OperatingBufferPtr_0x14[i] : 0);
                    uint rightValPlusFlag = rightVal + flag;

                    if (rightValPlusFlag >= flag)
                    {
                        uint leftVal = (i < left.CurrentLength_0x08 ? left.OperatingBufferPtr_0x14[i] : 0);
                        uint newValue = leftVal - rightValPlusFlag;
                        left.InsertValue_1030198(i, newValue);
                    }
                }
            }
        }

        public class UnkHolder
        {
            public PDIBigNumber buf1;
            public PDIBigNumber buf2;
            public PDIBigNumber buf3;
            public PDIBigNumber buf4;
            public int field_30;
        }

        public class BufReverserHolder
        {
            int empty;
            public PDIBigNumber buf;

            public BufReverserHolder(PDIBigNumber rev)
            {
                buf = rev;
            }
        }
    }
}
