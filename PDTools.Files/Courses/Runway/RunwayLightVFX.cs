using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;

using Syroot.BinaryData;

namespace PDTools.Files.Courses.Runway
{
    public class RunwayLightVFX
    {
        public short Index { get; set; }
        public byte TextureType { get; set; }
        public Vector3 Position { get; set; }
        public float UnkA { get; set; }
        public float UnkB { get; set; }
        public float UnkC { get; set; }
        public float UnkD { get; set; }

        public static RunwayLightVFX FromStream(BinaryStream bs, ushort rwyVersionMajor, ushort rwyVersionMinor)
        {
            var vfx = new RunwayLightVFX();
            vfx.Index = bs.ReadInt16();
            bs.Position += 1;
            vfx.TextureType = bs.Read1Byte();
            vfx.Position = new Vector3(bs.ReadSingle(), bs.ReadSingle(), bs.ReadSingle());
            vfx.UnkA = bs.ReadSingle();
            vfx.UnkB = bs.ReadSingle();
            vfx.UnkC = bs.ReadSingle();
            vfx.UnkD = bs.ReadSingle();

            return vfx;
        }

        public void ToStream(BinaryStream bs, ushort rwyVersionMajor, ushort rwyVersionMinor)
        {
            bs.WriteInt16(Index);
            bs.Position += 1;
            bs.WriteByte(TextureType);
            bs.WriteSingle(Position.X); bs.WriteSingle(Position.Y); bs.WriteSingle(Position.Z);
            bs.WriteSingle(UnkA);
            bs.WriteSingle(UnkB);
            bs.WriteSingle(UnkC);
            bs.WriteSingle(UnkD);
        }

        public static int GetSize(ushort rwyVersionMajor, ushort rwyVersionMinor)
        {
            return 0x20;
        }
    }
}
