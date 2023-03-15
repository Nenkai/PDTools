using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Syroot.BinaryData;

namespace PDTools.Files.Models.ModelSet3.Commands
{
    /// <summary>
    /// Seen in GT PSP, not present in GT6, but seemingly unused/stubbed
    /// </summary>
    public class Command_58_PSP_Unk : ModelCommand
    {
        public uint Unk1 { get; set; }
        public uint Unk2 { get; set; }

        public override void Read(BinaryStream bs, int commandsBaseOffset)
        {
            Unk1 = bs.ReadUInt32();
            Unk2 = bs.ReadUInt32();
        }

        public override void Write(BinaryStream bs)
        {
            bs.WriteUInt32(Unk1);
            bs.WriteUInt32(Unk2);
        }

        public override string ToString()
        {
            return $"{nameof(Command_58_PSP_Unk)}: {Unk1} {Unk2}";
        }
    }
}
