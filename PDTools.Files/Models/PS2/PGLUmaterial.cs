using Syroot.BinaryData;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDTools.Files.Models.PS2
{
    public class PGLUmaterial
    {
        public float[] Values { get; set; } = new float[16];
        public int[] Unk { get; set; } = new int[2];
        public int Unk2 { get; set; }
        public int Unk3 { get; set; }

        public void FromStream(BinaryStream bs, long mdlBasePos)
        {
            Values = bs.ReadSingles(16);
            Unk = bs.ReadInt32s(2);
            Unk2 = bs.ReadInt32();
            Unk3 = bs.ReadInt32();
        }

        public void Write(BinaryStream bs)
        {
            bs.WriteSingles(Values);
            bs.WriteInt32s(Unk);
            bs.WriteInt32(Unk2);
            bs.WriteInt32(Unk3);
        }

        public static int GetSize()
        {
            return 0x50;
        }
    }
}
