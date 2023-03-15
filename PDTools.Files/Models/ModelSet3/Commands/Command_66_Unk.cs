﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Syroot.BinaryData;

namespace PDTools.Files.Models.ModelSet3.Commands
{
    public class Command_66_Unk : ModelCommand
    {
        public uint Unk { get; set; }

        public override void Read(BinaryStream bs, int commandsBaseOffset)
        {
            Unk = bs.ReadUInt32();
        }

        public override void Write(BinaryStream bs)
        {
            bs.WriteUInt32(Unk);
        }

        public override string ToString()
        {
            return $"{nameof(Command_66_Unk)}";
        }
    }
}
