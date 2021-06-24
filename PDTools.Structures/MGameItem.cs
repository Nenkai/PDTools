using System;
using PDTools.Utils;

namespace PDTools.Structures
{
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
        public string F_Name { get; set; }

        public byte[] Blob { get; set; }

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
            stream.WriteByteData(Blob ?? new byte[] { 0x30 }); // 'zero_blob' if not set

            // Pad
            for (int i = 0; i < 0x40; i++)
                stream.WriteByte(0);
        }

        public static MGameItem Read(Span<byte> buffer)
        {
            var stream = new BitStream(BitStreamMode.Read, buffer);
            if (stream.ReadUInt32() != SerializeMagic)
                throw new ArgumentException("Provided MGameItem buffer is not valid, magic did not match.");

            stream.ReadUInt32();
            MGameItem item = new MGameItem();
            item.ItemType = (GameItemType)stream.ReadUInt32();
            item.ItemCategory = (GameItemCategory)stream.ReadUInt32();
            item.Argument1 = stream.ReadUInt32();
            item.Argument2 = stream.ReadUInt32();
            item.Argument3 = stream.ReadUInt32();
            item.Argument4 = stream.ReadUInt32();
            item.F_Name = stream.ReadString4Aligned();
            stream.ReadByteArrayPrefixed(item.Blob);

            for (int i = 0; i < 0x40; i++)
                stream.ReadByte();
            return item;
        }
    }

    public enum GameItemType
    {
        NONE,
        CAR,
        DRIVER,
        DRIVER_ITEM,
        MONEY,
        TUNE_PARTS,
        OTHERPARTS,
        MUSEUMCARD,
        MOVIE,
        SPECIAL,
        PARTS_TICKET,
        AVATAR,
        OTHER,
    }

    // x** = Main Category
    // *x* = Sub Category
    // **x = Item 
    public enum GameItemCategory
    {
        NONE,
        CAR = 100,
        DRIVER = 200,

        DRIVER_ITEM = 300,
        DRIVER_HEAD = 301,
        DRIVER_BODY = 302,
        DRIVER_SET = 303,
        MONEY = 400,

        TUNERPARTS = 500,
        BODY_CHASSIS = 511,
        ENGINE = 521,
        ADMISSION = 531,
        EMISSION = 532,
        BOOSTER = 541,
        TRANSMISSION = 551,
        DRIVETRAIN = 556,
        SUSPENSION = 561,
        BRAKE = 571,
        BTIRE = 581,
        CTIRE = 582,
        VTIRE = 583,
        STIRE = 586,
        OTHERS = 591,
        HORN = 596,

        OTHER_PARTS = 600,
        PAINT_ITEM = 601,
        SPECIAL_PAINT_ITEM = 602,

        MUSEUMCARD = 700,
        MOVIE = 800,

        SPECIAL = 900,
        PRESENTCAR_TICKET = 901,
        PRESENTITEM_TICKET = 902,
        SPECIAL_TICKET = 903,
    }
}
