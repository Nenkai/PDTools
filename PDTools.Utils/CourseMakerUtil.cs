using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Buffers;

using Syroot.BinaryData;
using Syroot.BinaryData.Core;

using PDTools.Compression;

namespace PDTools.Utils
{
    public class CourseMakerUtil
    {
        private static readonly byte[] k = {
            0x45, 0x32, 0x35, 0x67, 0x65, 0x69, 0x72, 0x45, 0x50, 0x48, 0x70, 0x63, 
            0x34, 0x57, 0x47, 0x32, 0x46, 0x6E, 0x7A, 0x61, 0x63, 0x4D, 0x71, 0x72, 0x75
        };

        public static bool Decrypt(string path)
        {
            byte[] src = File.ReadAllBytes(path);
            

            if (Encoding.ASCII.GetString(src, 0, 6) == "GT6TED")
                return false;
            else if (src.AsSpan(0, 4).SequenceEqual(new byte[] { 0xC5, 0xEE, 0xF7, 0xFF }))
            {
                src = PS2ZIP.Inflate(src);
                File.WriteAllBytes(path, src);
                return true;
            }
            else
            {
                int j = 1;
                for (int i = 0; i < src.Length; i++)
                {
                    src[i] ^= k[j++ - 1];
                    if (j > k.Length)
                        j = 1;
                }

                if (src.AsSpan(0, 4).SequenceEqual(new byte[] { 0xC5, 0xEE, 0xF7, 0xFF }))
                    src = PS2ZIP.Inflate(src);

                File.WriteAllBytes(path, src);
                return true;
            }
        }

        public static bool Encrypt(string path)
        {
            byte[] src = File.ReadAllBytes(path);

            if (Encoding.ASCII.GetString(src, 0, 6) != "GT6TED")
                return false;
            else
            {
                src = PS2ZIP.Deflate(src);
                int j = 1;
                for (int i = 0; i < src.Length; i++)
                {
                    src[i] ^= k[j++ - 1];
                    if (j > k.Length)
                        j = 1;
                }

                File.WriteAllBytes(path, src);
                return true;
            }
        }
    }
}
