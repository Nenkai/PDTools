// Copyright (c) 2026 Nenkai
// SPDX-License-Identifier: MIT

using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices;
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
        public Matrix4x4 Matrix { get; set; }
        public override void Read(BinaryStream bs, int commandsBaseOffset)
        {
            Matrix = MemoryMarshal.Cast<float, Matrix4x4>(bs.ReadSingles(4*4))[0];
        }

        public override void Write(BinaryStream bs)
        {
            for (var x = 0; x < 4; x++)
                for (int y = 0; y < 4; y++)
                bs.WriteSingle(Matrix[x, y]);
        }

        public override string ToString()
        {
            return $"{nameof(Command_16_PGLInverse)} - {string.Join(", ", Matrix)}";
        }
    }
}
