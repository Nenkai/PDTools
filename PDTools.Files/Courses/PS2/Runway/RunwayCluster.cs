using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

using Syroot.BinaryData;

namespace PDTools.Files.Courses.PS2.Runway;

public class RunwayCluster
{
    public ushort CheckpointLookupIndexStart { get; set; }
    public ushort CheckpointLookupLength { get; set; }
    public short[] TriIndices { get; set; }

    public static RunwayCluster FromStream(BinaryStream bs)
    {
        RunwayCluster cluster = new RunwayCluster();
        ushort triIndexCount = bs.ReadUInt16();
        cluster.CheckpointLookupLength = bs.ReadUInt16();
        cluster.CheckpointLookupIndexStart = bs.ReadUInt16();
        bs.ReadUInt16();
        int triIndicesOffset = bs.ReadInt32();
        bs.ReadInt32();

        bs.Position = triIndicesOffset;
        cluster.TriIndices = bs.ReadInt16s((int)triIndexCount);
        return cluster;
    }

    public static int GetSize()
    {
        return 0x10;
    }
}
