using Syroot.BinaryData;
using System.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PDTools.Utils;
using SixLabors.Fonts;

namespace PDTools.Files.Fonts
{
    public class GlyphShapes
    {
        public float XMin { get; set; }
        public float YMin { get; set; }

        /// <summary>
        /// For debugging; not used
        /// </summary>
        public float XMax { get; set; }

        /// <summary>
        /// For debugging, not used
        /// </summary>
        public float YMax { get; set; }

        public List<IGlyphShapeData> Data { get; set; } = new();

        public static GlyphShapes Read(BinaryStream bs, int size)
        {
            var data = new GlyphShapes();
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

                            data.Data.Add(point);
                        }
                        else
                        {
                            GlyphLine line = new GlyphLine();

                            line.Axis = bitStream.ReadBoolBit() ? GlyphAxis.Y : GlyphAxis.X;
                            int dist = (int)bitStream.ReadBits(elemSize);
                            line.Distance = BitValueToFloat(dist, elemSize);

                            data.Data.Add(line);

                        }
                    }
                    else
                    {
                        GlyphQuadraticBezierCurve curve = new GlyphQuadraticBezierCurve();
                        int x1 = (int)bitStream.ReadBits(elemSize);
                        curve.P1_DistX = BitValueToFloat(x1, elemSize);

                        int y1 = (int)bitStream.ReadBits(elemSize);
                        curve.P1_DistY = BitValueToFloat(y1, elemSize);

                        int x2 = (int)bitStream.ReadBits(elemSize);
                        curve.P2_DistX = BitValueToFloat(x2, elemSize);

                        int y2 = (int)bitStream.ReadBits(elemSize);
                        curve.P2_DistY = BitValueToFloat(y2, elemSize);

                        data.Data.Add(curve);
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

                    data.Data.Add(startPoint);
                }
            }

            return data;

        }

        public byte[] Write(BinaryStream bs)
        {
            BitStream bitStream = new BitStream(BitStreamMode.Write);
            bitStream.WriteBits((ulong)MiscUtils.PackFloatToBitRange(XMin, 12), 12);
            bitStream.WriteBits((ulong)MiscUtils.PackFloatToBitRange(YMin, 12), 12);

            foreach (var shape in Data)
            {
                if (shape is GlyphStartPoint startPoint)
                {
                    int flags = 0x01;
                    if (startPoint.Unk != null)
                        flags |= 0x02;

                    if (startPoint.Unk2 != null)
                        flags |= 0x04;

                    bitStream.WriteBits((ulong)flags, 6);

                    int xBitSize = MiscUtils.GetHighestBitIndexOfPackedFloat(startPoint.X);
                    int yBitSize = MiscUtils.GetHighestBitIndexOfPackedFloat(startPoint.Y);

                    ulong elemSize = (ulong)Math.Max(xBitSize, yBitSize);

                    bitStream.WriteBits(elemSize, 5);
                    bitStream.WriteBits((ulong)MiscUtils.PackFloatToBitRange(startPoint.X, (int)elemSize), elemSize);
                    bitStream.WriteBits((ulong)MiscUtils.PackFloatToBitRange(startPoint.Y, (int)elemSize), elemSize);

                    if (startPoint.Unk != null)
                        bitStream.WriteBoolBit((bool)startPoint.Unk);

                    if (startPoint.Unk2 != null)
                        bitStream.WriteBoolBit((bool)startPoint.Unk2);
                }
                else if (shape is GlyphPoint point)
                {
                    uint flags = 0x20 | 0x10;

                    int xBitSize = MiscUtils.GetHighestBitIndexOfPackedFloat(point.X);
                    int yBitSize = MiscUtils.GetHighestBitIndexOfPackedFloat(point.Y);

                    int elemSize = (int)Math.Max(xBitSize, yBitSize);
                    flags |= (uint)Math.Max(elemSize - 2, 0);

                    bitStream.WriteBits(flags, 6);
                    bitStream.WriteBoolBit(true); // Is a point
                    bitStream.WriteBits((ulong)MiscUtils.PackFloatToBitRange(point.X, (int)elemSize), (ulong)elemSize);
                    bitStream.WriteBits((ulong)MiscUtils.PackFloatToBitRange(point.Y, (int)elemSize), (ulong)elemSize);
                }
                else if (shape is GlyphLine line)
                {
                    uint flags = 0x20 | 0x10;

                    MiscUtils.PackFloat(line.Distance, out int distValue, out int bitCount);

                    flags |= (uint)Math.Max(bitCount - 2, 0);

                    bitStream.WriteBits(flags, 6);
                    bitStream.WriteBoolBit(false); // Not a point
                    bitStream.WriteBits((ulong)line.Axis, 1);
                    bitStream.WriteBits((ulong)distValue, (ulong)Math.Max(bitCount, 2));
                }
                else if (shape is GlyphQuadraticBezierCurve curve)
                {
                    ulong flags = 0x20;

                    int p1XSize = MiscUtils.GetHighestBitIndexOfPackedFloat(curve.P1_DistX);
                    int p1YSize = MiscUtils.GetHighestBitIndexOfPackedFloat(curve.P1_DistY);
                    int p2XSize = MiscUtils.GetHighestBitIndexOfPackedFloat(curve.P2_DistX);
                    int p2YSize = MiscUtils.GetHighestBitIndexOfPackedFloat(curve.P2_DistY);

                    int elemSize = new[] { p1XSize, p1YSize, p2XSize, p2YSize }.Max();

                    flags |= (uint)Math.Max(elemSize - 2, 0);
                    bitStream.WriteBits(flags, 6);
                    bitStream.WriteBits((ulong)MiscUtils.PackFloatToBitRange(curve.P1_DistX, (int)elemSize), (ulong)elemSize);
                    bitStream.WriteBits((ulong)MiscUtils.PackFloatToBitRange(curve.P1_DistY, (int)elemSize), (ulong)elemSize);
                    bitStream.WriteBits((ulong)MiscUtils.PackFloatToBitRange(curve.P2_DistX, (int)elemSize), (ulong)elemSize);
                    bitStream.WriteBits((ulong)MiscUtils.PackFloatToBitRange(curve.P2_DistY, (int)elemSize), (ulong)elemSize);
                }
            }

            bitStream.WriteBits(0, 6);
            bitStream.AlignToNextByte();

            if (bitStream.GetBuffer()[^1] != 0)
                bitStream.WriteByte(0);

            var buffer = bitStream.GetBuffer();
            return buffer.ToArray();
        }

        public void RecalculateBounds()
        {
            float minX = 0, minY = 0;
            float maxX = 0, maxY = 0;

            float currentX = 0, currentY = 0;
            for (int i1 = 0; i1 < Data.Count; i1++)
            {
                IGlyphShapeData? i = Data[i1];
                if (i is GlyphStartPoint startPoint)
                {
                    if (startPoint.Unk == true)
                    {
                        minX = startPoint.X;
                        minY = startPoint.Y;
                        maxX = startPoint.X;
                        maxY = startPoint.Y;
                    }
                    
                    currentX = startPoint.X;
                    currentY = startPoint.Y;
                }
                else if (i is GlyphPoint point)
                {
                    currentX += point.X;
                    currentY += point.Y;
                }
                else if (i is GlyphLine line)
                {
                    if (line.Distance == 0)
                        continue;

                    if (line.Axis == 0)
                        currentX += line.Distance;
                    else if (line.Axis == GlyphAxis.Y)
                        currentY += line.Distance;
                }
                else if (i is GlyphQuadraticBezierCurve curve)
                {
                    currentX = currentX + curve.P1_DistX + curve.P2_DistX;
                    currentY = currentY + curve.P1_DistY + curve.P2_DistY;
                }

                if (currentX < minX) minX = currentX;
                if (currentY < minY) minY = currentY;
                if (currentX > maxX) maxX = currentX;
                if (currentY > maxY) maxY = currentY;
            }

            XMin = minX;
            YMin = minY;
            XMax = maxX;
            YMax = maxY;
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
