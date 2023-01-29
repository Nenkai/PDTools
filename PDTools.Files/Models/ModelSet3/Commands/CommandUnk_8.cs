using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Syroot.BinaryData;

namespace PDTools.Files.Models.ModelSet3.Commands
{
    public class CommandUnk_8 : ModelCommand
    {
        public float Unk1 { get; set; }
        public float Unk2 { get; set; }
        public float Unk3 { get; set; }
        public float Unk4 { get; set; }
        public byte Unk5 { get; set; }
        public byte Unk6 { get; set; }
        public float Unk7 { get; set; }
        public float Unk8 { get; set; }
        public float Unk9 { get; set; }

        public override void Read(BinaryStream bs, int commandsBaseOffset)
        {
            Unk1 = bs.ReadSingle();
            Unk2 = bs.ReadSingle();
            Unk3 = bs.ReadSingle();
            Unk4 = bs.ReadSingle();
            Unk5 = bs.Read1Byte();
            Unk6 = bs.Read1Byte();
            Unk7 = bs.ReadSingle();
            Unk8 = bs.ReadSingle();
            Unk9 = bs.ReadSingle();
        }

        public override void Write(BinaryStream bs)
        {
            bs.WriteSingle(Unk1);
            bs.WriteSingle(Unk2);
            bs.WriteSingle(Unk3);
            bs.WriteSingle(Unk4);
            bs.WriteByte(Unk5);
            bs.WriteByte(Unk6);
            bs.WriteSingle(Unk7);
            bs.WriteSingle(Unk8);
            bs.WriteSingle(Unk9);

        }

        public override string ToString()
        {
            return $"{nameof(CommandUnk_8)}: {Unk1} {Unk2} {Unk3} {Unk4} {Unk5} {Unk6} {Unk7} {Unk8} {Unk9}";
        }
    }
}
