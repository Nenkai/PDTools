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
    public ushort CheckpointLookupLength     { get; set; }
    public short[] TriIndices                { get; set; }

    public static RunwayCluster FromStream(BinaryStream bs)
    {
        RunwayCluster cluster = new RunwayCluster();
        ushort triIndexCount          = bs.ReadUInt16();
        cluster.CheckpointLookupLength     = bs.ReadUInt16();
        cluster.CheckpointLookupIndexStart = bs.ReadUInt16();
        bs.ReadUInt16();                          // padding
        int triIndicesOffset          = bs.ReadInt32();
        bs.ReadInt32();                           // padding

        long savedPos = bs.Position;
        bs.Position = triIndicesOffset;
        cluster.TriIndices = bs.ReadInt16s((int)triIndexCount);
        bs.Position = savedPos;
        return cluster;
    }

    /// <summary>
    /// Writes the 0x10-byte cluster header.
    /// <paramref name="absoluteTriDataOffset"/> is the file-absolute byte offset of the
    /// first tri-index short for this cluster (written into the header's triIndicesOffset field).
    /// The actual tri-index data is written separately by <see cref="WriteTriData"/>.
    /// </summary>
    public void WriteHeader(BinaryStream bs, int absoluteTriDataOffset)
    {
        bs.WriteUInt16((ushort)TriIndices.Length);
        bs.WriteUInt16(CheckpointLookupLength);
        bs.WriteUInt16(CheckpointLookupIndexStart);
        bs.WriteUInt16(0);                    // padding
        bs.WriteInt32(absoluteTriDataOffset);
        bs.WriteInt32(0);                     // padding
    }

    /// <summary>Writes the tri-index array (sequentially; call after all cluster headers).</summary>
    public void WriteTriData(BinaryStream bs)
    {
        foreach (short idx in TriIndices)
            bs.WriteInt16(idx);
    }

    public static int GetSize() => 0x10;
}
