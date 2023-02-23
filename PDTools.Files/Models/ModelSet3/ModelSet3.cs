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
using PDTools.Files.Models.VM;
using PDTools.Utils;
using PDTools.Files.Models.ModelSet3.Materials;
using PDTools.Files.Models.ModelSet3.Meshes;
using PDTools.Files.Models.ModelSet3.FVF;
using PDTools.Files.Models.ModelSet3.ShapeStream;
using PDTools.Files.Models.Shaders;
using PDTools.Files.Models.Bones;
using PDTools.Files.Models.ModelSet3.Wing;
using PDTools.Files.Models.ModelSet3.PMSH;

namespace PDTools.Files.Models.ModelSet3
{
    public class ModelSet3
    {
        const string MAGIC = "MDL3";
        const string MAGIC_LE = "3LDM";

        public const int HeaderSize = 0xE4;

        public ushort Version { get; set; }
        public long BaseOffset { get; set; }

        public List<ModelSet3Model> Models { get; set; } = new();
        public List<MDL3ModelKey> ModelKeys { get; set; } = new();

        public List<MDL3Mesh> Meshes { get; set; } = new();
        public List<MDL3MeshKey> MeshKeys { get; set; } = new();
        public List<MDL3FlexibleVertexDefinition> FlexibleVertexFormats { get; set; } = new();
        public MDL3Materials Materials { get; set; } = new();
        public TextureSet3 TextureSet { get; set; }
        public ShadersHeader Shaders { get; set; }
        public List<MDL3Bone> Bones { get; set; } = new();
        public ushort _0x68Size { get; set; }
        public ushort VMStackSize { get; set; }
        public VMBytecode VirtualMachine { get; set; } = new VMBytecode();
        public Dictionary<short, VMHostMethodEntry> VMHostMethodEntries { get; set; } = new();
        public List<MDL3TextureKey> TextureKeys { get; set; } = new();
        public List<MDL3WingData> WingData { get; set; } = new();
        public List<MDL3WingKey> WingKeys { get; set; } = new();
        public List<MDL3ModelVMUnk> UnkVMData { get; set; } = new();
        public MDL3ModelVMUnk2 UnkVMData2 { get; set; }
        public MDL3ModelVMContext VMContext { get; set; }
        public List<MDL3PMSHKey> PMSHKeys { get; set; } = new();
        public MDL3PMSH PMSH { get; set; } = new();
        public MDL3ShapeStreamingMap StreamingInfo { get; set; }

        public CourseDataFile ParentCourseData { get; set; }
        public BinaryStream Stream { get; set; }

        public static ModelSet3 FromStream(BinaryStream bs, int txsPos = 0)
        {
            long basePos = bs.Position;

            string magic = bs.ReadString(4);
            if (magic == MAGIC)
                bs.ByteConverter = ByteConverter.Big;
            else if (magic == MAGIC_LE)
                bs.ByteConverter = ByteConverter.Little;
            else
                throw new InvalidDataException("Not a valid MDL3 file.");

            /* HEADER - 0xE4 */
            ModelSet3 modelSet = new();
            modelSet.BaseOffset = basePos;
            modelSet.Stream = bs;

            bs.ReadInt32(); // File Size
            bs.ReadInt32(); // Reloc Ptr
            modelSet.Version = bs.ReadUInt16();
            bs.ReadUInt16(); // Runtime Flags
            ushort modelCount = bs.ReadUInt16();
            ushort modelKeyCount = bs.ReadUInt16();
            ushort meshesCount = bs.ReadUInt16();
            ushort meshKeysCount = bs.ReadUInt16();
            ushort flexibleVerticesCount = bs.ReadUInt16();
            ushort bonesCount = bs.ReadUInt16();
            modelSet._0x68Size = bs.ReadUInt16();
            ushort registerValCount = bs.ReadUInt16();
            modelSet.VMStackSize = bs.ReadUInt16(); // Unk
            ushort count_0x5C = bs.ReadUInt16();
            bs.ReadUInt16(); // Unk
            ushort count_0x78 = bs.ReadUInt16();
            ushort count_0xA4 = bs.ReadUInt16();
            ushort count_0x54 = bs.ReadUInt16();
            ushort count_0x88 = bs.ReadUInt16();
            bs.ReadUInt16(); // Unk

            // Offsets
            uint modelsOffset = bs.ReadUInt32();
            uint modelKeysOffset = bs.ReadUInt32();
            uint meshesOffset = bs.ReadUInt32();
            uint meshKeysOffset = bs.ReadUInt32();
            uint flexibleVerticesOffset = bs.ReadUInt32();
            uint materialsOffset = bs.ReadUInt32();
            uint textureSetOffset = bs.ReadUInt32();
            uint shadersOffset = bs.ReadUInt32();
            uint bonesOffset = bs.ReadUInt32();
            uint unkKeyMap0x54 = bs.ReadUInt32();
            uint registerValOffset = bs.ReadUInt32();
            bs.ReadUInt32();
            bs.ReadUInt32(); // Unk
            uint vmBytecodeOffset = bs.ReadUInt32();
            uint vmBytecodeSize = bs.ReadUInt32();
            uint vmInstanceOffset = bs.ReadUInt32();
            bs.ReadUInt32(); // Unk
            bs.ReadUInt32(); // Unk
            uint offset_0x78 = bs.ReadUInt32();
            bs.Position += 0xC; // Unks
            uint offset_0x8c = bs.ReadUInt32();
            ushort count_0x8c = bs.ReadUInt16();
            ushort textureKeyCount = bs.ReadUInt16();
            uint textureKeysOffset = bs.ReadUInt32();
            bs.ReadUInt32(); // Unk
            ushort wingDataCount = bs.ReadUInt16();
            ushort wingKeysCount = bs.ReadUInt16();
            uint wingDataOffset = bs.ReadUInt32();
            uint wingKeysOffset = bs.ReadUInt32();
            uint offset_0xA4 = bs.ReadUInt32();
            bs.ReadUInt32(); // Unk
            uint shapeStreamMapOffset = bs.ReadUInt32();
            uint unkVMDataOffset = bs.ReadUInt32();
            bs.ReadUInt32(); // Unk
            uint unkVMDataOffset2 = bs.ReadUInt32();
            uint vm_related_offset_0xbc = bs.ReadUInt32();
            uint offset_0xC0 = bs.ReadUInt32();
            ushort count_0xC0 = bs.ReadUInt16();
            bs.ReadUInt16(); // Unk
            bs.ReadInt16(); // Unk
            ushort pmshKeyCount = bs.ReadUInt16();
            uint pmshKeysOffset = bs.ReadUInt32();
            uint pmshHeaderOffset = bs.ReadUInt32();
            uint offset_0xD4 = bs.ReadUInt32();
            bs.ReadUInt32();
            bs.ReadUInt32();
            uint carBodyStreamOffset = bs.ReadUInt32();

            // Starting to read stuff
            modelSet.ReadModels(bs, basePos, modelsOffset, modelCount);
            modelSet.ReadModelKeys(bs, basePos, modelKeysOffset, modelKeyCount);

            modelSet.ReadMeshes(bs, basePos, meshesOffset, meshesCount);
            modelSet.ReadMeshKeys(bs, basePos, meshKeysOffset, meshKeysCount);

            modelSet.ReadFlexibleVertices(bs, basePos, flexibleVerticesOffset, flexibleVerticesCount);

            modelSet.ReadMaterials(bs, basePos, materialsOffset, 1);
            
            modelSet.ReadTextureSet(bs, basePos, textureSetOffset, 1);

            modelSet.ReadShaders(bs, basePos, shadersOffset, 1);
            modelSet.ReadBones(bs, basePos, bonesOffset, bonesCount);

            modelSet.ReadVMBytecode(bs, basePos, vmBytecodeOffset, vmBytecodeSize);
            modelSet.ReadVMRegisterValues(bs, basePos, registerValOffset, registerValCount);
            modelSet.VirtualMachine.Print(modelSet.VMHostMethodEntries);
            modelSet.ReadTextureKeys(bs, basePos, textureKeysOffset, textureKeyCount);
            modelSet.ReadWingData(bs, basePos, wingDataOffset, wingDataCount);
            modelSet.ReadWingKeys(bs, basePos, wingKeysOffset, wingKeysCount);
            modelSet.ReadUnkVMData(bs, basePos, unkVMDataOffset, modelCount);
            modelSet.ReadUnkVMData2(bs, basePos, unkVMDataOffset2, 1);
            modelSet.ReadUnkVMContext(bs, basePos, vm_related_offset_0xbc, 1);
            modelSet.ReadPMSHKeys(bs, basePos, pmshKeysOffset, pmshKeyCount);
            modelSet.ReadPMSH(bs, basePos, pmshHeaderOffset, 1);
            modelSet.ReadStreamInfo(bs, basePos, shapeStreamMapOffset, 1);

            // link everything together
            modelSet.LinkAll();

            return modelSet;
        }

        private void LinkAll()
        {
            foreach (var modelKey in ModelKeys)
                Models[(ushort)modelKey.ModelID].Name = modelKey.Name;

            foreach (var meshKey in MeshKeys)
                Meshes[(ushort)meshKey.MeshID].Name = meshKey.Name;

            foreach (var wingKey in WingKeys)
                WingData[(ushort)wingKey.WingDataID].Name = wingKey.Name;

            foreach (var mesh in Meshes)
            {
                if (mesh.FVFIndex != -1)
                    mesh.FVF = FlexibleVertexFormats[mesh.FVFIndex];

                if (mesh.MaterialIndex != -1)
                    mesh.Material = Materials.Definitions[mesh.MaterialIndex];
            }
        }

        private void ReadModels(BinaryStream bs, long baseMdlPos, uint offset, uint count)
        {
            for (ushort i = 0; i < count; i++)
            {
                bs.Position = baseMdlPos + offset + (i * ModelSet3Model.GetSize());
                Models.Add(ModelSet3Model.FromStream(bs, baseMdlPos, Version));
            }
        }

        private void ReadModelKeys(BinaryStream bs, long baseMdlPos, uint offset, uint count)
        {
            for (ushort i = 0; i < count; i++)
            {
                bs.Position = baseMdlPos + offset + (i * MDL3ModelKey.GetSize());

                var key = MDL3ModelKey.FromStream(bs, baseMdlPos, Version);
                ModelKeys.Add(key);
            }
        }

        private void ReadMeshes(BinaryStream bs, long baseMdlPos, uint offset, uint count)
        {
            for (ushort i = 0; i < count; i++)
            {
                bs.Position = baseMdlPos + offset + (i * MDL3Mesh.GetSize());
                Meshes.Add(MDL3Mesh.FromStream(bs, baseMdlPos, Version));
            }
        }

        private void ReadMeshKeys(BinaryStream bs, long baseMdlPos, uint offset, uint count)
        {
            for (ushort i = 0; i < count; i++)
            {
                bs.Position = baseMdlPos + offset + (i * MDL3MeshKey.GetSize());

                var key = MDL3MeshKey.FromStream(bs, baseMdlPos, Version);
                MeshKeys.Add(key);

                Meshes[(ushort)key.MeshID].Name = key.Name;
            }
        }

        private void ReadFlexibleVertices(BinaryStream bs, long baseMdlPos, uint offset, uint count)
        {
            for (ushort i = 0; i < count; i++)
            {
                bs.Position = baseMdlPos + offset + (i * MDL3FlexibleVertexDefinition.GetSize());
                FlexibleVertexFormats.Add(MDL3FlexibleVertexDefinition.FromStream(bs, baseMdlPos, Version));
            }
        }

        private void ReadVMBytecode(BinaryStream bs, long baseMdlPos, uint offset, uint size)
        {
            bs.Position = baseMdlPos + offset;
            VirtualMachine.ReadBytecode(bs);
        }

        private void ReadVMRegisterValues(BinaryStream bs, long baseMdlPos, uint offset, uint count)
        {
            for (ushort i = 0; i < count; i++)
            {
                bs.Position = baseMdlPos + offset + i * 0x08;

                var hostMethodEntry = VMHostMethodEntry.FromStream(bs, baseMdlPos, Version);
                VMHostMethodEntries.Add(hostMethodEntry.StorageID, hostMethodEntry);
            }
        }

        private void ReadStreamInfo(BinaryStream bs, long baseMdlPos, uint offset, uint count)
        {
            if (Version <= 1)
                return;

            if (offset == 0)
                return;

            bs.Position = baseMdlPos + offset;
            StreamingInfo = MDL3ShapeStreamingMap.FromStream(bs, baseMdlPos, Version);
        }

        private void ReadTextureSet(BinaryStream bs, long baseMdlPos, uint offset, uint count)
        {
            bs.Position = baseMdlPos + offset;
            TextureSet = new TextureSet3();
            TextureSet.FromStream(bs, TextureSet3.TextureConsoleType.PS3);
        }

        private void ReadShaders(BinaryStream bs, long baseMdlPos, uint offset, uint count)
        {
            bs.Position = baseMdlPos + offset;
            Shaders = ShadersHeader.FromStream(bs, baseMdlPos);
        }

        private void ReadMaterials(BinaryStream bs, long baseMdlPos, uint offset, uint count)
        {
            bs.Position = baseMdlPos + offset;
            Materials = MDL3Materials.FromStream(bs, baseMdlPos, Version);
        }

        private void ReadBones(BinaryStream bs, long baseMdlPos, uint offset, uint count)
        {
            for (var i = 0; i < count; i++)
            {
                bs.Position = baseMdlPos + offset + (i * MDL3Bone.GetSize());
                MDL3Bone bone = MDL3Bone.FromStream(bs, baseMdlPos);
                Bones.Add(bone);
            }
        }

        private void ReadTextureKeys(BinaryStream bs, long baseMdlPos, uint offset, uint count)
        {
            for (var i = 0; i < count; i++)
            {
                bs.Position = baseMdlPos + offset + (i * MDL3TextureKey.GetSize());
                var key = MDL3TextureKey.FromStream(bs, baseMdlPos);
                TextureKeys.Add(key);
            }
        }

        private void ReadWingData(BinaryStream bs, long baseMdlPos, uint offset, uint count)
        {
            for (var i = 0; i < count; i++)
            {
                bs.Position = baseMdlPos + offset + (i * MDL3WingData.GetSize());
                var data = MDL3WingData.FromStream(bs, baseMdlPos, Version);
                WingData.Add(data);
            }
        }

        private void ReadWingKeys(BinaryStream bs, long baseMdlPos, uint offset, uint count)
        {
            for (var i = 0; i < count; i++)
            {
                bs.Position = baseMdlPos + offset + (i * MDL3WingKey.GetSize());
                var key = MDL3WingKey.FromStream(bs, baseMdlPos, Version);
                WingKeys.Add(key);
            }
        }

        private void ReadUnkVMData(BinaryStream bs, long baseMdlPos, uint offset, uint count)
        {
            for (var i = 0; i < count; i++)
            {
                bs.Position = baseMdlPos + offset + (i * MDL3ModelVMUnk.GetSize());
                var unk = MDL3ModelVMUnk.FromStream(bs, baseMdlPos, Version);
                UnkVMData.Add(unk);
            }
        }

        private void ReadUnkVMData2(BinaryStream bs, long baseMdlPos, uint offset, uint count)
        {
            bs.Position = baseMdlPos + offset;
            UnkVMData2 = MDL3ModelVMUnk2.FromStream(bs, baseMdlPos, Version);
        }

        private void ReadUnkVMContext(BinaryStream bs, long baseMdlPos, uint offset, uint count)
        {
            bs.Position = baseMdlPos + offset;
            VMContext = MDL3ModelVMContext.FromStream(bs, baseMdlPos, Version);
        }

        private void ReadPMSHKeys(BinaryStream bs, long baseMdlPos, uint offset, uint count)
        {
            for (var i = 0; i < count; i++)
            {
                bs.Position = baseMdlPos + offset + (i * MDL3PMSHKey.GetSize());
                var key = MDL3PMSHKey.FromStream(bs, baseMdlPos, Version);
                PMSHKeys.Add(key);
            }
        }

        private void ReadPMSH(BinaryStream bs, long baseMdlPos, uint offset, uint count)
        {
            if (offset != 0)
            {
                bs.Position = baseMdlPos + offset;
                PMSH = MDL3PMSH.FromStream(bs, baseMdlPos, Version);
            }
        }

        /// <summary>
        /// Gets all the vertices for a specified mesh.
        /// </summary>
        /// <param name="meshIndex"></param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        /// <exception cref="NotSupportedException"></exception>
        public Vector3[] GetVerticesOfMesh(short meshIndex)
        {
            if (meshIndex == -1)
                throw new InvalidOperationException("Mesh Index was -1.");

            var mesh = Meshes[meshIndex];

            if (mesh.FVFIndex != -1)
            {
                MDL3FlexibleVertexDefinition fvfDef = FlexibleVertexFormats[mesh.FVFIndex];
                if (!fvfDef.Elements.TryGetValue("position", out var field)
                    && !fvfDef.Elements.TryGetValue("position_1", out field)
                    && !fvfDef.Elements.TryGetValue("position_2", out field))
                    throw new InvalidOperationException("FVF does not contain 'position' field.");

                if (field.FieldType != CELL_GCM_VERTEX_TYPE.CELL_GCM_VERTEX_F && field.FieldType != CELL_GCM_VERTEX_TYPE.CELL_GCM_VERTEX_S1)
                    throw new NotSupportedException("Expected vector 3 with CELL_GCM_VERTEX_F or CELL_GCM_VERTEX_S1");

                var arr = new Vector3[mesh.VertexCount];

                if (mesh.VerticesOffset != 0)
                {
                    Span<byte> vertBuffer = new byte[field.ArrayIndex == 0 ? fvfDef.VertexSize : fvfDef.ArrayDefinition.VertexSize];
                    for (int i = 0; i < mesh.VertexCount; i++)
                    {
                        GetVerticesData(mesh, fvfDef, field, i, vertBuffer);
                        arr[i] = field.GetFVFFieldVector3(vertBuffer);
                    }
                }
                else
                {
                    // TODO: Try shapestream
                    // TODO: Find the mechanism that determines whether we are using a stream or not
                }

                return arr;
            }
            else if (mesh.PMSHRef != null)
            {
                var format = PMSH;
            }

            return null;
            
        }

        public List<Tri> GetTrisOfMesh(ushort meshIndex)
        {
            MDL3Mesh mesh = Meshes[meshIndex];
            var list = new List<Tri>();

            if (mesh.TriOffset != 0)
            {
                Stream.Position = BaseOffset + mesh.TriOffset;
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
            MDL3FlexibleVertexDefinition fvfDef = FlexibleVertexFormats[mesh.FVFIndex];
            if (!fvfDef.Elements.TryGetValue("map1", out var field) &&
                !fvfDef.Elements.TryGetValue("map12", out field) &&
                !fvfDef.Elements.TryGetValue("map12_2", out field))
                return Array.Empty<Vector2>();

            var arr = new Vector2[mesh.VertexCount];

            if (mesh.VerticesOffset != 0)
            {
                Span<byte> buffer = new byte[field.ArrayIndex == 0 ? fvfDef.VertexSize : fvfDef.ArrayDefinition.VertexSize];
                for (int i = 0; i < mesh.VertexCount; i++)
                {
                    GetVerticesData(mesh, fvfDef, field, i, buffer);
                    arr[i] = field.GetFVFFieldVector2(buffer);
                }
            }
            else
            {
                // TODO: Try shapestream
                // TODO: Find the mechanism that determines whether we are using a stream or not
            }

            return arr;
        }


        public void GetVerticesData(MDL3Mesh meshInfo, MDL3FlexibleVertexDefinition fvfDef, MDL3FVFElementDefinition field, int vertIndex, Span<byte> buffer)
        {
            if (field.ArrayIndex == 0)
            {
                Stream.Position = BaseOffset + meshInfo.VerticesOffset + vertIndex * fvfDef.VertexSize;
                Stream.Read(buffer);
            }
            else
            {
                Stream.Position = BaseOffset + meshInfo.VerticesOffset + fvfDef.ArrayDefinition.DataOffset + (fvfDef.ArrayDefinition.ArrayElementSize * field.ArrayIndex);
                Stream.Position += vertIndex * fvfDef.ArrayDefinition.VertexSize;
                Stream.Read(buffer);
            }
        }

        public static int GetHeaderSize()
        {
            return 0xE4;
        }
    }
}

