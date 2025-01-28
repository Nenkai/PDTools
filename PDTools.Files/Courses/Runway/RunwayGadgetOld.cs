using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Numerics;

using Syroot.BinaryData;

namespace PDTools.Files.Courses.Runway;

public class RunwayGadgetOld
{
    public Vector3 Position { get; set; }
    public float Angle { get; set; }
    public uint Flag { get; set; }

    public static RunwayGadgetOld FromStream(BinaryStream bs, ushort rwyVersionMajor, ushort rwyVersionMinor)
    {
        var unk = new RunwayGadgetOld();
        bs.Position += 8;
        unk.Position = new Vector3(bs.ReadSingle(), bs.ReadSingle(), bs.ReadSingle());
        bs.Position += 4;
        unk.Angle = bs.ReadSingle();
        unk.Flag = bs.ReadUInt32();

        return unk;
    }

    public void ToStream(BinaryStream bs, ushort rwyVersionMajor, ushort rwyVersionMinor)
    {
        bs.WriteInt32(0);
        bs.WriteInt32(0);
        bs.WriteSingle(Position.X); bs.WriteSingle(Position.Y); bs.WriteSingle(Position.Z);
        bs.WriteInt32(0);
        bs.WriteSingle(Angle);
        bs.WriteUInt32(Flag);
    }

    public static int GetSize(ushort rwyVersionMajor, ushort rwyVersionMinor)
    {
        return 0x20;
    }
}
