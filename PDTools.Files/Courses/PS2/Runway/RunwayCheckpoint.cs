using System.Numerics;
using Syroot.BinaryData;

namespace PDTools.Files.Courses.PS2.Runway;

public class RunwayCheckpoint
{
    public Vector3 Left { get; set; }
    public Vector3 Middle { get; set; }
    public Vector3 Right { get; set; }
    public float TrackV { get; set; }

    public static RunwayCheckpoint FromStream(BinaryStream bs)
    {
        RunwayCheckpoint checkpoint = new();

        checkpoint.Left = new Vector3(bs.ReadSingle(), bs.ReadSingle(), bs.ReadSingle());
        checkpoint.Middle = new Vector3(bs.ReadSingle(), bs.ReadSingle(), bs.ReadSingle());
        checkpoint.Right = new Vector3(bs.ReadSingle(), bs.ReadSingle(), bs.ReadSingle());
        checkpoint.TrackV = bs.ReadSingle();

        return checkpoint;
    }

    public void ToStream(BinaryStream bs)
    {
        bs.WriteSingle(Left.X);
        bs.WriteSingle(Left.Y);
        bs.WriteSingle(Left.Z);

        bs.WriteSingle(Middle.X);
        bs.WriteSingle(Middle.Y);
        bs.WriteSingle(Middle.Z);

        bs.WriteSingle(Right.X);
        bs.WriteSingle(Right.Y);
        bs.WriteSingle(Right.Z);

        bs.WriteSingle(TrackV);
    }

    public static int GetSize()
    {
        return 0x28;
    }
}
