using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Syroot.BinaryData;

namespace PDTools.Files.Models.VM.Instructions;

public class VMIntConst : VMInstruction
{
    public override VMInstructionOpcode Opcode => VMInstructionOpcode.PushIntConst;

    public int Value { get; set; }

    public override void Read(BinaryStream bs, int commandsBaseOffset)
    {
        Value = bs.ReadInt32();
    }

    public override void Write(BinaryStream bs)
    {
        bs.WriteInt32(Value);
    }

    public override string Disassemble(Dictionary<short, VMHostMethodEntry> values)
    {
        return $"CONST_VALUE: {Value} ({BitConverter.Int32BitsToSingle(Value)})";
    }
}
