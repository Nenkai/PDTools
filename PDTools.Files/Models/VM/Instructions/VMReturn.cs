using Syroot.BinaryData;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDTools.Files.Models.VM.Instructions
{
    public class VMReturn : VMInstruction
    {
        public byte Offset { get; set; }

        public override void Read(BinaryStream bs, int commandsBaseOffset)
        {
            Offset = bs.Read1Byte();
        }

        public override void Write(BinaryStream bs)
        {
            bs.WriteByte(Offset);
        }

        public override string Disassemble(Dictionary<short, VMHostMethodEntry> values)
        {
            return $"{nameof(VMReturn)}: {Offset}";
        }
    }
}
