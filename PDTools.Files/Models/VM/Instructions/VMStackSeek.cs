using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Syroot.BinaryData;

namespace PDTools.Files.Models.VM.Instructions;

/// <summary>
/// Used to make space for stack variables at the start of VM blocks.
/// </summary>
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
