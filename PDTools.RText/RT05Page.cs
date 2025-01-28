using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System;
using System.Linq;

using Syroot.BinaryData;

using PDTools.Crypto;
using PDTools.Utils;

namespace PDTools.RText;

public class RT05Page : RTextPageBase
{
    public const int EntrySize = 0x10;
    public const int EntrySizeGT7 = 0x18;

    private readonly bool _gt7;

    public RT05Page(bool gt7 = false)
    {
        _gt7 = gt7;
    }

    public override void Read(BinaryStream reader)
    {
        var pageNameOffset = _gt7 ? reader.ReadInt64() : reader.ReadUInt32();
        var pairUnitCount = reader.ReadUInt32();
        reader.ReadUInt32(); // Unk
        var pairUnitOffset = _gt7 ? reader.ReadInt64() : reader.ReadUInt32();

        reader.BaseStream.Position = (int)pageNameOffset;
        Name = reader.ReadString(StringCoding.ZeroTerminated);

        for (int i = 0; i < pairUnitCount; i++)
        {
            reader.BaseStream.Position = pairUnitOffset + (i * (_gt7 ? EntrySizeGT7 : EntrySize));
            int id = reader.ReadInt32();

            ushort labelLen = reader.ReadUInt16();
            ushort valueLen = reader.ReadUInt16();

            long labelOffset = _gt7 ? reader.ReadInt64() : reader.ReadUInt32();
            long valueOffset = _gt7 ? reader.ReadInt64() : reader.ReadUInt32();

            reader.BaseStream.Position = labelOffset;
            string label = ReadString(reader, labelLen);

            reader.BaseStream.Position = valueOffset;
            string value = ReadString(reader, valueLen);

            var pair = new RTextPairUnit(id, label, value);
            PairUnits.Add(label, pair);
        }
    }

    public override void Write(BinaryStream writer, int baseOffset, int baseDataOffset)
    {
        writer.BaseStream.Position = baseDataOffset;
        int baseNameOffset = (int)writer.BaseStream.Position;
        writer.WriteString(Name, StringCoding.ZeroTerminated);
        writer.Align(0x04, grow: true);

        int pairUnitOffset = (int)writer.BaseStream.Position;

        // Proceed to write the string tree, skip the entry map for now
        writer.BaseStream.Position += EntrySize * PairUnits.Count;
        int lastStringPos = (int)writer.BaseStream.Position;

        // Write our strings
        int j = 0;

        // Setup for binary searching - strings are sorted by length, then their encrypted values
        // Override the sorting logic - couldn't find a better way to structure this to make it work across all versions
        // Not the most efficient
        var orderedPairs = PairUnits.Values.OrderBy(
            e => Encrypt(Encoding.UTF8.GetBytes(e.Label), RTextConstants.CRYPTO_KEY.AlignString(0x20)),
            ByteBufferComparer.Default);

        foreach (var pair in orderedPairs)
        {
            writer.BaseStream.Position = lastStringPos;

            int labelOffset = (int)writer.BaseStream.Position;
            var encLabel = Encrypt(Encoding.UTF8.GetBytes(pair.Label), RTextConstants.CRYPTO_KEY.AlignString(0x20));
            writer.WriteBytes(encLabel); writer.WriteByte(0); // Null terminate
            writer.Align(0x04, grow: true);

            int valueOffset = (int)writer.BaseStream.Position;
            var encValue = Encrypt(Encoding.UTF8.GetBytes(pair.Value), RTextConstants.CRYPTO_KEY.AlignString(0x20));
            writer.WriteBytes(encValue); writer.WriteByte(0); // Null terminate
            writer.Align(0x04, grow: true);

            lastStringPos = (int)writer.BaseStream.Position;

            // Write the offsets
            writer.BaseStream.Position = pairUnitOffset + (j * EntrySize);
            writer.Write(pair.ID);
            writer.Write((ushort)(encLabel.Length + 1));
            writer.Write((ushort)(encValue.Length + 1));
            writer.Write(labelOffset);
            writer.Write(valueOffset);

            j++;
        }

        // Finish page toc entry
        writer.BaseStream.Position = baseOffset;
        writer.Write(baseNameOffset);
        writer.Write(PairUnits.Count);
        writer.Write(0); // Unk
        writer.Write(pairUnitOffset);

        // Seek to the end of it
        writer.BaseStream.Position = writer.BaseStream.Length;
    }

    private static string ReadString(BinaryStream reader, ushort length)
    {
        var buffer = reader.ReadBytes(length - 1);

        /* Haven't seen any evidence in the gt6 eboot upon getting a string that this corresponds to a rtext being encrypted */
        if (buffer.Length > 0)
            buffer = Decrypt(buffer, RTextConstants.CRYPTO_KEY.AlignString(0x20));
        return Encoding.UTF8.GetString(buffer);
    }

    static byte[] Decrypt(byte[] encrypted, string key)
    {
        if (encrypted.Length == 0)
            return encrypted;

        using Salsa20SymmetricAlgorithm salsa20 = new Salsa20SymmetricAlgorithm();
        var dataKey = new byte[8];
        var keyBytes = Encoding.UTF8.GetBytes(key);
        byte[] decrypted = new byte[encrypted.Length];
        using (var decrypt = salsa20.CreateDecryptor(keyBytes, dataKey))
            decrypt.TransformBlock(encrypted, 0, encrypted.Length, decrypted, 0);

        return decrypted;
    }

    static byte[] Encrypt(byte[] input, string key)
    {
        if (input.Length == 0)
            return [];

        using Salsa20SymmetricAlgorithm salsa20 = new Salsa20SymmetricAlgorithm();
        var dataKey = new byte[8];
        var keyBytes = Encoding.UTF8.GetBytes(key);

        byte[] encrypted = new byte[input.Length];
        using (var encrypt = salsa20.CreateEncryptor(keyBytes, dataKey))
            encrypt.TransformBlock(input, 0, input.Length, encrypted, 0);

        return encrypted;
    }
}
