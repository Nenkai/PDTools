using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDTools.Files.Fonts
{
    public struct GlyphLine : IGlyphShapeData
    {
        public float Distance { get; set; }
        public GlyphAxis Axis { get; set; }

        public GlyphLine(float distance, GlyphAxis axis)
        {
            Distance = distance; 
            Axis = axis;
        }

        public override string ToString()
        {
            return $"Line - {Axis}: {Distance}";
        }
    }
}
