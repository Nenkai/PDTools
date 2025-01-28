using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Syroot.BinaryData;

namespace PDTools.Files.Models.VM.Instructions;

public class VMVariablePush : VMInstruction
{
    public override VMInstructionOpcode Opcode => VMInstructionOpcode.StackVariablePush;

    public bool IsHostMethod;
    public short Index;

    public override void Read(BinaryStream bs, int commandsBaseOffset)
    {
        short bits = bs.ReadInt16();
        IsHostMethod = bits >> 15 != 0;
        Index = (short)(bits & 0b1111111_11111111);
    }

    public override void Write(BinaryStream bs)
    {
        short bits = 0;
        bits |= (short)(IsHostMethod ? 1 << 15 : 0);
        bits |= (short)(Index & 0b1111111_11111111);
        bs.WriteInt16(bits);
    }

    public override string Disassemble(Dictionary<short, VMHostMethodEntry> values)
    {
        return $"VARIABLE_PUSH: Local:{Index}";
    }
}
