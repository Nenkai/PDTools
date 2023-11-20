using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;

using Syroot.BinaryData;

namespace PDTools.Files.Models.PS2.Commands
{
    public class Cmd_Unk53 : ModelSetupPS2Command
    {
        public override ModelSetupPS2Opcode Opcode => ModelSetupPS2Opcode.pgl_53;

        public ushort ShapeIndex { get; set; }
        public ushort Multiplier { get; set; }

        public override void Read(BinaryStream bs, int commandsBaseOffset)
        {
            ShapeIndex = bs.ReadUInt16();
            Multiplier = bs.ReadUInt16();
        }

        public override void Write(BinaryStream bs)
        {
            bs.WriteUInt16(ShapeIndex);
            bs.WriteUInt16(Multiplier);
        }

        public override string ToString()
        {
            return $"{nameof(Cmd_Unk53)}";
        }
    }
}
