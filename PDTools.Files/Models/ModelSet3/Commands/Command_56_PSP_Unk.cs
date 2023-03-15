﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Syroot.BinaryData;

namespace PDTools.Files.Models.ModelSet3.Commands
{
    /// <summary>
    /// Seen in GT PSP, not present in GT6 - seemingly unused
    /// </summary>
    public class Command_56_PSP_Unk : ModelCommand
    {
        public byte[] Values { get; set; }

        public override void Read(BinaryStream bs, int commandsBaseOffset)
        {
            Values = bs.ReadBytes(0x10);
        }

        public override void Write(BinaryStream bs)
        {
            bs.WriteBytes(Values);
        }

        public override string ToString()
        {
            return $"{nameof(Command_56_PSP_Unk)}: {string.Join(",", Values)}";
        }
    }
}
