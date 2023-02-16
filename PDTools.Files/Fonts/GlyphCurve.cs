using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDTools.Files.Fonts
{
    public struct GlyphCurve : IGlyphShapeData
    {
        public float P1 { get; set; }
        public float P2 { get; set; }
        public float P3 { get; set; }
        public float P4 { get; set; }

        public override string ToString()
        {
            return $"Curve - {P1} {P2} {P3} {P4}";
        }
    }
}
