using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Syroot.BinaryData;

namespace PDTools.Files.Models.Shaders
{
    public class ShaderProgramEntry_0x2C_0x14
    {
        public string Name { get; set; }
        public short Unk { get; set; }
        public short Unk2 { get; set; }
        public short Unk3 { get; set; }
        public short Unk4 { get; set; }
        public int Unk5 { get; set; }

        public static ShaderProgramEntry_0x2C_0x14 FromStream(BinaryStream bs, long basePos)
        {
            var entry = new ShaderProgramEntry_0x2C_0x14();
            int nameOffset = bs.ReadInt32();
            entry.Unk = bs.ReadInt16();
            entry.Unk2 = bs.ReadInt16();
            entry.Unk3 = bs.ReadInt16();
            entry.Unk4 = bs.ReadInt16();
            entry.Unk5 = bs.ReadInt32();

            bs.Position = basePos + nameOffset;
            entry.Name = bs.ReadString(StringCoding.ZeroTerminated);

            return entry;
        }

        public override string ToString()
        {
            return $"{Name} (Unk: {Unk}, Unk2: {Unk2}, Unk3: {Unk3}, Unk4: {Unk4}, Unk5: {Unk5})";
        }
    }
}
