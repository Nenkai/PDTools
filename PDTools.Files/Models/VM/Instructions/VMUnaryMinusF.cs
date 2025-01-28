using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Syroot.BinaryData;

namespace PDTools.Files.Models.VM.Instructions;

public class VMUnaryMinusF : VMInstruction
{
    public override VMInstructionOpcode Opcode => VMInstructionOpcode.FloatUnaryMinusOperator;

    public override void Read(BinaryStream bs, int commandsBaseOffset)
    {

    }

    public override void Write(BinaryStream bs)
    {

    }
}
