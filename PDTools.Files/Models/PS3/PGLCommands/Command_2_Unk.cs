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

using Syroot.BinaryData;

namespace PDTools.Files.Models.PS3.PGLCommands
{
    public class Command_2_Unk : ModelSetupCommand
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
            return $"{nameof(Command_2_Unk)}: {ModelIndex}";
        }
    }
}
