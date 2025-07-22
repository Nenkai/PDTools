using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Syroot.BinaryData;

namespace PDTools.Files.Models.PS3.ModelSet3.ShapeStream;

public class MDL3ShapeStreamingChunkInfo
{
    public uint DeflatedChunkOffset { get; set; }
    public uint DeflatedChunkSize { get; set; }
    public Dictionary<ushort, MDL3ShapeStreamingInfoShapeEntry> Entries { get; set; } = [];

    public static MDL3ShapeStreamingChunkInfo FromStream(BinaryStream bs, long baseMdlPos, uint mdl3Version)
    {
        MDL3ShapeStreamingChunkInfo info = new MDL3ShapeStreamingChunkInfo();
        uint flag = bs.ReadUInt32();
        info.DeflatedChunkOffset = bs.ReadUInt32();
        info.DeflatedChunkSize = bs.ReadUInt32();

        uint meshEntriesOffset = bs.ReadUInt32();
        ushort meshEntriesCount = bs.ReadUInt16();

        for (int i = 0; i < meshEntriesCount; i++)
        {
            bs.Position = baseMdlPos + meshEntriesOffset + i * 0x08;
            var meshEntry = MDL3ShapeStreamingInfoShapeEntry.FromStream(bs, baseMdlPos, mdl3Version);
            info.Entries.Add(meshEntry.ShapeIndex, meshEntry);
        }

        return info;
    }
}
