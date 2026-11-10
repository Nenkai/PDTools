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

namespace PDTools.Files.Models.PS2.RenderCommands;

public class Cmd_JumpUShort : ModelSetupPS2Command
{
    public override ModelSetupPS2Opcode Opcode => ModelSetupPS2Opcode.Jump_UShort;

    public ushort JumpOffset { get; set; }

    public override void Read(BinaryStream bs, int commandsBaseOffset)
    {
        JumpOffset = bs.ReadUInt16();           
    }

    public override void Write(BinaryStream bs)
    {
        bs.WriteUInt16(JumpOffset);
    }

    public override string ToString()
    {
        return $"{nameof(Cmd_JumpUShort)}";
    }
}
