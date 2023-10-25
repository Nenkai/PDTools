using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;

using Syroot.BinaryData;

namespace PDTools.Files.Models.PS2.Commands
{
    public class Command_4_pgluCallShape1ub : ModelSetupPS2Command
    {
        public override ModelSetupPS2Opcode Opcode => ModelSetupPS2Opcode.pgluCallShape_1ub;

        public byte ShapeIndex { get; set; }

        public override void Read(BinaryStream bs, int commandsBaseOffset)
        {
            ShapeIndex = bs.Read1Byte();
        }

        public override void Write(BinaryStream bs)
        {
            bs.WriteByte(ShapeIndex);
        }

        public override string ToString()
        {
            return $"{nameof(Command_4_pgluCallShape1ub)} - Shape: {ShapeIndex}";
        }
    }
}
