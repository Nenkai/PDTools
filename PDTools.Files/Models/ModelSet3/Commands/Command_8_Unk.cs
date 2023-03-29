using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Syroot.BinaryData;

namespace PDTools.Files.Models.ModelSet3.Commands
{
    public class Command_8_Unk : ModelSetupCommand
    {
        public float Unk1 { get; set; }
        public float Unk2 { get; set; }
        public float Unk3 { get; set; }
        public float Unk4 { get; set; }
        public byte Flag { get; set; }

        public float Unk7 { get; set; }
        public short Unk8 { get; set; }
        public byte Unk9 { get; set; }

        public override void Read(BinaryStream bs, int commandsBaseOffset)
        {
            Unk1 = bs.ReadSingle();
            Unk2 = bs.ReadSingle();
            Unk3 = bs.ReadSingle();
            Unk4 = bs.ReadSingle();
            Flag = bs.Read1Byte();

            if ((Flag & 0x80) == 0)
            {

            }
            else
            {
                if ((Flag & 0x40) != 0x00)
                {
                    Unk7 = bs.ReadSingle();
                }

                if ((Flag & 0x20) != 0x00)
                {
                    Unk8 = bs.ReadInt16();
                }

                if ((Flag & 0x10) != 0x00)
                {
                    Unk9 = bs.Read1Byte();
                }
            }

            int count = Flag & 0b1111;
            var singles = bs.ReadSingles(count);

            byte count2 = bs.Read1Byte();
            bs.ReadInt16s(count2);
        }

        public override void Write(BinaryStream bs)
        {
            throw new NotImplementedException("Finish this");

            bs.WriteSingle(Unk1);
            bs.WriteSingle(Unk2);
            bs.WriteSingle(Unk3);
            bs.WriteSingle(Unk4);
            bs.WriteByte(Flag);

        }

        public override string ToString()
        {
            return $"{nameof(Command_8_Unk)}: {Unk1} {Unk2} {Unk3} {Unk4} {Flag}";
        }
    }
}
