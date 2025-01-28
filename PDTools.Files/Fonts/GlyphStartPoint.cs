using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDTools.Files.Fonts;

public struct GlyphStartPoint : IGlyphShapeData
{
    public float X { get; set; }
    public float Y { get; set; }

    public bool? Unk { get; set; }
    public bool? Unk2 { get; set; }

    public GlyphStartPoint(float x, float y)
    {
        X = x;
        Y = y;

        Unk = null;
        Unk2 = null;
    }

    public override readonly string ToString()
    {
        return $"Start Point - X: {X}, Y:{Y}, {Unk} {Unk2}";
    }
}
