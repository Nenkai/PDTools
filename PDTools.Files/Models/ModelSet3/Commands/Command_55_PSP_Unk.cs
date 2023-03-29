using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Syroot.BinaryData;

namespace PDTools.Files.Models.ModelSet3.Commands
{
    /// <summary>
    /// Seen in GT PSP, not present in GT6
    /// </summary>
    public class Command_55_PSP_Unk : ModelSetupCommand
    {
        public float Unk1 { get; set; }
        public float Unk2 { get; set; }
        public float Unk3 { get; set; }
        public float Unk4 { get; set; }

        public override void Read(BinaryStream bs, int commandsBaseOffset)
        {
            Unk1 = bs.ReadSingle();
            Unk2 = bs.ReadSingle();
            Unk3 = bs.ReadSingle();
            Unk4 = bs.ReadSingle();
        }

        public override void Write(BinaryStream bs)
        {
            bs.WriteSingle(Unk1);
            bs.WriteSingle(Unk2);
            bs.WriteSingle(Unk3);
            bs.WriteSingle(Unk4);
        }

        public override string ToString()
        {
            return $"{nameof(Command_55_PSP_Unk)}: {Unk1} {Unk2} {Unk3} {Unk4}";
        }
    }
}
