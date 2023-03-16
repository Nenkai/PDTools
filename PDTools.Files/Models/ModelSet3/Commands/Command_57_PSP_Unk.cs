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
    public class Command_57_PSP_Unk : ModelSetupCommand
    {
        public ushort Unk1 { get; set; }
        public ushort Unk2 { get; set; }
        public ushort Unk3 { get; set; }
        public ushort Unk4 { get; set; }

        public override void Read(BinaryStream bs, int commandsBaseOffset)
        {
            Unk1 = bs.ReadUInt16();
            Unk2 = bs.ReadUInt16();
            Unk3 = bs.ReadUInt16();
            Unk4 = bs.ReadUInt16();
        }

        public override void Write(BinaryStream bs)
        {
            bs.WriteUInt16(Unk1);
            bs.WriteUInt16(Unk2);
            bs.WriteUInt16(Unk3);
            bs.WriteUInt16(Unk4);
        }

        public override string ToString()
        {
            return $"{nameof(Command_57_PSP_Unk)}: {Unk1} {Unk2} {Unk3} {Unk4}";
        }
    }
}
