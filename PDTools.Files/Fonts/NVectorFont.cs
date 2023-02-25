using Syroot.BinaryData;
using System.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;
using PDTools.Crypto;
using System.Diagnostics;

namespace PDTools.Files.Fonts
{
    /// <summary>
    /// Represents a bit-packed, vectored font format.
    /// </summary>
    public class NVectorFont
    {
        public byte UnkUnused1 { get; set; }
        public byte UnkUnused2 { get; set; }
        public byte UnkUnused3 { get; set; }
        public byte UnkUnused4 { get; set; }

        public List<char> Characters { get; private set; } = new();
        public List<Glyph> Glyphs { get; private set; } = new();

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
            vecFont.UnkUnused1 = bs.Read1Byte();
            vecFont.UnkUnused2 = bs.Read1Byte();
            vecFont.UnkUnused3 = bs.Read1Byte();
            vecFont.UnkUnused4 = bs.Read1Byte();

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
            bs.WriteByte(UnkUnused1);
            bs.WriteByte(UnkUnused2);
            bs.WriteByte(UnkUnused3);
            bs.WriteByte(UnkUnused4);
            bs.WriteUInt16((ushort)Glyphs.Count);
            bs.WriteInt16(0x385);
            bs.WriteInt16(0x7B);
            bs.WriteInt16(0);

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
            WriteGlyphs(bs);

            // In older GTs that weird data is in its own file with extension cvs
            int unkCvsDataOffset = (int)bs.Position;
            bs.Write(new byte[0x2284]);

            while (bs.Position % 0x10 != 0)
                bs.WriteByte(0xFF);

            bs.Position = 0x1C;
            bs.WriteInt32(unkCvsDataOffset);
        }

        private void WriteGlyphs(BinaryStream bs)
        {
            int glyphsOffset = (int)bs.Position;
            int mainGlyphDataOffset = glyphsOffset + (Glyphs.Count * 0x14);
            int lastGlyphDataOffset = mainGlyphDataOffset;

            // Some glyphs are equal, they're refered to again
            // So keep a crc of each written glyph
            Dictionary<uint, (int GlyphDataSize, int GlyphRelativeDataOffset)> checksumToGlyphOffsetAndSize = new();

            for (var i = 0; i < Glyphs.Count; i++)
            {
                Glyph glyph = Glyphs[i];
                bs.Position = lastGlyphDataOffset;

                byte[] buffer = glyph.Points.Write(bs);
                uint glyphBufferCrc = CRC32.CRC32_0x04C11DB7(buffer);

                int glyphDataSize, glyphDataOffset;
                if (!checksumToGlyphOffsetAndSize.TryGetValue(glyphBufferCrc, out (int GlyphDataSize, int GlyphRelativeDataOffset) value))
                {
                    bs.Write(buffer);

                    glyphDataSize = buffer.Length;
                    glyphDataOffset = lastGlyphDataOffset - mainGlyphDataOffset;
                    checksumToGlyphOffsetAndSize.Add(glyphBufferCrc, (glyphDataSize, glyphDataOffset));
                }
                else
                {
                    glyphDataSize = value.GlyphDataSize;
                    glyphDataOffset = value.GlyphRelativeDataOffset;
                }

                lastGlyphDataOffset = (int)bs.Position;

                bs.Position = glyphsOffset + (i * 0x14);
                bs.WriteUInt16(glyph.Character);
                bs.WriteUInt16(glyph.Flags);
                bs.WriteInt32(0);
                bs.WriteInt32(glyphDataSize);

                uint bits = 0;
                bits |= (uint)((glyph.Width & 0b1111_11111111) << 20); // 12 bits
                bits |= (uint)((glyph.HeightOffset & 0b1111_11111111) << 8); // 12 bits
                bits |= (uint)((glyphDataSize + 0x1F) / 0x10 & 0b11111111); // 8 bit
                bs.WriteUInt32(bits);

                bs.WriteInt32((int)glyphDataOffset);
            }

            bs.Position = 0x14;
            bs.WriteInt32(glyphsOffset);

            bs.Position = 0x18;
            bs.WriteInt32(mainGlyphDataOffset);

            bs.Position = lastGlyphDataOffset;
        }

        /// <summary>
        /// Adds the glyph to the font (and recalculates the glyph's bounds/width)
        /// </summary>
        /// <param name="glyph"></param>
        /// <exception cref="Exception"></exception>
        public void AddGlyph(Glyph glyph)
        {
            if (Characters.Contains(glyph.Character))
                throw new Exception($"Glyph '{glyph.Character}' already exists");

            glyph.Points.RecalculateBounds();
            glyph.Width = (ushort)(glyph.Points.XMax - glyph.Points.XMin);

            Characters.Add(glyph.Character);
            Glyphs.Add(glyph);
        }
    }
}
