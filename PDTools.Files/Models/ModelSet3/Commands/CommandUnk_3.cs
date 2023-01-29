﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;

using Syroot.BinaryData;

namespace PDTools.Files.Models.ModelSet3.Commands
{
    public class CommandUnk_3 : ModelCommand
    {
        public byte MeshIndex;

        public override void Read(BinaryStream bs, int commandsBaseOffset)
        {
            MeshIndex = bs.Read1Byte();
        }

        public override void Write(BinaryStream bs)
        {
            bs.WriteByte(MeshIndex);
        }

        public override string ToString()
        {
            return $"{nameof(CommandUnk_3)}: {MeshIndex}";
        }
    }
}
