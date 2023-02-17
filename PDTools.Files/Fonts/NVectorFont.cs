using Syroot.BinaryData;
using System.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PDTools.Utils;
using System.Numerics;

namespace PDTools.Files.Fonts
{
    public class NVectorFont
    {
        public byte Unk1 { get; set; }
        public byte Unk2 { get; set; }
        public byte Unk3 { get; set; }
        public byte Unk4 { get; set; }

        public List<char> Characters { get; set; } = new();
        public List<Glyph> Glyphs { get; set; } = new();

        public NVectorFont()
        {

        }

        public static NVectorFont Read(Stream stream)
        {
            BinaryStream bs = new BinaryStream(stream);
            bs.ByteConverter = ByteConverter.Big;

            if (bs.ReadUInt32() != 0x4E564543)
                throw new Exception("Not a vector font file");

            var vecFont = new NVectorFont();
            vecFont.Unk1 = bs.Read1Byte();
            vecFont.Unk2 = bs.Read1Byte();
            vecFont.Unk3 = bs.Read1Byte();
            vecFont.Unk4 = bs.Read1Byte();

            short charCount = bs.ReadInt16();
            short unk2 = bs.ReadInt16();
            int unk3 = bs.ReadInt32();
            int charsOffset = bs.ReadInt32();
            int glyphsOffset = bs.ReadInt32();
            int glyphDataOffset = bs.ReadInt32();
            bs.Position = charsOffset;

            for (var i = 0; i < charCount; i++)
            {
                short value = bs.ReadInt16();
                char c = (char)value;
                vecFont.Characters.Add(c);
            }

            for (var i = 0; i < charCount; i++)
            {
                bs.Position = glyphsOffset + (i * 0x14);
                Glyph glyph = Glyph.Read(bs, glyphDataOffset);
                vecFont.Glyphs.Add(glyph);
            }

            return vecFont;
        }

        public Glyph GetGlyphByChar(char ch)
        {
            int idx = Characters.IndexOf(ch);
            if (idx == -1)
                return null;

            return Glyphs[idx];
        }

        public void Write(Stream stream)
        {
            BinaryStream bs = new BinaryStream(stream, ByteConverter.Big);
            bs.WriteString("NVEC", StringCoding.Raw);
            bs.WriteByte(Unk1);
            bs.WriteByte(Unk2);
            bs.WriteByte(Unk3);
            bs.WriteByte(Unk4);
            bs.WriteUInt16((ushort)Glyphs.Count);

            // Skip header to write glyphs
            bs.Position = 0x20;

            int charsOffset = (int)bs.Position;
            for (var i = 0; i < Characters.Count; i++)
                bs.WriteUInt16((ushort)Characters[i]);
            bs.Align(0x08, grow: true);

            long lastOffset = bs.Position;

            bs.Position = 0x10;
            bs.WriteInt32(charsOffset);

            // Write each glyph data, glyphs info will be written after
            bs.Position = lastOffset;

            int glyphsOffset = (int)bs.Position;
            int glyphDataOffset = glyphsOffset + (Glyphs.Count * 0x14);
            int lastGlyphDataOffset = glyphDataOffset;

            for (var i = 0; i < Glyphs.Count; i++)
            {
                Glyph glyph = Glyphs[i];
                bs.Position = lastGlyphDataOffset;

                BitStream bitStream = new BitStream(BitStreamMode.Write);
                bitStream.WriteBits((ulong)MiscUtils.PackFloatToBitRange(glyph.Points.XMin, 12), 12);
                bitStream.WriteBits((ulong)MiscUtils.PackFloatToBitRange(glyph.Points.YMin, 12), 12);

                foreach (var shape in glyph.Points.Data)
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
                        ulong flags = 0x20 | 0x10;

                        int xBitSize = MiscUtils.GetHighestBitIndexOfPackedFloat(point.X);
                        int yBitSize = MiscUtils.GetHighestBitIndexOfPackedFloat(point.Y);

                        ulong elemSize = (ulong)Math.Max(xBitSize, yBitSize);
                        flags |= (ulong)Math.Max(elemSize - 2, 0);

                        bitStream.WriteBits(flags, 6);
                        bitStream.WriteBoolBit(true); // Is a point
                        bitStream.WriteBits((ulong)MiscUtils.PackFloatToBitRange(point.X, (int)elemSize), elemSize);
                        bitStream.WriteBits((ulong)MiscUtils.PackFloatToBitRange(point.Y, (int)elemSize), elemSize);
                    }
                    else if (shape is GlyphLine line)
                    {
                        ulong flags = 0x20 | 0x10;

                        MiscUtils.PackFloat(line.Distance, out int distValue, out int bitCount);

                        flags |= (ulong)Math.Max(bitCount - 2, 0);

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

                        int elemSize = new[]{ p1XSize, p1YSize, p2XSize, p2YSize }.Max();

                        flags |= (ulong)Math.Max(elemSize - 2, 0);
                        bitStream.WriteBits(flags, 6);
                        bitStream.WriteBoolBit(true); // Is a point
                        bitStream.WriteBits((ulong)MiscUtils.PackFloatToBitRange(curve.P1_DistX, (int)elemSize), (ulong)elemSize);
                        bitStream.WriteBits((ulong)MiscUtils.PackFloatToBitRange(curve.P1_DistY, (int)elemSize), (ulong)elemSize);
                        bitStream.WriteBits((ulong)MiscUtils.PackFloatToBitRange(curve.P2_DistX, (int)elemSize), (ulong)elemSize);
                        bitStream.WriteBits((ulong)MiscUtils.PackFloatToBitRange(curve.P2_DistY, (int)elemSize), (ulong)elemSize);
                    }                                                                 
                }

                var buffer = bitStream.GetBuffer();
                bs.Position = glyphsOffset + (i * 0x14);
            }
        }


    }
}
