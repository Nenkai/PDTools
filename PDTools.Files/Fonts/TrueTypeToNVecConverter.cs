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
        /* Some notes:
         * May not be the prettiest conversion code, Y is also swapped around
         * "/ 2" is hardcoded for 2048 units per em fonts */

        public static int Scale { get; set; } = 2;
        public static void Convert(string inputTtf, string outputVec)
        {
            using var ttf = File.OpenRead(inputTtf);
            Typeface ttfFont = new OpenFontReader().Read(ttf);

            NVectorFont vecFont = new NVectorFont();

            List<uint> unicodes = new List<uint>();
            ttfFont.CollectUnicode(unicodes);
            unicodes = unicodes.Distinct().ToList();

            foreach (var codePoint in unicodes)
            {
                if (codePoint == 0)
                    continue;

                ushort glyphIndex = ttfFont.GetGlyphIndex((int)codePoint);
                TTFGlyph ttfGlyph = ttfFont.GetGlyph(glyphIndex);

                NVecGlyph vecGlyph = new NVecGlyph((char)codePoint);
                vecGlyph.AdvanceWidth = (ushort)(ttfFont.GetAdvanceWidthFromGlyphIndex((ushort)glyphIndex) / Scale);

                int offset = ttfFont.Bounds.YMax - ttfGlyph.Bounds.YMax;
                vecGlyph.HeightOffset = (ushort)(offset / Scale);

                Span<ushort> endPoints = ttfGlyph.EndPoints.AsSpan();

                GlyphPointF lastPoint = default;

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
                                if (prev.onCurve)
                                {
                                    IGlyphShapeData shape = CompareAdd(prev, currentOutlineStartPoint);
                                    vecGlyph.Points.Data.Add(shape);
                                }
                            }

                            currentOutlineStartPoint = point;

                            var startPoint = new GlyphStartPoint(point.X / Scale, (-point.Y / Scale));
                            lastPoint = point;

                            if (i == 0)
                                startPoint.Unk = true;

                            vecGlyph.Points.Data.Add(startPoint);
                        }
                        else
                        {
                            // Handle curves
                            if (!ttfGlyph.GlyphPoints[i].onCurve)
                            {
                                GlyphPointF start = lastPoint;
                                GlyphPointF control = ttfGlyph.GlyphPoints[i];

                                if (endPoints.IndexOf((ushort)i) != -1)
                                {
                                    lastPoint = currentOutlineStartPoint;
                                }
                                else
                                {
                                    GlyphPointF next = ttfGlyph.GlyphPoints[i + 1];
                                    if (next.onCurve)
                                    {
                                        lastPoint = next;
                                    }
                                    else
                                    {
                                        // Calculate Midpoint
                                        lastPoint = new GlyphPointF(
                                            ((control.X + next.X) / 2),
                                            ((control.Y + next.Y) / 2),
                                            false
                                        );
                                    }
                                }
                                
                                var curve = GetCurve(start, control, lastPoint);
                                vecGlyph.Points.Data.Add(curve);
                            }
                            else
                            {
                                // Handle non-curves
                                IGlyphShapeData shape = CompareAdd(lastPoint, point);
                                vecGlyph.Points.Data.Add(shape);
                                lastPoint = point;
                            }
                        }
                    }

                    vecGlyph.Points.Data.Add(CompareAdd(ttfGlyph.GlyphPoints[^1], currentOutlineStartPoint));
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
                return new GlyphLine(-(next.Y - prev.Y) / Scale, GlyphAxis.Y);
            }
            else if (next.Y == prev.Y)
            {
                return new GlyphLine((next.X - prev.X) / Scale, GlyphAxis.X);
            }
            else
            {
                return new GlyphPoint((next.X - prev.X) / Scale, -(next.Y - prev.Y) / Scale);
            }
        }

        private static GlyphQuadraticBezierCurve GetCurve(GlyphPointF start, GlyphPointF control, GlyphPointF end)
        {
            float p1X = (control.X - start.X) / Scale;
            float p1Y = -(control.Y - start.Y) / Scale;

            float p2X = (end.X - control.X) / Scale;
            float p2Y = -(end.Y - control.Y) / Scale;

            return new GlyphQuadraticBezierCurve(p1X, p1Y, p2X, p2Y);
        }
    }
}
