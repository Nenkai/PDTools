using Syroot.BinaryData;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDTools.Files.Models.VM.Instructions
{
    public class VM0x10 : VMInstruction
    {
        public short Value { get; set; }

        public override void Read(BinaryStream bs, int commandsBaseOffset)
        {
            Value = bs.ReadInt16();
        }

        public override void Write(BinaryStream bs)
        {
            bs.WriteInt16(Value);
        }

        public override string Disassemble(Dictionary<short, VMHostMethodEntry> values)
        {
            return $"{nameof(VM0x10)}: Index:{Value}";
        }
    }
}
