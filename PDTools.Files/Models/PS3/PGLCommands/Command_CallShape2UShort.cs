﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Syroot.BinaryData;

namespace PDTools.Files.Models.PS3.PGLCommands
{
    public class Command_CallShape2UShort : ModelSetupCommand
    {
        public short Unk { get; set; }
        public short Unk2 { get; set; }

        public override void Read(BinaryStream bs, int commandsBaseOffset)
        {
            Unk = bs.ReadInt16();
            Unk2 = bs.ReadInt16();
        }

        public override void Write(BinaryStream bs)
        {
            bs.WriteInt16(Unk);
            bs.WriteInt16(Unk2);
        }

        public override string ToString()
        {
            return $"{nameof(Command_CallShape2UShort)} - {Unk} {Unk2}";
        }
    }
}
