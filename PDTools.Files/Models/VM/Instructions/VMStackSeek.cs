using Syroot.BinaryData;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDTools.Files.Models.VM.Instructions
{
    public class VMStackSeek : VMInstruction
    {
        public override VMInstructionOpcode Opcode => VMInstructionOpcode.StackAdvance;

        public byte Count { get; set; }

        public override void Read(BinaryStream bs, int commandsBaseOffset)
        {
            Count = bs.Read1Byte();
        }

        public override void Write(BinaryStream bs)
        {
            bs.WriteByte(Count);
        }

        public override string Disassemble(Dictionary<short, VMHostMethodEntry> values)
        {
            return $"{nameof(VMStackSeek)}: Count={Count}";
        }
    }
}
