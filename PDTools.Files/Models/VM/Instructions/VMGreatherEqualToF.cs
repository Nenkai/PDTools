﻿using Syroot.BinaryData;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDTools.Files.Models.VM.Instructions
{
    public class VMGreaterEqualToF : VMInstruction
    {
        public override VMInstructionOpcode Opcode => VMInstructionOpcode.FloatGreaterOrEqualTo;

        public override void Read(BinaryStream bs, int commandsBaseOffset)
        {

        }

        public override void Write(BinaryStream bs)
        {

        }

        public override string Disassemble(Dictionary<short, VMHostMethodEntry> values)
        {
            return $"GREATER_EQUAL_TO_FLOAT: >=";
        }
    }
}
