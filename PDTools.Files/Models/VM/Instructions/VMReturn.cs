using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Syroot.BinaryData;

namespace PDTools.Files.Models.VM.Instructions;

public class VMReturn : VMInstruction
{
    public override VMInstructionOpcode Opcode => VMInstructionOpcode.Return;

    public byte RetOffset { get; set; }

    public override void Read(BinaryStream bs, int commandsBaseOffset)
    {
        RetOffset = bs.Read1Byte();
    }

    public override void Write(BinaryStream bs)
    {
        bs.WriteByte(RetOffset);
    }

    public override string Disassemble(Dictionary<short, VMHostMethodEntry> values)
    {
        return $"{nameof(VMReturn)}: {RetOffset}";
    }
}
