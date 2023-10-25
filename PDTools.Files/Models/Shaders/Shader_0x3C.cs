using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Syroot.BinaryData;

namespace PDTools.Files.Models.Shaders
{
    public class Shaders_0x3C
    {
        public string Name { get; set; }
        public int Unk1 { get; set; }
        public int Unk2 { get; set; }
        public short Unk3 { get; set; }
        public short Unk4 { get; set; }
        public byte[] Unk5 { get; set; }

        public static Shaders_0x3C FromStream(BinaryStream bs, long basePos)
        {
            var entry = new Shaders_0x3C();
            int nameOffset = bs.ReadInt32();
            entry.Unk1 = bs.ReadInt32();
            entry.Unk2 = bs.ReadInt32();

            int unkOffset_0x0C = bs.ReadInt32();
            bs.Position += 4 * sizeof(int);
            entry.Unk3 = bs.ReadInt16();
            entry.Unk4 = bs.ReadInt16();
            entry.Unk5 = bs.ReadBytes(3);

            bs.Position = basePos + nameOffset;
            entry.Name = bs.ReadString(StringCoding.ZeroTerminated);

            return entry;
        }
    }
}
