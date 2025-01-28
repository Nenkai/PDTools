using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

using Syroot.BinaryData;

namespace PDTools.Files.Courses.PS2.Runway;

public class RunwayRoadTri
{
    public ushort Vert1 { get; set; }
    public ushort Vert2 { get; set; }
    public ushort Vert3 { get; set; }
    public byte UnkBits { get; set; } // Returned from runway search - 5 bits and 3 bits
    public byte Unk { get; set; } // Returned from runway search
    public uint Flags { get; set; }

    public static RunwayRoadTri FromStream(BinaryStream bs)
    {
        RunwayRoadTri tri = new RunwayRoadTri();
        tri.Vert1 = bs.ReadUInt16();
        tri.Vert2 = bs.ReadUInt16();
        tri.Vert3 = bs.ReadUInt16();
        tri.UnkBits = bs.Read1Byte();
        tri.Unk = bs.Read1Byte();
        tri.Flags = bs.ReadUInt32();
        return tri;
    }

    public static int GetSize()
    {
        return 0x0C;
    }
}
