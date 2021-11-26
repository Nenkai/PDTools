using System;
using System.Collections.Generic;
using System.IO;
using Syroot.BinaryData.Core;
using Syroot.BinaryData;
using Syroot.BinaryData.Memory;

using System.Linq;
using System.Numerics;
using System.Buffers.Binary;
using PDTools.Files.Textures;
using PDTools.Files.Courses.CourseData;

namespace PDTools.Files.Models
{
    public class MDL3
    {
        const string MAGIC = "MDL3";
        const string MAGIC_LE = "3LDM";

        public ushort VersionMajor { get; set; }
        public long BasePosition { get; set; }

        public Dictionary<ushort, MDL3ModelRenderParams> ModelRenderParams { get; set; } = new();
        public Dictionary<ushort, MDL3FVFDefinition> FVFs { get; set; } = new();
        //public Dictionary<ushort, MDL3MeshInfo> MeshInfos { get; set; } = new();
        public Dictionary<ushort, MDL3Mesh> Meshes { get; set; } = new();
        public MDL3MaterialMap Materials { get; set; } = new();
        public TextureSet3 TextureSet { get; set; }
        public MDL3ShapeStreamingMap StreamingInfo { get; set; }

        public BinaryStream Stream { get; set; }
        public CourseDataFile ParentCourseData { get; set; }

        public static MDL3 FromStream(BinaryStream bs, int txsPos = 0)
        {
            long basePos = bs.Position;

            string magic = bs.ReadString(4);
            if (magic != MAGIC && magic != MAGIC_LE)
                throw new InvalidDataException("Not a valid MDL3 file.");

            /* HEADER - 0xE4 */
            MDL3 mdl3 = new();
            mdl3.BasePosition = basePos;
            mdl3.Stream = bs;

            bs.ReadInt32(); // File Size
            bs.ReadInt32(); // Reloc Ptr
            mdl3.VersionMajor = bs.ReadUInt16();
            bs.ReadUInt16(); // Runtime Flags
            ushort modelRenderParamsCount = bs.ReadUInt16();
            ushort count_0x34 = bs.ReadUInt16();
            ushort meshesCount = bs.ReadUInt16();
            ushort count_0x3C = bs.ReadUInt16();
            ushort flexibleVerticesCount = bs.ReadUInt16();
            ushort bonesCount = bs.ReadUInt16();
            ushort sizeFor0x68 = bs.ReadUInt16();
            ushort count_0x58 = bs.ReadUInt16();
            bs.ReadUInt16(); // Unk
            ushort count_0x5C = bs.ReadUInt16();
            bs.ReadUInt16(); // Unk
            ushort count_0x78 = bs.ReadUInt16();
            ushort count_0xA4 = bs.ReadUInt16();
            ushort count_0x54 = bs.ReadUInt16();
            ushort count_0x88 = bs.ReadUInt16();
            bs.ReadUInt16(); // Unk

            // Offsets
            uint modelRenderParamsOffset = bs.ReadUInt32();
            uint offset_0x34 = bs.ReadUInt32();
            uint meshesOffset = bs.ReadUInt32();
            uint offset_0x3C = bs.ReadUInt32();
            uint flexibleVerticesOffset = bs.ReadUInt32();
            uint materialsOffset = bs.ReadUInt32();
            uint textureSetOffset = bs.ReadUInt32();
            uint shadersOffset = bs.ReadUInt32();
            uint bonesOffset = bs.ReadUInt32();
            uint unkKeyMap0x54 = bs.ReadUInt32();
            uint offset_0x58 = bs.ReadUInt32();
            uint offset_0x5C = bs.ReadUInt32();
            bs.ReadUInt32(); // Unk
            uint vmCommandsOffset = bs.ReadUInt32();
            uint offset_0x68 = bs.ReadUInt32();
            bs.ReadUInt32(); // Unk
            bs.ReadUInt32(); // Unk
            bs.ReadUInt32(); // Unk
            uint offset_0x78 = bs.ReadUInt32();
            bs.Position += 0xC; // Unks
            uint offset_0x8c = bs.ReadUInt32();
            ushort count_0x8c = bs.ReadUInt16();
            ushort count_0x90 = bs.ReadUInt16();
            uint offset_0x90 = bs.ReadUInt32();
            bs.ReadUInt32(); // Unk
            ushort count_0x9c = bs.ReadUInt16();
            ushort count_0xA0 = bs.ReadUInt16();
            uint offset_0x9c = bs.ReadUInt32();
            uint offset_0xA0 = bs.ReadUInt32();
            uint offset_0xA4 = bs.ReadUInt32();
            bs.ReadUInt32(); // Unk
            uint shapeStreamMapOffset = bs.ReadUInt32();
            uint offset_0xB0 = bs.ReadUInt32();
            bs.ReadUInt32(); // Unk
            uint offset_0xB8 = bs.ReadUInt32();
            uint vm_related_offset_0x8C = bs.ReadUInt32();
            uint offset_0xC0 = bs.ReadUInt32();
            ushort count_0xC0 = bs.ReadUInt16();
            bs.ReadUInt16(); // Unk
            bs.ReadInt16(); // Unk
            ushort count_0xCC = bs.ReadUInt16();
            uint offset_0xCC = bs.ReadUInt32();
            uint offset_0xD0 = bs.ReadUInt32();
            uint offset_0xD4 = bs.ReadUInt32();
            uint carBodyStreamOffset = bs.ReadUInt32();

            // Starting to read stuff
            mdl3.ReadModelRenderParams(bs, basePos, modelRenderParamsOffset, modelRenderParamsCount);
            mdl3.ReadMaterials(bs, basePos, materialsOffset, 1);
            mdl3.ReadFlexibleVertices(bs, basePos, flexibleVerticesOffset, flexibleVerticesCount);
            mdl3.ReadMeshes(bs, basePos, meshesOffset, meshesCount);
            mdl3.ReadTextureSet(bs, basePos, textureSetOffset, 1);
            //mdl3.ReadStreamInfo(bs, basePos, shapeStreamMapOffset, 1);

            return mdl3;
        }

        public Vector3[] GetVerticesOfMesh(ushort meshIndex)
        {
            var mesh = Meshes[meshIndex];

            MDL3FVFDefinition fvfDef = FVFs[mesh.FVFIndex];
            if (!fvfDef.FieldDefinitions.TryGetValue("position", out var field))
                throw new InvalidOperationException("FVF does not contain 'position' field.");

            if (field.FieldType != CELL_GCM_VERTEX_TYPE.CELL_GCM_VERTEX_F)
                throw new NotSupportedException("Expected vector 3 float");

            var arr = new Vector3[mesh.VertexCount];

            if (mesh.VerticesOffset != 0)
            {
                Span<byte> buffer = new byte[fvfDef.VertexSize];
                for (int i = 0; i < mesh.VertexCount; i++)
                {
                    GetVerticesData(mesh, fvfDef, i, buffer);
                    SpanReader sr = new SpanReader(buffer, Endian.Big);

                    sr.Position += field.StartOffset;
                    float x = sr.ReadSingle();
                    float y = sr.ReadSingle();
                    float z = sr.ReadSingle();
                    arr[i] = new Vector3(x, y, z);
                }
            }
            else
            {
                // TODO: Try shapestream
                // TODO: Find the mechanism that determines whether we are using a stream or not
            }

            return arr;
        }

        public List<Tri> GetTrisOfMesh(ushort meshIndex)
        {
            MDL3Mesh mesh = Meshes[meshIndex];
            var list = new List<Tri>();

            if (mesh.TriOffset != 0)
            {
                Stream.Position = BasePosition + mesh.TriOffset;
                for (int i = 0; i < mesh.TriCount; i++)
                {
                    ushort a = Stream.ReadUInt16();
                    ushort b = Stream.ReadUInt16();
                    ushort c = Stream.ReadUInt16();
                    if (a < mesh.VertexCount && b < mesh.VertexCount && c < mesh.VertexCount)
                    {
                        list.Add(new(a, b, c));
                    }
                    else
                    {
                        return null; // Tristrip - FIX ME
                    }
                }
            }
            else
            {
                // TODO: Try shapestream
                // TODO: Find the mechanism that determines whether we are using a stream or not
            }

            return list;
        }

        public Vector2[] GetUVsOfMesh(ushort meshIndex)
        {
            var mesh = Meshes[meshIndex];

            MDL3FVFDefinition fvfDef = FVFs[mesh.FVFIndex];
            if (!fvfDef.FieldDefinitions.TryGetValue("map12", out var field))
                return Array.Empty<Vector2>();

           var arr = new Vector2[mesh.VertexCount];

            if (mesh.VerticesOffset != 0)
            {
                Span<byte> buffer = new byte[fvfDef.VertexSize];
                for (int i = 0; i < mesh.VertexCount; i++)
                {
                    GetVerticesData(mesh, fvfDef, i, buffer);
                    SpanReader sr = new SpanReader(buffer, Endian.Big);

                    sr.Position += field.StartOffset;
                    float u = 0, v = 0;
                    if (field.FieldType == CELL_GCM_VERTEX_TYPE.CELL_GCM_VERTEX_S1)
                    {
                        u = ((float)sr.ReadUInt16() * (1f / (float)short.MaxValue));
                        v = ((float)sr.ReadUInt16() * (1f / (float)short.MaxValue));
                    }
                    else if (field.FieldType == CELL_GCM_VERTEX_TYPE.CELL_GCM_VERTEX_UB)
                    {
                        u = ((float)sr.ReadByte() * (1f / (float)sbyte.MaxValue));
                        v = ((float)sr.ReadByte() * (1f / (float)sbyte.MaxValue));
                    }
                    else
                    {
                        ;
                    }

                    arr[i] = new Vector2(u, v);
                }
            }
            else
            {
                // TODO: Try shapestream
                // TODO: Find the mechanism that determines whether we are using a stream or not
            }

            return arr;
        }

        public void GetVerticesData(MDL3Mesh meshInfo, MDL3FVFDefinition fvfDef, int vertIndex, Span<byte> buffer)
        {
            Stream.Position = BasePosition + meshInfo.VerticesOffset + (vertIndex * fvfDef.VertexSize);
            Stream.Read(buffer);
        }

        private void ReadModelRenderParams(BinaryStream bs, long baseMdlPos, uint offset, uint count)
        {
            for (ushort i = 0; i < count; i++)
            {
                bs.Position = baseMdlPos + offset + (i * 0x30);
                ModelRenderParams.Add(i, MDL3ModelRenderParams.FromStream(bs, baseMdlPos, VersionMajor));
            }
        }

        private void ReadFlexibleVertices(BinaryStream bs, long baseMdlPos, uint offset, uint count)
        {
            for (ushort i = 0; i < count; i++)
            {
                bs.Position = baseMdlPos + offset + (i * 0x78);
                FVFs.Add(i, MDL3FVFDefinition.FromStream(bs, baseMdlPos, VersionMajor));
            }
        }

        private void ReadMeshes(BinaryStream bs, long baseMdlPos, uint offset, uint count)
        {
            for (ushort i = 0; i < count; i++)
            {
                bs.Position = baseMdlPos + offset + (i * 0x30);
                Meshes.Add(i, MDL3Mesh.FromStream(bs, baseMdlPos, VersionMajor));
            }
        }

        private void ReadStreamInfo(BinaryStream bs, long baseMdlPos, uint offset, uint count)
        {
            if (VersionMajor <= 1)
                return;

            if (offset == 0)
                return;

            bs.Position = baseMdlPos + offset;
            StreamingInfo = MDL3ShapeStreamingMap.FromStream(bs, baseMdlPos, VersionMajor);
        }

        private void ReadTextureSet(BinaryStream bs, long baseMdlPos, uint offset, uint count)
        {
            bs.Position = baseMdlPos + offset;
            TextureSet = new TextureSet3();
            TextureSet.FromStream(bs);
        }

        private void ReadMaterials(BinaryStream bs, long baseMdlPos, uint offset, uint count)
        {
            bs.Position = baseMdlPos + offset;
            Materials = MDL3MaterialMap.FromStream(bs, baseMdlPos, VersionMajor);
        }
    }
}

