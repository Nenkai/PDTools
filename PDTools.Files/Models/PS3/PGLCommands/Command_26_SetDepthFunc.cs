﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;

using Syroot.BinaryData;

namespace PDTools.Files.Models.PS3.PGLCommands
{
    public class Command_26_SetDepthFunc : ModelSetupCommand
    {
        public byte Func { get; set; }
        public override void Read(BinaryStream bs, int commandsBaseOffset)
        {
            Func = bs.Read1Byte();
        }

        public override void Write(BinaryStream bs)
        {
            bs.WriteByte(Func);
        }

        public override string ToString()
        {
            return $"{nameof(Command_26_SetDepthFunc)}";
        }
    }
}
