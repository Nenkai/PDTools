using Syroot.BinaryData;

namespace PDTools.Files.Courses.Runway;

public class RunwayRoadTri
{
    public uint VertA { get; set; }
    public uint VertB { get; set; }
    public uint VertC { get; set; }
    public byte SurfaceType { get; set; }
    public byte flagsA { get; set; }
    public byte unk { get; set; }
    public uint unkBits { get; set; }
    public byte unk2 { get; set; } // usually 0xFF

    public static RunwayRoadTri FromStream(BinaryStream bs, ushort rwyVersionMajor, ushort rwyVersionMinor)
    {
        long basePos = bs.Position;
        RunwayRoadTri roadTri = new();

        if (rwyVersionMajor >= 6)
        {
            uint val = bs.ReadUInt32();
            roadTri.VertA = val >> 8;
            roadTri.SurfaceType = (byte)(val & 0xFF);

            val = bs.ReadUInt32();
            roadTri.VertB = val >> 8;
            roadTri.flagsA = (byte)(val & 0xFF);

            val = bs.ReadUInt32();
            roadTri.VertC = val >> 8;
            roadTri.unk = (byte)(val & 0xFF);
            roadTri.unkBits = bs.ReadUInt32();

            roadTri.unk2 = bs.Read1Byte(); // 0xFF
            bs.Position += 3; // Empty
        }
        else if (rwyVersionMajor >= 4)
        {
            bs.Position = basePos + 0x1;
            roadTri.VertA = bs.ReadUInt16();
            roadTri.SurfaceType = bs.Read1Byte();

            bs.Position = basePos + 0x5;
            roadTri.VertB = bs.ReadUInt16();
            roadTri.flagsA = bs.Read1Byte();

            bs.Position = basePos + 0x9;
            roadTri.VertC = bs.ReadUInt16();
            roadTri.unk = bs.Read1Byte();
            roadTri.unkBits = bs.ReadUInt32();
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
        if (rwyVersionMajor >= 6)
        {
            bs.WriteUInt32(VertA << 8 | (byte)SurfaceType);
            bs.WriteUInt32(VertB << 8 | (byte)flagsA);
            bs.WriteUInt32(VertC << 8 | (byte)unk);
            bs.WriteUInt32(unkBits);
            bs.WriteByte(unk2);
            bs.Position += 3;
        }
        else if (rwyVersionMajor >= 4)
        {
            bs.WriteByte(0); bs.WriteUInt16((ushort)VertA); // int24 cheesing
            bs.WriteByte(SurfaceType);
            bs.WriteByte(0); bs.WriteUInt16((ushort)VertB);
            bs.WriteByte(flagsA);
            bs.WriteByte(0);bs.WriteUInt16((ushort)VertC);
            bs.WriteByte(unk);
            bs.WriteUInt32(unkBits);
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
        if (rwyVersionMajor >= 6)
            return 0x14;
        else if (rwyVersionMajor >= 4)
            return 0x10;
        else
            return 0x14;
    }
}

