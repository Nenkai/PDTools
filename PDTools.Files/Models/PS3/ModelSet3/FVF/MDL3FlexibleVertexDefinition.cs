using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Syroot.BinaryData;

namespace PDTools.Files.Models.PS3.ModelSet3.FVF;

public class MDL3FlexibleVertexDefinition
{
    public int Unk0x04 { get; set; }
    public byte VertexSize { get; set; }
    public Dictionary<string, MDL3FVFElementDefinition> Elements = [];
    public MDL3FVFFieldArrayDefinition ArrayDefinition { get; set; }

    public string Name { get; set; }
    public static MDL3FlexibleVertexDefinition FromStream(BinaryStream bs, long baseMdlPos, uint mdl3Version)
    {
        MDL3FlexibleVertexDefinition def = new MDL3FlexibleVertexDefinition();
        uint nameOffset = bs.ReadUInt32();
        def.Unk0x04 = bs.ReadInt32();
        uint fieldDefsOffset = bs.ReadUInt32();
        bs.Position += 8;
        uint unkOffset_0x14 = bs.ReadUInt32();
        byte fieldCount = bs.Read1Byte();
        def.VertexSize = bs.Read1Byte();
        bs.Position += 0x5A;
        uint unkOffset_0x74 = bs.ReadUInt32();

        bs.Position = baseMdlPos + nameOffset;
        def.Name = bs.ReadString(StringCoding.ZeroTerminated);

        for (int i = 0; i < fieldCount; i++)
        {
            bs.Position = baseMdlPos + fieldDefsOffset + i * 0x08;
            var element = MDL3FVFElementDefinition.FromStream(bs, baseMdlPos, mdl3Version);
            def.Elements.Add(element.Name, element);
        }

        if (unkOffset_0x74 != 0)
        {
            bs.Position = baseMdlPos + unkOffset_0x74;
            def.ArrayDefinition = MDL3FVFFieldArrayDefinition.FromStream(bs, baseMdlPos, mdl3Version);
        }
        return def;
    }

    public static int GetSize()
    {
        return 0x78;
    }

    public override string ToString()
    {
        return $"{Name} - Vertex Size: 0x{VertexSize:X2} ({Elements.Count} fields)";
    }
}
