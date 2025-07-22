using PDTools.Crypto;

using System.Buffers.Binary;

namespace PDTools.PDIWheelCryptor;

public class Program
{
    static void Main(string[] args)
    {
        if (args.Length != 1)
        {
            Console.WriteLine("Usage: PDTools.PDIWheelCryptor <file to en/decrypt>");
            return;
        }

        if (!File.Exists(args[0]))
        {
            Console.WriteLine($"File '{args[0]}' does not exist.");
            return;
        }

        var file = File.ReadAllBytes(args[0]);

        byte[] key = new byte[]
        {
            0x7D, 0x7D, 0xAF, 0xB4, 0xF8, 0xF5, 0x93, 0x00,
            0xC1, 0xAD, 0xF7, 0xA6, 0xEA, 0x90, 0x1C, 0x92,
            0xFD, 0x4D, 0xC7, 0x15, 0xBF, 0x5C, 0x16, 0x45,
            0xC0, 0xDD, 0x20, 0x9A, 0x20, 0x58, 0x19, 0x29
        };

        byte[] nonce = new byte[12];
        BinaryPrimitives.WriteUInt64LittleEndian(nonce.AsSpan(0), 0x6F8E971ADBF1A8CAL);
        var cha = new ChaCha20(key, nonce, 0);
        cha.DecryptBytes(file, file.Length);

        File.WriteAllBytes(args[0], file);
        Console.WriteLine("Crypted file.");
    }
}
