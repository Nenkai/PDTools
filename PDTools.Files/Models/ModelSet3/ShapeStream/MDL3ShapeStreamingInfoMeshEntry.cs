using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Syroot.BinaryData;

namespace PDTools.Files.Models.ModelSet3.ShapeStream
{
    public class MDL3ShapeStreamingInfoMeshEntry
    {
        public uint OffsetInChunk { get; set; }
        public ushort MeshIndex { get; set; }
        public ushort Unk { get; set; }

        public static MDL3ShapeStreamingInfoMeshEntry FromStream(BinaryStream bs, long baseMdlPos, uint mdl3Version)
        {
            MDL3ShapeStreamingInfoMeshEntry entry = new MDL3ShapeStreamingInfoMeshEntry();
            entry.OffsetInChunk = bs.ReadUInt32();
            entry.MeshIndex = bs.ReadUInt16();
            entry.Unk = bs.ReadUInt16();

            return entry;
        }
    }
}
