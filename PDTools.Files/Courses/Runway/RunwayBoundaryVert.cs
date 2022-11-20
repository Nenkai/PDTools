using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Syroot.BinaryData;

namespace PDTools.Files.Courses.Runway;

public class RunwayBoundaryVert
{
    public Vector3 Position { get; set; }
    public Vector3 UnkVersion6Vec { get; set; }
    public short counter;
    public ushort flags;

    public static RunwayBoundaryVert FromStream(BinaryStream bs, RunwayFile rwy)
    {
        RunwayBoundaryVert boundaryVert = new();

        if (rwy.VersionMajor >= 6 && (rwy.BoundaryVertScaleX > 0 || rwy.BoundaryVertScaleY > 0 || rwy.BoundaryVertScaleZ > 0))
        {
            float X = (float)RunwayFile.DecodeAVXFloat(bs.ReadInt32(), rwy.BBoxAndRoadVertScaleX);
            float Y = (float)RunwayFile.DecodeAVXFloat(bs.ReadInt32(), rwy.BBoxAndRoadVertScaleY);
            float Z = (float)RunwayFile.DecodeAVXFloat(bs.ReadInt32(), rwy.BBoxAndRoadVertScaleZ);

            boundaryVert.Position = new(X, Y, Z);
        }
        else
        {
            boundaryVert.Position = new(bs.ReadSingle(), bs.ReadSingle(), bs.ReadSingle());
        }

        if (rwy.VersionMajor >= 6)
            boundaryVert.UnkVersion6Vec = new(bs.ReadSingle(), bs.ReadSingle(), bs.ReadSingle());

        boundaryVert.counter = bs.ReadInt16();
        boundaryVert.flags = bs.ReadUInt16();
        return boundaryVert;
    }

    public void ToStream(BinaryStream bs, RunwayFile rwy)
    {
        bs.WriteSingle(Position.X);
        bs.WriteSingle(Position.Y);
        bs.WriteSingle(Position.Z);

        if (rwy.VersionMajor >= 6)
            throw new NotImplementedException("Implement writing extra vector for version >6 in boundary verts");

        bs.WriteInt16(counter);
        bs.WriteUInt16(flags);
    }

    public static int GetSize(RunwayFile rwy)
    {
        if (rwy.VersionMajor >= 6)
        {
            return 0x1C;
        }
        else
        {
            return 0x10;
        }
    }
}

