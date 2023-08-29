using Syroot.BinaryData;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDTools.Files.Models.ModelSet2
{
    public class VIFCommand
    {
        public ushort VUAddr { get; set; }
        public byte Num { get; set; }
        public VIFCommandOpcode CommandOpcode { get; set; }
        public bool IRQ { get; set; }

        public List<object> UnpackData { get; set; } = new List<object>();

        public static VIFCommand FromStream(BinaryStream bs, long mdlBasePos)
        {
            VIFCommand cmd = new VIFCommand();
            cmd.VUAddr = bs.ReadUInt16();
            cmd.Num = bs.Read1Byte();

            byte bits = bs.Read1Byte();
            cmd.CommandOpcode = (VIFCommandOpcode)(bits & 0b1111111);
            cmd.IRQ = ((bits >> 7) & 1) == 1;

            if (cmd.CommandOpcode == VIFCommandOpcode.STROW)
            {
                bs.Position += 4 * 4;
            }
            else if (((int)cmd.CommandOpcode & (int)VIFCommandOpcode.UNPACK) != 0)
            {
                for (var i = 0; i < cmd.Num; i++)
                {
                    int field_type = (int)cmd.CommandOpcode & 0x03;
                    int elem_count = (((int)cmd.CommandOpcode >> 2) & 0x03) + 1;

                    if (field_type == 0)
                    {
                        cmd.UnpackData.Add(bs.ReadInt32s(elem_count));
                    }
                    else if (field_type == 1)
                    {
                        cmd.UnpackData.Add(bs.ReadInt16s(elem_count));
                    }
                    else if (field_type == 2)
                    {
                        cmd.UnpackData.Add(bs.ReadBytes(elem_count));
                    }
                }
            }

            bs.Align(0x04);

            return cmd;
        }

        public override string ToString()
        {
            return $"{VUAddr} {CommandOpcode:X4} ({Num})";
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
    }
}
