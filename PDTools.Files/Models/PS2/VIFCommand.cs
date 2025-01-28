using Syroot.BinaryData;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDTools.Files.Models.PS2;

public class VIFCommand
{
    public ushort VUAddr { get; set; }
    public byte Num { get; set; }
    public VIFCommandOpcode CommandOpcode { get; set; }
    public bool IRQ { get; set; }

    public GIFTag GIFTag { get; set; }
    public List<object> UnpackData { get; set; } = [];

    public void FromStream(BinaryStream bs)
    {
        VUAddr = bs.ReadUInt16();
        Num = bs.Read1Byte();

        byte bits = bs.Read1Byte();
        CommandOpcode = (VIFCommandOpcode)(bits & 0b1111111);
        IRQ = (bits >> 7 & 1) == 1;

        if (VUAddr == 0xC0C0)
        {
            GIFTag = new GIFTag();
            GIFTag.FromStream(bs);
        }
        else
        {
            if (CommandOpcode == VIFCommandOpcode.STROW)
            {
                bs.Position += 4 * 4;
            }
            else if (((int)CommandOpcode & (int)VIFCommandOpcode.UNPACK) != 0)
            {
                for (var i = 0; i < Num; i++)
                {
                    int field_type = (int)CommandOpcode & 0x03;
                    int elem_count = ((int)CommandOpcode >> 2 & 0x03) + 1;

                    if (field_type == 0)
                    {
                        UnpackData.Add(bs.ReadInt32s(elem_count));
                    }
                    else if (field_type == 1)
                    {
                        UnpackData.Add(bs.ReadInt16s(elem_count));
                    }
                    else if (field_type == 2)
                    {
                        UnpackData.Add(bs.ReadBytes(elem_count));
                    }
                }
            }
        }

        bs.Align(0x04);
    }

    public void Write(BinaryStream bs)
    {
        bs.WriteUInt16(VUAddr);
        bs.WriteByte(Num);
        bs.WriteByte((byte)(((IRQ ? 1 : 0) << 7) | ((byte)CommandOpcode & 0b1111111)));

        if (VUAddr == 0xC0C0)
        {
            GIFTag.Write(bs);
        }
        else
        {
            for (int i = 0; i < UnpackData.Count; i++)
            {
                if (UnpackData[i] is int[] intArr)
                    bs.WriteInt32s(intArr);
                else if (UnpackData[i] is byte[] byteArr)
                    bs.WriteBytes(byteArr);
                else if (UnpackData[i] is short[] shortArr)
                    bs.WriteInt16s(shortArr);
            }
        }

        bs.Align(0x04, grow: true);
    }

    public override string ToString()
    {
        return $"{VUAddr} {CommandOpcode:X4} ({Num})";
    }
}

public enum VIFCommandOpcode : byte
{
    NOP = 0x00,
    STCYCL = 0x01,
    OFFSET = 0x02,
    BASE = 0x03,
    ITOP = 0x04,
    STMOD = 0x05,
    MSKPATH3 = 0x06,
    MARK = 0x07,
    FLUSHE = 0x10,
    FLUSH = 0x11,
    FLUSHA = 0x13,
    MSCAL = 0x14,
    MSCALF = 0x15,
    MSCNT = 0x17,
    STMASK = 0x20,
    STROW = 0x30,
    STCOL = 0x31,
    MPG = 0x4A,
    DIRECT = 0x50,
    DIRECTHL = 0x51,
    UNPACK = 0x60,
}

public enum VIFCommandOpcodeUnpack : byte
{
    UNPACK_S_32 = 0x00,
    UNPACK_S_16 = 0x01,
    UNPACK_S_8 = 0x02,

    UNPACK_V2_32 = 0x04,
    UNPACK_V2_16 = 0x05,
    UNPACK_V2_8 = 0x06,

    UNPACK_V3_32 = 0x08,
    UNPACK_V3_16 = 0x09,
    UNPACK_V3_8 = 0x0A,

    UNPACK_V4_32 = 0x0C,
    UNPACK_V4_16 = 0x0D,
    UNPACK_V4_8 = 0x0E,

    UNPACK_V4_5 = 0x0F,
}
