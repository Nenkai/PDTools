using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Syroot.BinaryData;

namespace PDTools.Files.Models
{
    public class MDL3FVFFieldDefinition
    {
        /// <summary>
        /// Name of the field within the flexible vertex.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Location of the field within the flexible vertex.
        /// </summary>
        public byte StartOffset { get; set; }

        /// <summary>
        /// Count of data elements within the flexible vertex.
        /// </summary>
        public byte ElementCount { get; set; }

        /// <summary>
        /// Data Type within the flexible vertex.
        /// </summary>
        public CELL_GCM_VERTEX_TYPE FieldType { get; set; }

        public byte Unk { get; set; }

        public static MDL3FVFFieldDefinition FromStream(BinaryStream bs, long baseMdlPos, uint mdl3Version)
        {
            MDL3FVFFieldDefinition def = new MDL3FVFFieldDefinition();
            uint nameOffset = bs.ReadUInt32();
            def.StartOffset = bs.Read1Byte();
            def.ElementCount = bs.Read1Byte();
            def.FieldType = (CELL_GCM_VERTEX_TYPE)bs.Read1Byte();
            def.Unk = bs.Read1Byte();

            bs.Position = baseMdlPos + nameOffset;
            def.Name = bs.ReadString(StringCoding.ZeroTerminated);
            return def;
        }

        public override string ToString()
        {
            return $"{Name} (Start: 0x{StartOffset:X2}, Type: {FieldType}, {ElementCount} elements, {Unk})";
        }
    }

    public enum CELL_GCM_VERTEX_TYPE
    {
        /// <summary>
        /// Normalized short
        /// </summary>
        CELL_GCM_VERTEX_S1 = 1,

        /// <summary>
        /// Float
        /// </summary>
        CELL_GCM_VERTEX_F = 2,

        /// <summary>
        /// Half Float
        /// </summary>
        CELL_GCM_VERTEX_SF = 3,

        /// <summary>
        /// Unsigned byte
        /// </summary>
        CELL_GCM_VERTEX_UB = 4,

        /// <summary>
        /// Short
        /// </summary>
        CELL_GCM_VERTEX_S32K = 5,

        /// <summary>
        /// Vector, 10 11 11 bits
        /// </summary>
        CELL_GCM_VERTEX_CMP = 6,

        CELL_GCM_VERTEX_UB256 = 7,
    }
}
