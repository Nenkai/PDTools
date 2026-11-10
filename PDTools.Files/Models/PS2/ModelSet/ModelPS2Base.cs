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

using PDTools.Files.Models.PS2.RenderCommands;
using PDTools.Files.Textures.PS2;

namespace PDTools.Files.Models.PS2.ModelSet;

public abstract class ModelPS2Base
{
    /// <summary>
    /// Commands interpreted on every tick to figure how and what shape should be rendered.
    /// </summary>
    public List<ModelSetupPS2Command> Commands { get; set; } = [];
}
