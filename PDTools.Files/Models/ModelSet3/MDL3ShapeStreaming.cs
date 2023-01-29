using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Syroot.BinaryData;

namespace PDTools.Files.Models.ModelSet3
{
    public class MDL3ShapeStreamingMap
    {
        public List<MDL3ShapeStreamingInfo> Infos { get; set; } = new();

        public static MDL3ShapeStreamingMap FromStream(BinaryStream bs, long baseMdlPos, uint mdl3Version)
        {
            MDL3ShapeStreamingMap map = new MDL3ShapeStreamingMap();

            uint shapeStreamInfoCount = bs.ReadUInt32();
            uint shapeStreamInfoOffset = bs.ReadUInt32();
            bs.ReadUInt32(); // Unk
            uint indicesMapOffset = bs.ReadUInt32();
            bs.Position += 0x0C;
            short indicesMapCount = bs.ReadInt16();
            bs.ReadInt16();

            for (int i = 0; i < shapeStreamInfoCount; i++)
            {
                bs.Position = baseMdlPos + shapeStreamInfoOffset + i * 0x20;
                MDL3ShapeStreamingInfo info = MDL3ShapeStreamingInfo.FromStream(bs, baseMdlPos, mdl3Version);
                map.Infos.Add(info);
            }

            return map;
        }
    }
}
