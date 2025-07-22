using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Syroot.BinaryData;

namespace PDTools.Files.Models.PS3.ModelSet3.ShapeStream;

public class MDL3ShapeStreamingInfoShapeEntry
{
    public uint OffsetInChunk { get; set; }
    public ushort ShapeIndex { get; set; }
    public ushort Unk { get; set; }

    public static MDL3ShapeStreamingInfoShapeEntry FromStream(BinaryStream bs, long baseMdlPos, uint mdl3Version)
    {
        MDL3ShapeStreamingInfoShapeEntry entry = new MDL3ShapeStreamingInfoShapeEntry();
        entry.OffsetInChunk = bs.ReadUInt32();
        entry.ShapeIndex = bs.ReadUInt16();
        entry.Unk = bs.ReadUInt16();

        return entry;
    }
}
