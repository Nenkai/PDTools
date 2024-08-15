﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;

using Syroot.BinaryData;

namespace PDTools.Files.Models.PS2.Commands
{
    public class Cmd_pgluSetExternalTexIndex : ModelSetupPS2Command
    {
        public override ModelSetupPS2Opcode Opcode => ModelSetupPS2Opcode.pglExternalTexIndex;

        public byte TexSetIndex { get; set; }

        public Cmd_pgluSetExternalTexIndex() { }

        public Cmd_pgluSetExternalTexIndex(byte index)
        {
            ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual(index, 16, "External tex set index must be below 0-15.");
            TexSetIndex = index;
        }
        public override void Read(BinaryStream bs, int commandsBaseOffset)
        {
            TexSetIndex = bs.Read1Byte();
        }

        public override void Write(BinaryStream bs)
        {
            bs.WriteByte(TexSetIndex);
        }

        public override string ToString()
        {
            return $"{nameof(Cmd_pgluSetExternalTexIndex)} - TexSet: {TexSetIndex}";
        }
    }
}
