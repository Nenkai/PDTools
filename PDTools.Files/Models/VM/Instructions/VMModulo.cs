using Syroot.BinaryData;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDTools.Files.Models.VM.Instructions
{
    public class VMModulo : VMInstruction
    {
        public override VMInstructionOpcode Opcode => VMInstructionOpcode.Modulo;

        public override void Read(BinaryStream bs, int commandsBaseOffset)
        {

        }

        public override void Write(BinaryStream bs)
        {

        }

        public override string Disassemble(Dictionary<short, VMHostMethodEntry> values)
        {
            return $"MODULO: %";
        }
    }
}
