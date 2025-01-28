using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Syroot.BinaryData;

namespace PDTools.Files.Models.VM.Instructions;

public class VMUnaryBitwiseNot : VMInstruction
{
    public override VMInstructionOpcode Opcode => VMInstructionOpcode.UnaryBitwiseNotOperator;

    public override void Read(BinaryStream bs, int commandsBaseOffset)
    {

    }

    public override void Write(BinaryStream bs)
    {

    }
}
