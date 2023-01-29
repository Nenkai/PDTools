﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Syroot.BinaryData;

namespace PDTools.Files.Models.ModelSet3.Commands
{
    public class CommandUnk_53 : ModelCommand
    {
        public ushort Unk { get; set; }

        public override void Read(BinaryStream bs, int commandsBaseOffset)
        {
            Unk = bs.ReadUInt16();
        }

        public override void Write(BinaryStream bs)
        {
            bs.WriteUInt16(Unk);
        }

        public override string ToString()
        {
            return $"{nameof(CommandUnk_53)}: {Unk}";
        }
    }
}
