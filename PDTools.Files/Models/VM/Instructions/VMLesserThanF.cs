﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Syroot.BinaryData;

namespace PDTools.Files.Models.VM.Instructions;

public class VMLesserThanF : VMInstruction
{
    public override VMInstructionOpcode Opcode => VMInstructionOpcode.FloatLesserThan;

    public override void Read(BinaryStream bs, int commandsBaseOffset)
    {

    }

    public override void Write(BinaryStream bs)
    {

    }

    public override string Disassemble(Dictionary<short, VMHostMethodEntry> values)
    {
        return $"LESSER_EQUAL_TO_FLOAT: <";
    }
}
