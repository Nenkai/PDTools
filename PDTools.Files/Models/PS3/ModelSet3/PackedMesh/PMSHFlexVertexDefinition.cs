﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Syroot.BinaryData;

namespace PDTools.Files.Models.PS3.ModelSet3.PackedMesh;

public class PMSHFlexVertexDefinition
{
    public byte PackedElementCount { get; set; }
    public byte RawElementCount { get; set; }
    public byte PackedStride { get; set; }
    public byte NonPackedStride { get; set; }
    public int Unk { get; set; }

    public Dictionary<string, PMSHFlexVertexElementDefinition> PackedElements { get; set; } = [];

    // May have elements with the same name.
    public List<PMSHFlexVertexElementDefinition> Elements { get; set; } = [];

    public static PMSHFlexVertexDefinition FromStream(BinaryStream bs, long mdlBasePos, ushort mdl3VersionMajor)
    {
        PMSHFlexVertexDefinition declaration = new();

        declaration.PackedElementCount = bs.Read1Byte();
        declaration.RawElementCount = bs.Read1Byte();
        declaration.PackedStride = bs.Read1Byte();
        declaration.NonPackedStride = bs.Read1Byte();
        declaration.Unk = bs.ReadInt32();

        int elementDefsOffset = bs.ReadInt32();
        int elementNamesOffst = bs.ReadInt32();

        var packedElemList = new List<PMSHFlexVertexElementDefinition>();
        var elemList = new List<PMSHFlexVertexElementDefinition>();

        for (var i = 0; i < declaration.PackedElementCount; i++)
        {
            bs.Position = mdlBasePos + elementDefsOffset + i * PMSHFlexVertexElementDefinition.GetSize();
            var elem = PMSHFlexVertexElementDefinition.FromStream(bs, mdlBasePos, mdl3VersionMajor);
            elem.IsPacked = true;
            packedElemList.Add(elem);
        }

        for (var i = 0; i < declaration.RawElementCount; i++)
        {
            bs.Position = mdlBasePos + elementDefsOffset + declaration.PackedElementCount * PMSHFlexVertexElementDefinition.GetSize() + i * PMSHFlexVertexElementDefinition.GetSize();
            var elem = PMSHFlexVertexElementDefinition.FromStream(bs, mdlBasePos, mdl3VersionMajor);
            elemList.Add(elem);
        }

        for (var i = 0; i < declaration.PackedElementCount; i++)
        {
            bs.Position = mdlBasePos + elementNamesOffst + i * 0x04;
            int nameOffset = bs.ReadInt32();
            bs.Position = mdlBasePos + nameOffset;
            string name = bs.ReadString(StringCoding.ZeroTerminated);
            packedElemList[i].Name = name;
            declaration.PackedElements.Add(name, packedElemList[i]);
        }

        for (var i = 0; i < declaration.RawElementCount; i++)
        {
            bs.Position = mdlBasePos + elementNamesOffst + declaration.PackedElementCount * 0x04 + i * 0x04;
            int nameOffset = bs.ReadInt32();
            bs.Position = mdlBasePos + nameOffset;
            string name = bs.ReadString(StringCoding.ZeroTerminated);
            elemList[i].Name = name;
            declaration.Elements.Add(elemList[i]);
        }

        return declaration;
    }

    public PMSHFlexVertexElementDefinition GetElement(string name)
    {
        if (PackedElements.TryGetValue(name, out PMSHFlexVertexElementDefinition elem))
            return elem;

        // TODO: Elements can have multiple elements with the same name.
        // Fetch by index on elements with the same list?
        return Elements.FirstOrDefault(e => e.Name == name);
    }

    public static int GetSize()
    {
        return 0x10;
    }
}
