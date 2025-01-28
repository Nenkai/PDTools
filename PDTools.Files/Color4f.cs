using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDTools.Files;

public struct Color4f
{
    public float R { get; set; }
    public float G { get; set; }
    public float B { get; set; }
    public float A { get; set; }

    public Color4f(float r, float g, float b, float a)
    {
        R = r;
        G = g;
        B = b;
        A = a;
    }

    public static readonly Color4f Zero = new(0f, 0f, 0f, 0f);
    public static readonly Color4f One = new(1f, 1f, 1f, 1f);

    public override readonly int GetHashCode()
    {
        return HashCode.Combine(R, G, B, A);
    }
}
