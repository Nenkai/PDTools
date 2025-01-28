using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;

using Syroot.BinaryData;

namespace PDTools.Files.Models.PS2.Commands
{
    public class Cmd_pgluCallShape_UShort : ModelSetupPS2Command
    {
        public override ModelSetupPS2Opcode Opcode => ModelSetupPS2Opcode.pgluCallShape_UShort;

        public ushort ShapeIndex { get; set; }

        public override void Read(BinaryStream bs, int commandsBaseOffset)
        {
            ShapeIndex = bs.ReadUInt16();
        }

        public override void Write(BinaryStream bs)
        {
            bs.WriteUInt16(ShapeIndex);
        }

        public override string ToString()
        {
            return $"{nameof(Cmd_pgluCallShape_UShort)} - Shape: {ShapeIndex}";
        }
    }
}
