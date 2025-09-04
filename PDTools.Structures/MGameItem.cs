using System;
using PDTools.Utils;
using PDTools.Enums.PS3;

namespace PDTools.Structures;

public class MGameItem
{
    public const uint SerializeMagic = 0xE5_E5_D2_B3;
    public const uint Version = 1_00;

    // itemtype
    public GameItemType ItemType { get; set; }

    // itemcategory
    public GameItemCategory ItemCategory { get; set; }

    // argument1
    public uint Argument1 { get; set; }

    // argument2
    public uint Argument2 { get; set; }

    // argument3
    public uint Argument3 { get; set; }

    // argument4
    public uint Argument4 { get; set; }

    // f_name
    public string? F_Name { get; set; }

    public byte[]? Blob { get; set; }

    public void Write(byte[] data)
    {
        var stream = new BitStream();
        stream.WriteUInt32(SerializeMagic);
        stream.WriteUInt32(Version);
        stream.WriteUInt32((uint)ItemType);
        stream.WriteUInt32((uint)ItemCategory);
        stream.WriteUInt32(Argument1);
        stream.WriteUInt32(Argument2);
        stream.WriteUInt32(Argument3);
        stream.WriteUInt32(Argument4);
        stream.WriteNullStringAligned4(F_Name);
        stream.WriteByteData(Blob ?? "0"u8.ToArray()); // 'zero_blob' if not set

        // Pad
        for (int i = 0; i < 0x40; i++)
            stream.WriteByte(0);
    }

    public static MGameItem Read(Span<byte> buffer)
    {
        var stream = new BitStream(BitStreamMode.Read, buffer);
        if (stream.ReadUInt32() != SerializeMagic)
            throw new ArgumentException("Provided MGameItem buffer is not valid, magic did not match.");

        uint version = stream.ReadUInt32();
        MGameItem item = new MGameItem();
        item.ItemType = (GameItemType)stream.ReadUInt32();
        item.ItemCategory = (GameItemCategory)stream.ReadUInt32();
        item.Argument1 = stream.ReadUInt32();
        item.Argument2 = stream.ReadUInt32();
        item.Argument3 = stream.ReadUInt32();
        item.Argument4 = stream.ReadUInt32();
        item.F_Name = stream.ReadString4Aligned(align: 0x04);
        item.Blob = stream.ReadByteArrayPrefixed();

        for (int i = 0; i < 0x40; i++)
            stream.ReadByte();
        return item;
    }
}
