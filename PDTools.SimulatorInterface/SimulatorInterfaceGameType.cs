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

namespace PDTools.SimulatorInterface;

/// <summary>
/// Games supporting a Simulation Interface.
/// </summary>
public enum SimulatorInterfaceGameType
{
    /// <summary>
    /// Gran Turismo 6
    /// </summary>
    GT6,

    /// <summary>
    /// Gran Turismo Sport
    /// </summary>
    GTSport,

    /// <summary>
    /// Gran Turismo 7
    /// </summary>
    GT7
}
