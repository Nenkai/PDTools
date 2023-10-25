using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Syroot.BinaryData;

namespace PDTools.Files.Models.PS3.ModelSet3.PackedMesh
{
    public class PackedMeshHeader
    {
        public List<PackedMeshEntry> Entries { get; set; } = new();
        public List<PackedMeshElementBitLayoutArray> BitLayoutDefinitionArray { get; set; } = new();
        public List<PackedMeshFlexVertexDefinition> StructDeclarations { get; set; } = new();

        public static PackedMeshHeader FromStream(BinaryStream bs, long mdlBasePos, ushort mdl3VersionMajor)
        {
            PackedMeshHeader packedMesh = new();

            if (bs.ReadInt32() != 0x504d5348)
                throw new Exception("Expected PMSH magic.");

            int unk = bs.ReadInt32();
            int relocSize = bs.ReadInt32();
            bs.ReadInt32(); // Reloc ptr
            short formatCount = bs.ReadInt16();
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

            for (var i = 0; i < formatCount; i++)
            {
                bs.Position = mdlBasePos + formatsOffset + i * PackedMeshEntry.GetSize();
                var format = PackedMeshEntry.FromStream(bs, mdlBasePos, mdl3VersionMajor);
                packedMesh.Entries.Add(format);
            }

            for (var i = 0; i < elementBitLayoutDefinitionCount; i++)
            {
                bs.Position = mdlBasePos + elementBitLayoutDefinitionsOffset + i * PackedMeshElementBitLayoutArray.GetSize();
                var arr = PackedMeshElementBitLayoutArray.FromStream(bs, mdlBasePos, mdl3VersionMajor);
                packedMesh.BitLayoutDefinitionArray.Add(arr);
            }

            for (var i = 0; i < structDeclarationCount; i++)
            {
                bs.Position = mdlBasePos + structDeclarationsOffset + i * PackedMeshFlexVertexDefinition.GetSize();
                var decl = PackedMeshFlexVertexDefinition.FromStream(bs, mdlBasePos, mdl3VersionMajor);
                packedMesh.StructDeclarations.Add(decl);
            }

            return packedMesh;
        }
    }
}
