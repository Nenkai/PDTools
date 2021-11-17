using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Syroot.BinaryData;

namespace PDTools.Files.Courses.Runway;

public class RunwayBoundaryVert
{
    public Vector3 Position { get; set; }
    public short counter;
    public ushort flags;

    public static RunwayBoundaryVert FromStream(BinaryStream bs, ushort versionMajor, ushort versionMinor)
    {
        RunwayBoundaryVert boundaryVert = new();
        boundaryVert.Position = new(bs.ReadSingle(), bs.ReadSingle(), bs.ReadSingle());
        boundaryVert.counter = bs.ReadInt16();
        boundaryVert.flags = bs.ReadUInt16();
        return boundaryVert;
    }

    public void ToStream(BinaryStream bs, ushort versionMajor, ushort versionMinor)
    {
        bs.WriteSingle(Position.X);
        bs.WriteSingle(Position.Y);
        bs.WriteSingle(Position.Z);
        bs.WriteInt16(counter);
        bs.WriteUInt16(flags);
    }

    public static int GetSize(ushort versionMajor, ushort versionMinor)
    {
        return 0x10;
    }
}

