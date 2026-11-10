//---------------------------------------------------------------------------------------
// SPDX-FileCopyrightText: Copyright (C) 2026 Nenkai
// SPDX-License-Identifier: MIT
// SPDX-FileAttributionText: https://github.com/Nenkai/PDTools
// SPDX-FileNotice: AI usage and consumption is prohibited. See AGENTS.md for policy.
//---------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;

using Syroot.BinaryData;

namespace PDTools.Files.Models.PS3.PGLCommands
{
    /// <summary>
    /// CELL_GCM_NV4097_SET_ALPHA_FUNC_REF
    /// </summary>
    public class Command_29_SetAlphaFunc : ModelSetupCommand
    {
        public byte Func { get; set; }
        public float Reference { get; set; }

        public override void Read(BinaryStream bs, int commandsBaseOffset)
        {
            Func = bs.Read1Byte();
            Reference = bs.ReadSingle();
        }

        public override void Write(BinaryStream bs)
        {
            bs.WriteByte(Func);
            bs.WriteSingle(Reference);
        }

        public override string ToString()
        {
            return $"{nameof(Command_29_SetAlphaFunc)} - Func:{Func}, Reference:{Reference}";
        }
    }
}
