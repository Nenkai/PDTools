using Syroot.BinaryData;

namespace PDTools.Files.Courses.PS2.Runway;

/// <summary>
/// One spawn / starting-grid position inside a RNW4 file.
/// Six of these are stored contiguously at file offset 0xC0 (each 16 bytes = 4 floats).
/// </summary>
public class RunwayStartingPosition
{
    public float X        { get; set; }
    public float Y        { get; set; }
    public float Z        { get; set; }

    /// <summary>
    /// Heading angle of the spawned car, in radians (positive = counter-clockwise from +Z).
    /// </summary>
    public float Rotation { get; set; }

    public static RunwayStartingPosition FromStream(BinaryStream bs)
    {
        return new RunwayStartingPosition
        {
            X        = bs.ReadSingle(),
            Y        = bs.ReadSingle(),
            Z        = bs.ReadSingle(),
            Rotation = bs.ReadSingle(),
        };
    }

    public void ToStream(BinaryStream bs)
    {
        bs.WriteSingle(X);
        bs.WriteSingle(Y);
        bs.WriteSingle(Z);
        bs.WriteSingle(Rotation);
    }

    /// <summary>Size of one record in bytes (always 0x10).</summary>
    public static int GetSize() => 0x10;
}
