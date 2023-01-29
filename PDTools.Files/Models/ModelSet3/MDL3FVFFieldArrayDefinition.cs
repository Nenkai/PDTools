using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Syroot.BinaryData;

namespace PDTools.Files.Models.ModelSet3
{
    public class MDL3FVFFieldArrayDefinition
    {
        public uint DataOffset { get; set; }
        public uint ArrayLength { get; set; }
        public uint ArrayElementSize { get; set; }
        public byte VertexSize { get; set; }

        /// <summary>
        /// Not sure
        /// </summary>
        public byte VertexSizeDefault { get; set; }

        public static MDL3FVFFieldArrayDefinition FromStream(BinaryStream bs, long baseMdlPos, uint mdl3Version)
        {
            MDL3FVFFieldArrayDefinition def = new MDL3FVFFieldArrayDefinition();
            def.DataOffset = bs.ReadUInt32();
            def.ArrayLength = bs.ReadUInt32();
            def.ArrayElementSize = bs.ReadUInt32();
            def.VertexSize = bs.Read1Byte();
            def.VertexSizeDefault = bs.Read1Byte();
            // 2 bytes pad?

            return def;
        }
    }
}
