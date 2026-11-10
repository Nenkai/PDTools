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

namespace PDTools.Files.Fonts;

public struct GlyphLine : IGlyphShapeData
{
    public float Distance { get; set; }
    public GlyphAxis Axis { get; set; }

    public GlyphLine(float distance, GlyphAxis axis)
    {
        Distance = distance; 
        Axis = axis;
    }

    public override readonly string ToString()
    {
        return $"Line - {Axis}: {Distance}";
    }
}
