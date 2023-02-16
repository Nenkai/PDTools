using Syroot.BinaryData;
using System.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PDTools.Utils;

namespace PDTools.Files.Fonts
{
    public class GlyphPoints
    {
        public float XMin { get; set; }
        public float YMin { get; set; }

        public List<IGlyphShapeData> Points { get; set; } = new();

        public static GlyphPoints Read(BinaryStream bs, int size)
        {
            var data = new GlyphPoints();
            byte[] bytes = bs.ReadBytes(size);
            BitStream bitStream = new BitStream(BitStreamMode.Read, bytes);

            ulong xMinBits = bitStream.ReadBits(12);
            data.XMin = BitValueToFloat((long)xMinBits, 12);

            ulong yMinBits = bitStream.ReadBits(12);
            data.YMin = BitValueToFloat((long)yMinBits, 12);

            while (true)
            {
                int flags = (int)bitStream.ReadBits(6);
                if (flags == 0)
                    break;

                if ((flags & 0x20) != 0)
                {
                    int elemSize = (flags & 0x0F) + 2;
                    if ((flags & 0x10) != 0)
                    {
                        bool isPoint = bitStream.ReadBoolBit();
                        if (isPoint)
                        {
                            GlyphPoint point = new GlyphPoint();

                            int x = (int)bitStream.ReadBits(elemSize);
                            point.X = BitValueToFloat(x, elemSize);

                            int y = (int)bitStream.ReadBits(elemSize);
                            point.Y = BitValueToFloat(y, elemSize);

                            data.Points.Add(point);
                        }
                        else
                        {
                            GlyphLine line = new GlyphLine();

                            line.Axis = bitStream.ReadBoolBit() ? GlyphAxis.Y : GlyphAxis.X;
                            int dist = (int)bitStream.ReadBits(elemSize);
                            line.Distance = BitValueToFloat(dist, elemSize);

                            data.Points.Add(line);

                        }
                    }
                    else
                    {
                        GlyphCurve curve = new GlyphCurve();
                        int x1 = (int)bitStream.ReadBits(elemSize);
                        curve.P1 = BitValueToFloat(x1, elemSize);

                        int y1 = (int)bitStream.ReadBits(elemSize);
                        curve.P2 = BitValueToFloat(y1, elemSize);

                        int x2 = (int)bitStream.ReadBits(elemSize);
                        curve.P3 = BitValueToFloat(x2, elemSize);

                        int y2 = (int)bitStream.ReadBits(elemSize);
                        curve.P4 = BitValueToFloat(y2, elemSize);

                        data.Points.Add(curve);
                    }
                }
                else if ((flags & 0x01) != 0)
                {
                    GlyphStartPoint startPoint = new GlyphStartPoint();

                    int elemSize = (int)bitStream.ReadBits(5);
                    int x = (int)bitStream.ReadBits(elemSize);
                    startPoint.X = BitValueToFloat(x, elemSize);

                    int y = (int)bitStream.ReadBits(elemSize);
                    startPoint.Y = BitValueToFloat(y, elemSize);

                    if ((flags & 0x02) != 0)
                        startPoint.Unk = bitStream.ReadBoolBit();

                    if ((flags & 0x04) != 0)
                        startPoint.Unk2 = bitStream.ReadBoolBit();

                    data.Points.Add(startPoint);
                }
            }

            return data;

        }

        private static float BitValueToFloat(long value, int bitCount)
        {
            int maxVal = (int)Math.Pow(2, bitCount);

            if (value > ((maxVal / 2) - 1))
                value -= maxVal;

            return (float)value;
        }
    }

    public enum GlyphAxis
    {
        X,
        Y
    }
}
