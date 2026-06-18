using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

using Syroot.BinaryData;

namespace PDTools.Files.Courses.PS2.Runway;

public class RunwayRoadVert
{
    public Vector3 Vertex { get; set; }
    public short Unk { get; set; }
    public byte Unk2 { get; set; }
    public byte Unk3 { get; set; }
    public static RunwayRoadVert FromStream(BinaryStream bs)
    {
        RunwayRoadVert vert = new RunwayRoadVert();
        vert.Vertex = new Vector3(bs.ReadSingle(), bs.ReadSingle(), bs.ReadSingle());
        vert.Unk = bs.ReadInt16();
        vert.Unk2 = bs.Read1Byte();
        vert.Unk3 = bs.Read1Byte();

        return vert;
    }

    public void ToStream(BinaryStream bs)
    {
        bs.WriteSingle(Vertex.X);
        bs.WriteSingle(Vertex.Y);
        bs.WriteSingle(Vertex.Z);
        bs.WriteInt16(Unk);
        bs.WriteByte(Unk2);
        bs.WriteByte(Unk3);
    }

    public static int GetSize()
    {
        return 0x10;
    }
}
