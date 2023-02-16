using Syroot.BinaryData;
using System.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
    }
}
