using Syroot.BinaryData;
using System.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.Drawing;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.Fonts;

namespace PDTools.Files.Fonts
{
    public class Glyph
    {
       public char Character { get; set; }

        public ushort Flags { get; set; }
        public GlyphShapes Points { get; set; }

        public static Glyph Read(BinaryStream bs, int dataOffset)
        {
            var glyph = new Glyph();
            glyph.Character = (char)bs.ReadInt16();
            glyph.Flags = (ushort)bs.ReadInt16();
            bs.ReadInt32();

            int dataLength = bs.ReadInt32();
            bs.ReadInt32();

            int offsetWithinData = bs.ReadInt32();

            bs.Position = dataOffset + offsetWithinData;

            glyph.Points = GlyphShapes.Read(bs, dataLength);

            return glyph;
        }

        public Image<Rgba32> GetAsImage()
        {
            using var image = new Image<Rgba32>(2048, 2048);
            image.Mutate(ctx =>
            {
                float currentX = 0, currentY = 0;

                PathBuilder path = new PathBuilder();
                bool originSet = false;

                var pen = new Pen(new SolidBrush(Color.Black), 2);
                for (int i1 = 0; i1 < Points.Data.Count; i1++)
                {
                    IGlyphShapeData? i = Points.Data[i1];
                    if (i is GlyphStartPoint startPoint)
                    {
                        currentX = startPoint.X + 1024;
                        currentY = startPoint.Y + 1024;

                        ctx.Draw(Color.Red, 2, path.Build());
                        path.Clear();
                        path.CloseAllFigures();

                    }
                    else if (i is GlyphPoint point)
                    {
                        path.AddLine(new PointF(currentX, currentY), new PointF(currentX + point.X, currentY + point.Y));

                        currentX += point.X;
                        currentY += point.Y;
                    }
                    else if (i is GlyphLine line)
                    {
                        if (line.Distance == 0)
                        {
                            continue;
                        }

                        if (line.Axis == 0)
                        {
                            path.AddLine(new PointF(currentX, currentY), new PointF(currentX + line.Distance, currentY));

                            currentX += line.Distance;
                        }
                        else if (line.Axis == GlyphAxis.Y)
                        {
                            path.AddLine(new PointF(currentX, currentY), new PointF(currentX, currentY + line.Distance));

                            currentY += line.Distance;
                        }
                    }
                    else if (i is GlyphQuadraticBezierCurve curve)
                    {
                        float controlX = currentX + curve.P1_DistX;
                        float controlY = currentY + curve.P1_DistY;

                        // Draw curve
                        path.AddQuadraticBezier(new PointF(currentX, currentY),
                            new PointF(controlX, controlY),
                            new PointF(controlX + curve.P2_DistX, controlY + curve.P2_DistY));

                        currentX = currentX + curve.P1_DistX + curve.P2_DistX;
                        currentY = currentY + curve.P1_DistY + curve.P2_DistY;
                    }

                    ctx.Draw(Color.Red, 5, path.Build());
                }
            });

            return image;
        }
    }
}
