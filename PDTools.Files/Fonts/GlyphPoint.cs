using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDTools.Files.Fonts
{
    public struct GlyphPoint : IGlyphShapeData
    {
        public float X { get; set; }
        public float Y { get; set; }

        public override string ToString()
        {
            return $"Point - X: {X}, Y: {Y}";
        }
    }
}
