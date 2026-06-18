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
    /// <summary>
    /// Byte packed as [7:5] SectorId (3 bits, 0–7) | [4:0] CpSubIndex (5 bits, 0–31).
    /// Returned in the runway-search result as TriUnk (sector) / TriUnk2 (sub-index).
    /// SectorId identifies the in-game timing sector (0 = T1, 1 = T2, …) for this triangle.
    /// CpSubIndex is the local checkpoint sub-index used for V-coord interpolation.
    /// </summary>
    public byte UnkBits { get; set; }

    /// <summary>High 3 bits of <see cref="UnkBits"/>: in-game timing sector ID (0–7).</summary>
    public byte SectorId    => (byte)(UnkBits >> 5);

    /// <summary>Low 5 bits of <see cref="UnkBits"/>: checkpoint sub-index within the sector (0–31).</summary>
    public byte CpSubIndex  => (byte)(UnkBits & 0x1F);

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

    public void ToStream(BinaryStream bs)
    {
        bs.WriteUInt16(Vert1);
        bs.WriteUInt16(Vert2);
        bs.WriteUInt16(Vert3);
        bs.WriteByte(UnkBits);
        bs.WriteByte(Unk);
        bs.WriteUInt32(Flags);
    }

    public static int GetSize()
    {
        return 0x0C;
    }
}
