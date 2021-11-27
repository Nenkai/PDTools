using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Syroot.BinaryData;

namespace PDTools.Files.Models
{
    public class MDL3FVFDefinition
    {
        public byte VertexSize { get; set; }
        public Dictionary<string, MDL3FVFFieldDefinition> FieldDefinitions = new();
        public MDL3FVFFieldArrayDefinition ArrayDefinition { get; set; }

        public static MDL3FVFDefinition FromStream(BinaryStream bs, long baseMdlPos, uint mdl3Version)
        {
            MDL3FVFDefinition def = new MDL3FVFDefinition();
            uint strOffset = bs.ReadUInt32();
            bs.Position += 4; // Nothing
            uint fieldDefsOffset = bs.ReadUInt32();
            bs.Position += 8;
            uint unkOffset_0x14 = bs.ReadUInt32();
            byte fieldCount = bs.Read1Byte();
            def.VertexSize = bs.Read1Byte();
            bs.Position += 0x5A;
            uint unkOffset_0x74 = bs.ReadUInt32();

            for (int i = 0; i < fieldCount; i++)
            {
                bs.Position = baseMdlPos + fieldDefsOffset + (i * 0x08);
                var field = MDL3FVFFieldDefinition.FromStream(bs, baseMdlPos, mdl3Version);
                def.FieldDefinitions.Add(field.Name, field);
            }

            if (unkOffset_0x74 != 0)
            {
                bs.Position = baseMdlPos + unkOffset_0x74;
                def.ArrayDefinition = MDL3FVFFieldArrayDefinition.FromStream(bs, baseMdlPos, mdl3Version);
            }
            return def;
        }

        public override string ToString()
        {
            return $"Vertex Size: 0x{VertexSize:X2} ({FieldDefinitions.Count} fields)";
        }
    }
}
