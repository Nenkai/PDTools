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

namespace PDTools.Structures.MGameParameter;

/// <summary>
/// For GT5
/// </summary>
public class BoostParams
{
    public int BoostFront { get; set; }
    public int BoostRear { get; set; }
    public int BoostFrontMax { get; set; }
    public int BoostRearMax { get; set; }
    public int BoostFrontMin { get; set; }
    public int BoostRearMin { get; set; }

    public void CopyTo(BoostParams other)
    {
        other.BoostFront = other.BoostFront;
        other.BoostRear = other.BoostRear;
        other.BoostFrontMax = other.BoostFrontMax;
        other.BoostRearMax = other.BoostRearMax;
        other.BoostFrontMin = other.BoostFrontMin;
        other.BoostRearMin = other.BoostRearMin;
    }
}
