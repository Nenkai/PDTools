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
            sr.ReadByte();
            sr.ReadByte();
            int rawSize = sr.ReadInt32();

            int deflatedSize = file.Length - (8 + 12); // IV + Header + CRC at the bottom
            byte[] deflateData = sr.ReadBytes(deflatedSize);
            byte[] inflatedData = new byte[rawSize];

            ICSharpCodeInflater d = new ICSharpCodeInflater(true);
            d.SetInput(deflateData);
            d.Inflate(inflatedData);

            // Read Header
            SpanReader sr2 = new SpanReader(inflatedData);
            short header1Size = sr2.ReadInt16();
            byte[] header1 = sr2.ReadBytes(header1Size);

            short header2Size = sr2.ReadInt16();
            byte[] header2 = sr2.ReadBytes(header2Size);

            // Optionally check sha but this needs more investigation
            var unk = new BufferReverser();
            unk.InitReverse(header1);

            byte[] elfData = inflatedData.Skip(0x104).ToArray();
            using (var hash = SHA512.Create())
            {
                // Used to check against the two 0x80 blobs, but those i don't know how they generate the hash
                var hashedInputBytes = hash.ComputeHash(elfData);
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
    }
}
