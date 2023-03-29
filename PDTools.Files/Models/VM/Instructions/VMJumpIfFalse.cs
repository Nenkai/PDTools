using Syroot.BinaryData;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDTools.Files.Models.VM.Instructions
{
    public class VMJumpIfFalse : VMInstruction
    {
        public override VMInstructionOpcode Opcode => VMInstructionOpcode.JumpIfFalse0x26;

        public short JumpOffset { get; set; }

        public override void Read(BinaryStream bs, int commandsBaseOffset)
        {
            JumpOffset = bs.ReadInt16();
        }

        public override void Write(BinaryStream bs)
        {
            bs.WriteInt16(JumpOffset);
        }

        public override string Disassemble(Dictionary<short, VMHostMethodEntry> values)
        {
            return $"JUMP_IF_FALSE: Offset={Offset + JumpOffset + 3:X2}";
        }
    }
}
