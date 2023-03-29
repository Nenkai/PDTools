using Syroot.BinaryData;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDTools.Files.Models.VM.Instructions
{
    public class VMLesserEqualToF : VMInstruction
    {
        public override VMInstructionOpcode Opcode => VMInstructionOpcode.FloatLesserOrEqualTo;

        public override void Read(BinaryStream bs, int commandsBaseOffset)
        {

        }

        public override void Write(BinaryStream bs)
        {

        }

        public override string Disassemble(Dictionary<short, VMHostMethodEntry> values)
        {
            return $"LESSER_EQUAL_TO_FLOAT: <=";
        }
    }
}
