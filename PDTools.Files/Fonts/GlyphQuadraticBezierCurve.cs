using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDTools.Files.Fonts;

public struct GlyphQuadraticBezierCurve : IGlyphShapeData
{
    public float P1_DistX { get; set; }
    public float P1_DistY { get; set; }
    public float P2_DistX { get; set; }
    public float P2_DistY { get; set; }

    public GlyphQuadraticBezierCurve(float p1_X, float p1_Y, float p2_X, float p2_Y)
    {
        P1_DistX = p1_X;
        P1_DistY = p1_Y;
        P2_DistX = p2_X;
        P2_DistY = p2_Y;
    }

    public override readonly string ToString()
    {
        return $"Curve - X1Dist:{P1_DistX} Y1Dist:{P1_DistY} X2Dist:{P2_DistX} Y2Dist:{P2_DistY}";
    }
}
