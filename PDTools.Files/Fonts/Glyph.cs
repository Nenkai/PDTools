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
        public GlyphPoints Points { get; set; }

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

            glyph.Points = GlyphPoints.Read(bs, dataLength);

            return glyph;
        }

        public Image<Rgba32> GetAsImage()
        {
            using var image = new Image<Rgba32>(2048, 2048);
            image.Mutate(ctx =>
            {
                float currentX = 0, currentY = 0;

                var pen = new Pen(new SolidBrush(Color.Black), 2);
                foreach (var i in Points.Points)
                {
                    if (i is GlyphStartPoint startPoint)
                    {
                        currentX = startPoint.X + 1024;
                        currentY = startPoint.Y + 1024;
                    }
                    else if (i is GlyphPoint point)
                    {
                        ctx.DrawLines(pen, new PointF[] {
                            new PointF(currentX, currentY), 
                            new PointF(currentX + point.X, currentY + point.Y),
                        });

                        currentX += point.X;
                        currentY += point.Y;
                    }
                    else if (i is GlyphLine line)
                    {
                        if (line.Distance == 0)
                            continue;

                        if (line.Axis == 0)
                        {
                            ctx.DrawLines(pen, new PointF[] {
                                new PointF(currentX, currentY),
                                new PointF(currentX + line.Distance, currentY),
                            });

                            currentX += line.Distance;
                        }
                        else if (line.Axis == GlyphAxis.Y)
                        {
                             ctx.DrawLines(pen, new PointF[] {
                                new PointF(currentX, currentY),
                                new PointF(currentX, currentY + line.Distance),
                            });

                            currentY += line.Distance;
                        }
                    }
                    else if (i is GlyphCurve curve)
                    {
                        float controlX = currentX + curve.P1;
                        float controlY = currentY + curve.P2;

                        // Draw line to next
                        ctx.DrawLines(new Pen(new SolidBrush(Color.Red), 2), new PointF[] {
                            new PointF(currentX, currentY),
                            new PointF(currentX + curve.P1 + curve.P3, currentY + curve.P2 + curve.P4),
                        });

                        // Draw control point/lines
                        ctx.DrawLines(new Pen(new SolidBrush(Color.DarkRed), 2), new PointF[] {
                            new PointF(currentX, currentY),
                            new PointF(currentX + curve.P1, currentY + curve.P2),
                        });

                        ctx.DrawLines(new Pen(new SolidBrush(Color.DarkRed), 2), new PointF[] {
                            new PointF(currentX + curve.P1, currentY + curve.P2),
                            new PointF((currentX + curve.P1) + curve.P3, (currentY + curve.P2) + curve.P4),
                        });

                        // Draw curve

                        // TODO: Figure multiple curves
                        ctx.DrawBeziers(pen,
                            new PointF(currentX, currentY), // Start
                            new PointF(controlX, controlY),
                            new PointF(controlX, controlY),
                            new PointF(controlX + curve.P3, controlY + curve.P4) // End
                        );

                        currentX = currentX + curve.P1 + curve.P3;
                        currentY = currentY + curve.P2 + curve.P4;
                    }
                }
            });

            return image;
        }
    }
}
