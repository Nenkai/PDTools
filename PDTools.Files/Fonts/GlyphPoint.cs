// Copyright (c) 2026 Nenkai
// SPDX-License-Identifier: MIT

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDTools.Files.Fonts;

public struct GlyphPoint : IGlyphShapeData
{
    public float X { get; set; }
    public float Y { get; set; }

    public GlyphPoint(float x, float y)
    {
        X = x;
        Y = y;
    }

    public override readonly string ToString()
    {
        return $"Point - X: {X}, Y: {Y}";
    }
}
