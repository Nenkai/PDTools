using PDTools.Crypto;

using Syroot.BinaryData.Memory;

using System.Security.Cryptography;

using ICSharpCodeInflater = ICSharpCode.SharpZipLib.Zip.Compression.Inflater;

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


            byte[] elfData = inflatedData.Skip(0x104).ToArray();
            using (var hash = SHA512.Create())
            {
                // Used to check against the two 0x80 blobs, but those i don't know how they generate the hash
                var hashedInputBytes = hash.ComputeHash(elfData);

                var test = new BufferReverser();
                test.PositionInt = 1;
                test.InitInternalBuffer(1, false);
                test.OperatingBuffer[0] = test.field_10 != 0 ? 0xFFFEC397 : 0x13C69;

                var hash2r = new BufferReverser();
                hash2r.InitReverse(sha512Hash2);

                var hash1r = new BufferReverser();
                hash1r.InitReverse(sha512Hash1);

                var unkk = new BufferReverser();
                unkk.InitReverse(hashedInputBytes);

                // The 0x80 blobs are obfuscated with some really odd memory operations, negating, bit moving, among other things
                // Spent hours, but no dice

                var holder = new UnkHolder();
                DeobfuscateHashes(holder, new BufReverserHolder(hash1r));
            }

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

        // Ignore this and below
        public static void DeobfuscateHashes(UnkHolder holder, BufReverserHolder hash1In)
        {
            // 0x04 (1) is left empty
            // 0x0c is set
            // 0x14 (2) is hash1In
            // 0x24 (3) is left empty
            // 0x2c (4) is left empty

            BufferReverser rev = new BufferReverser();
            rev.InitInternalBuffer(20, false);

            // 1032120
            for (var i = 0; i < 0x20; i++)
            {
                uint val = hash1In.buf.OperatingBuffer[i] + (i > 0 ? 1u : 0u);
                rev.OperatingBuffer[i] = 0 - val;
            }

            // 1032760
            // 1033330
            // 1032FC8

            // su_1031E18_RotateWeird
            BufferReverser rev2 = new BufferReverser();
            rev2.InitInternalBuffer(20, false);
            rev.OperatingBuffer.CopyTo(rev2.OperatingBuffer.AsSpan());
            uint v4 = 0;
            for (var i = 0; i < 0x20; i++)
            {
                uint src = rev2.OperatingBuffer[i];
                rev2.OperatingBuffer[i] = (rev2.OperatingBuffer[i] << 1) | v4;
                v4 = src >> 31;
            }

            BufferReverser rev3 = new BufferReverser();
            rev3.InitInternalBuffer(20, false);
            for (var i = 0; i < 0x20; i++)
            {
                uint val = rev2.OperatingBuffer[i] + (i > 0 ? 1u : 0u);
                rev3.OperatingBuffer[i] = 0 - val;
            }


            // sub_1033330
            BufferReverser rev4 = new BufferReverser();
            rev4.InitInternalBuffer(20, false);
            rev3.OperatingBuffer.CopyTo(rev4.OperatingBuffer.AsSpan());
            v4 = 0;
            for (var i = 0; i < 0x20; i++)
            {
                uint src = rev4.OperatingBuffer[i];
                rev4.OperatingBuffer[i] = (rev4.OperatingBuffer[i] << 1) | v4;
                v4 = src >> 31;
            }

            // Converts back?
            BufferReverser rev5 = new BufferReverser();
            rev5.InitInternalBuffer(20, false);
            for (var i = 0; i < 0x20; i++)
            {
                uint val = rev3.OperatingBuffer[i] + (i > 0 ? 1u : 0u);
                rev5.OperatingBuffer[i] = rev.OperatingBuffer[i] - val;
            }
    ;

            // Gave up beyond this point
            // all of this is in a loop, severe obfuscation and memory magic going on

        }

        private static void MemcpyInt(Span<uint> dst, Span<uint> src, int length)
        {
            src.Slice(0, length).CopyTo(dst);
        }

        public class UnkHolder
        {
            public BufferReverser buf1;
            public BufferReverser buf2;
            public BufferReverser buf3;
            public BufferReverser buf4;
            public int field_30;
        }

        public class BufReverserHolder
        {
            int empty;
            public BufferReverser buf;

            public BufReverserHolder(BufferReverser rev)
            {
                buf = rev;
            }
        }
    }
}
