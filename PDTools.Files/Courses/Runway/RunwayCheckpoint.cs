using System.Numerics;
using Syroot.BinaryData;

namespace PDTools.Files.Courses.Runway;

public class RunwayCheckpoint
{
    public Vector3 Left { get; set; }
    public Vector3 Middle { get; set; }
    public Vector3 Right { get; set; }
    public float TrackV { get; set; }

    public static RunwayCheckpoint FromStream(BinaryStream bs, ushort versionMajor, ushort versionMinor)
    {
        long basePos = bs.Position;

        RunwayCheckpoint checkpoint = new();

        checkpoint.Left = new Vector3(bs.ReadSingle(), bs.ReadSingle(), bs.ReadSingle());
        checkpoint.Middle = new Vector3(bs.ReadSingle(), bs.ReadSingle(), bs.ReadSingle());

        if (versionMajor >= 4)
        {
            checkpoint.TrackV = bs.ReadSingle();
            bs.Position = basePos + 0x28;
        }
        checkpoint.Right = new Vector3(bs.ReadSingle(), bs.ReadSingle(), bs.ReadSingle());

        return checkpoint;
    }

    public void ToStream(BinaryStream bs, ushort versionMajor, ushort versionMinor)
    {
        bs.WriteSingle(Left.X);
        bs.WriteSingle(Left.Y);
        bs.WriteSingle(Left.Z);

        bs.WriteSingle(Middle.X);
        bs.WriteSingle(Middle.Y);
        bs.WriteSingle(Middle.Z);

        if (versionMajor >= 4)
        {
            bs.WriteSingle(TrackV);

            bs.WriteSingle(Middle.X);
            bs.WriteSingle(Middle.Y);
            bs.WriteSingle(Middle.Z);
        }

        bs.WriteSingle(Right.X);
        bs.WriteSingle(Right.Y);
        bs.WriteSingle(Right.Z);

        bs.WriteSingle(TrackV);
    }

    public static int GetSize(ushort versionMajor, ushort versionMinor)
    {
        if (versionMajor >= 2 && versionMajor < 3)
            return 0x28;
        else
            return 0x38;
    }
}
