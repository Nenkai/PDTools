using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;
using Syroot.BinaryData;

namespace PDTools.Files.Courses.Runway;

public class RunwayLightDefinition
{
    public short UnkIndex { get; set; }
    public byte UnkCount { get; set; }
    public Vector3 Position { get; set; }
    public Vector3 Direction { get; set; }
    public float Unk { get; set; }

    public static RunwayLightDefinition FromStream(BinaryStream bs, ushort rwyVersionMajor, ushort rwyVersionMinor)
    {
        var lightSet = new RunwayLightDefinition();
        lightSet.UnkIndex = bs.ReadInt16();
        bs.Position += 1;
        lightSet.UnkCount = bs.Read1Byte();
        lightSet.Position = new Vector3(bs.ReadSingle(), bs.ReadSingle(), bs.ReadSingle());
        lightSet.Direction = new Vector3(bs.ReadSingle(), bs.ReadSingle(), bs.ReadSingle());
        lightSet.Unk = bs.ReadSingle();

        return lightSet;
    }

    public void ToStream(BinaryStream bs, ushort rwyVersionMajor, ushort rwyVersionMinor)
    {
        bs.WriteInt16(UnkIndex);
        bs.Position += 1;
        bs.WriteByte(UnkCount);
        bs.WriteSingle(Position.X); bs.WriteSingle(Position.Y); bs.WriteSingle(Position.Z);
        bs.WriteSingle(Direction.X); bs.WriteSingle(Direction.Y); bs.WriteSingle(Direction.Z);
        bs.WriteSingle(Unk);
    }

    public static int GetSize(ushort rwyVersionMajor, ushort rwyVersionMinor)
    {
        return 0x20;
    }
}
