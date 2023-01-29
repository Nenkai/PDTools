﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Syroot.BinaryData;

namespace PDTools.Files.Models.ModelSet3.Commands
{
    public class CommandUnk_2 : ModelCommand
    {
        public short ModelIndex;

        public override void Read(BinaryStream bs, int commandsBaseOffset)
        {
            ModelIndex = bs.ReadInt16();
        }

        public override void Write(BinaryStream bs)
        {
            bs.WriteInt16(ModelIndex);
        }

        public override string ToString()
        {
            return $"{nameof(CommandUnk_2)}: {ModelIndex}";
        }
    }
}
