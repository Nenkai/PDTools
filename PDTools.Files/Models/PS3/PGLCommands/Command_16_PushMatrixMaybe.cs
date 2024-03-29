﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Syroot.BinaryData;

namespace PDTools.Files.Models.PS3.PGLCommands
{
    /// <summary>
    /// Not sure
    /// </summary>
    public class Command_16_PGLInverse : ModelSetupCommand
    {
        public float[] Matrix { get; set; }
        public override void Read(BinaryStream bs, int commandsBaseOffset)
        {
            Matrix = bs.ReadSingles(16);
        }

        public override void Write(BinaryStream bs)
        {
            for (var i = 0; i < 16; i++)
                bs.WriteSingle(Matrix[i]);
        }

        public override string ToString()
        {
            return $"{nameof(Command_16_PGLInverse)} - {string.Join(", ", Matrix)}";
        }
    }
}
