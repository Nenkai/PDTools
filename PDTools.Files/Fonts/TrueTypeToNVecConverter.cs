using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

using Typography.OpenFont;

using TTFGlyph = Typography.OpenFont.Glyph;
using NVecGlyph = PDTools.Files.Fonts.Glyph;

namespace PDTools.Files.Fonts
{
    public class TrueTypeToNVecConverter
    {
        public static void Convert(string inputTtf, string outputVec)
        {
            using var ttf = File.OpenRead(inputTtf);
            var ttfFont = new OpenFontReader().Read(ttf);

            NVectorFont vecFont = new NVectorFont();
            foreach (var ttfGlyphName in ttfFont.GetGlyphNameIter())
            {
                int codePoint = AdobeGlyphList.GetUnicodeValueByGlyphName(ttfGlyphName.glyphName);
                if (codePoint == 0)
                    continue;

                TTFGlyph ttfGlyph = ttfFont.GetGlyph(ttfGlyphName.glyphIndex);

                NVecGlyph vecGlyph = new NVecGlyph((char)codePoint);
                vecGlyph.AdvanceWidth = (ushort)(ttfFont.GetAdvanceWidthFromGlyphIndex((ushort)ttfGlyphName.glyphIndex) / 2);

                if (codePoint == (char)'A')
                    ;

                Span<ushort> endPoints = ttfGlyph.EndPoints.AsSpan();

                if (ttfGlyph.GlyphPoints.Any())
                {
                    GlyphPointF currentOutlineStartPoint = ttfGlyph.GlyphPoints[0];
                    for (var i = 0; i < ttfGlyph.GlyphPoints.Length; i++)
                    {
                        GlyphPointF point = ttfGlyph.GlyphPoints[i];

                        int endPointIndex = endPoints.IndexOf((ushort)(i - 1));
                        bool isEnd = (endPointIndex != -1 && endPointIndex != endPoints.Length - 1);
                        if (i == 0 || isEnd)
                        {
                            if (i != 0)
                            {
                                var prev = ttfGlyph.GlyphPoints[i - 1];
                                IGlyphShapeData shape = CompareAdd(prev, currentOutlineStartPoint);
                                vecGlyph.Points.Data.Add(shape);
                            }

                            currentOutlineStartPoint = point;

                            var startPoint = new GlyphStartPoint(point.X / 2, ((-point.Y) / 2));


                            if (i == 0)
                                startPoint.Unk = true;

                            vecGlyph.Points.Data.Add(startPoint);
                            vecGlyph.Points.Data.Add(new GlyphLine(0, GlyphAxis.Y));
                        }
                        else
                        {
                            IGlyphShapeData shape = CompareAdd(ttfGlyph.GlyphPoints[i - 1], point);
                            vecGlyph.Points.Data.Add(shape);
                        }
                    }

                    vecGlyph.Points.Data.Add(CompareAdd(ttfGlyph.GlyphPoints[^1], currentOutlineStartPoint));
                    vecGlyph.Points.Data.Add(new GlyphLine(0, GlyphAxis.Y));
                }
                vecFont.AddGlyph(vecGlyph);
            }

            using var outputStream = File.Create(outputVec);
            vecFont.Write(outputStream);
        }

        private static IGlyphShapeData CompareAdd(GlyphPointF prev, GlyphPointF next)
        {
            if (next.X == prev.X)
            {
                return new GlyphLine((prev.Y - next.Y) / 2, GlyphAxis.Y);
            }
            else if (next.Y == prev.Y)
            {
                return new GlyphLine((next.X - prev.X) / 2, GlyphAxis.X);
            }
            else
            {
                return new GlyphPoint((next.X - prev.X) / 2, (prev.Y - next.Y) / 2);
            }
        }
    }
}
