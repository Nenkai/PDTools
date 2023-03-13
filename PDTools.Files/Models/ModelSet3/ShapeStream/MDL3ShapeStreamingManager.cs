using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Syroot.BinaryData;

namespace PDTools.Files.Models.ModelSet3.ShapeStream
{
    public class MDL3ShapeStreamingManager
    {
        public List<MDL3ShapeStreamingChunkInfo> ChunkInfos { get; set; } = new();

        public uint BufferSize { get; set; }
        public static MDL3ShapeStreamingManager FromStream(BinaryStream bs, long baseMdlPos, uint mdl3Version)
        {
            MDL3ShapeStreamingManager man = new MDL3ShapeStreamingManager();

            uint shapeStreamChunkCount = bs.ReadUInt32();
            uint shapeStreamChunkInfoOffset = bs.ReadUInt32();
            man.BufferSize = bs.ReadUInt32(); // Unk
            uint shapeStreamGroupsOffset = bs.ReadUInt32();
            bs.Position += 0x0C;
            short shapeStreamGroupCount = bs.ReadInt16();
            bs.ReadInt16();

            for (int i = 0; i < shapeStreamChunkCount; i++)
            {
                bs.Position = baseMdlPos + shapeStreamChunkInfoOffset + i * 0x20;
                MDL3ShapeStreamingChunkInfo info = MDL3ShapeStreamingChunkInfo.FromStream(bs, baseMdlPos, mdl3Version);
                man.ChunkInfos.Add(info);
            }

            return man;
        }
    }
}
