﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Syroot.BinaryData;

namespace PDTools.Files.Models.PS3.PGLCommands
{
    public class Command_7_Unk : ModelSetupCommand
    {
        public ushort Value { get; set; }

        public override void Read(BinaryStream bs, int commandsBaseOffset)
        {
            Value = bs.ReadUInt16();
        }

        public override void Write(BinaryStream bs)
        {
            bs.WriteUInt16(Value);
        }

        public override string ToString()
        {
            return $"{nameof(Command_7_Unk)}: {Value}";
        }
    }
}
