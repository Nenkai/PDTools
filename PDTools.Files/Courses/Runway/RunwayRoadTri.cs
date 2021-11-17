using Syroot.BinaryData;

namespace PDTools.Files.Courses.Runway;

public class RunwayRoadTri
{
    public uint VertA { get; set; }
    public uint VertB { get; set; }
    public uint VertC { get; set; }
    public uint unk { get; set; }
    public byte SurfaceType { get; set; }
    public byte flagsA { get; set; }
    public byte flagsB { get; set; }
    public byte flagsC { get; set; }

    public static RunwayRoadTri FromStream(BinaryStream bs, ushort rwyVersionMajor, ushort rwyVersionMinor)
    {
        long basePos = bs.Position;
        RunwayRoadTri roadTri = new();

        if (rwyVersionMajor >= 4)
        {
            bs.Position = basePos + 0x1;
            roadTri.VertA = bs.ReadUInt16();
            roadTri.SurfaceType = bs.Read1Byte();

            bs.Position = basePos + 0x5;
            roadTri.VertB = bs.ReadUInt16();
            roadTri.flagsA = bs.Read1Byte();

            bs.Position = basePos + 0x9;
            roadTri.VertC = bs.ReadUInt16();
            roadTri.flagsB = bs.Read1Byte();
            roadTri.unk = bs.ReadUInt16();
            roadTri.flagsC = bs.Read1Byte();
        }
        else
        {
            roadTri.VertA = bs.ReadUInt32();
            roadTri.VertB = bs.ReadUInt32();
            roadTri.VertC = bs.ReadUInt32();
            roadTri.SurfaceType = bs.Read1Byte();
            roadTri.flagsA = bs.Read1Byte(); // 1 = Pit?

            bs.ReadUInt16();
            bs.ReadInt16();
            bs.ReadUInt16();

        }

        return roadTri;
    }

    public void ToStream(BinaryStream bs, ushort rwyVersionMajor, ushort rwyVersionMinor)
    {
        if (rwyVersionMajor >= 4)
        {
            bs.WriteByte(0);
            bs.WriteUInt16((ushort)VertA);
            bs.WriteByte(SurfaceType);
            bs.WriteByte(0);
            bs.WriteUInt16((ushort)VertB);
            bs.WriteByte(flagsA);
            bs.WriteByte(0);
            bs.WriteUInt16((ushort)VertC);
            bs.WriteByte(flagsB);
            bs.WriteUInt16((ushort)unk);
            bs.WriteByte(flagsC);
            bs.WriteByte(0);
        }
        else
        {
            bs.WriteUInt32(VertA);
            bs.WriteUInt32(VertB);
            bs.WriteUInt32(VertC);
            bs.WriteUInt32(0);
            bs.WriteUInt32(0);
        }
    }

    public static int GetSize(ushort rwyVersionMajor, ushort rwyVersionMinor)
    {
        if (rwyVersionMajor >= 4)
            return 0x10;
        else
            return 0x14;
    }
}

