using Syroot.BinaryData;

namespace PDTools.Files.Courses.PS2.Runway;

/// <summary>
/// One 8-byte entry in the GTRW gate/sector table.
/// Fields:
///   SectorId  – track zone: 0 = global origin, 1 = start/finish gate, 2 = main track
///   GateFlag  – record type: -1 = sentinel (b/e of list), 0 = timing gate, 1 = normal
///   TrackV    – V-coordinate (metres) of this record along the track
/// </summary>
public class GT3GateRecord
{
    public short SectorId  { get; set; }
    public short GateFlag  { get; set; }
    public float TrackV    { get; set; }

    /// <summary>True when this record marks a timing-sector boundary (T1/T2/T3).</summary>
    public bool IsTimingGate  => GateFlag == 0;

    /// <summary>True when this is a sentinel entry (start/finish markers).</summary>
    public bool IsSentinel    => GateFlag == -1;

    public static GT3GateRecord FromStream(BinaryStream bs)
        => new GT3GateRecord
        {
            SectorId = bs.ReadInt16(),
            GateFlag = bs.ReadInt16(),
            TrackV   = bs.ReadSingle(),
        };

    public void ToStream(BinaryStream bs)
    {
        bs.WriteInt16(SectorId);
        bs.WriteInt16(GateFlag);
        bs.WriteSingle(TrackV);
    }

    public static int GetSize() => 8;
}
