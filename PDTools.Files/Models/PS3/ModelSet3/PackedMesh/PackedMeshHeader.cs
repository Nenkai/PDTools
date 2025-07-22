using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Syroot.BinaryData;

namespace PDTools.Files.Models.PS3.ModelSet3.PackedMesh;

public class PackedMeshHeader
{
    public List<PMSHMesh> Meshes { get; set; } = [];
    public List<PMSHMeshUnk0x1C> Unk0X1Cs { get; set; } = [];
    public List<PMSHElementBitLayoutArray> BitLayoutDefinitionArray { get; set; } = [];
    public List<PMSHFlexVertexDefinition> StructDeclarations { get; set; } = [];

    public static PackedMeshHeader FromStream(BinaryStream bs, long mdlBasePos, ushort mdl3VersionMajor)
    {
        PackedMeshHeader packedMesh = new();

        if (bs.ReadInt32() != 0x504d5348)
            throw new Exception("Expected PMSH magic.");

        int unk = bs.ReadInt32();
        int relocSize = bs.ReadInt32();
        bs.ReadInt32(); // Reloc ptr
        short meshCount = bs.ReadInt16();
        short unkCount0x1C = bs.ReadInt16();
        short elementBitLayoutDefinitionCount = bs.ReadInt16();
        short structDeclarationCount = bs.ReadInt16();

        int formatsOffset = bs.ReadInt32();
        int unkOffset0x1C = bs.ReadInt32();
        int elementBitLayoutDefinitionsOffset = bs.ReadInt32();
        int structDeclarationsOffset = bs.ReadInt32();
        int unkOffset0x28 = bs.ReadInt32();
        bs.ReadInt32();
        int unkOffset0x30 = bs.ReadInt32();

        for (var i = 0; i < meshCount; i++)
        {
            bs.Position = mdlBasePos + formatsOffset + i * PMSHMesh.GetSize();
            var format = PMSHMesh.FromStream(bs, mdlBasePos, mdl3VersionMajor);
            packedMesh.Meshes.Add(format);
        }

        for (var i = 0; i < unkCount0x1C; i++)
        {
            bs.Position = mdlBasePos + unkOffset0x1C + i * PMSHMeshUnk0x1C.GetSize();
            var mesh = PMSHMeshUnk0x1C.FromStream(bs, mdlBasePos, mdl3VersionMajor);
            packedMesh.Unk0X1Cs.Add(mesh);
        }


        for (var i = 0; i < elementBitLayoutDefinitionCount; i++)
        {
            bs.Position = mdlBasePos + elementBitLayoutDefinitionsOffset + i * PMSHElementBitLayoutArray.GetSize();
            var arr = PMSHElementBitLayoutArray.FromStream(bs, mdlBasePos, mdl3VersionMajor);
            packedMesh.BitLayoutDefinitionArray.Add(arr);
        }

        for (var i = 0; i < structDeclarationCount; i++)
        {
            bs.Position = mdlBasePos + structDeclarationsOffset + i * PMSHFlexVertexDefinition.GetSize();
            var decl = PMSHFlexVertexDefinition.FromStream(bs, mdlBasePos, mdl3VersionMajor);
            packedMesh.StructDeclarations.Add(decl);
        }

        return packedMesh;
    }
}
