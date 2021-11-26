using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Syroot.BinaryData;

namespace PDTools.Files.Models
{
    public class MDL3ShapeStreamingInfo
    {
        public uint DataOffset { get; set; }
        public uint DataSize { get; set; }
        public Dictionary<short, MDL3ShapeStreamingInfoMeshEntry> Entries { get; set; } = new();

        public static MDL3ShapeStreamingInfo FromStream(BinaryStream bs, long baseMdlPos, uint mdl3Version)
        {
            MDL3ShapeStreamingInfo info = new MDL3ShapeStreamingInfo();
            uint flag = bs.ReadUInt32();
            info.DataOffset = bs.ReadUInt32();
            info.DataSize = bs.ReadUInt32();

            ushort meshEntriesCount = bs.ReadUInt16();
            uint meshEntriesOffset = bs.ReadUInt32();

            for (int i = 0; i < meshEntriesCount; i++)
            {
                bs.Position = baseMdlPos + meshEntriesOffset + (i * 0x08);
                var meshEntry = MDL3ShapeStreamingInfoMeshEntry.FromStream(bs, baseMdlPos, mdl3Version);
                info.Entries.Add(meshEntry.MeshIndex, meshEntry);
            }

            return info;
        }
    }
}
