using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Buffers.Binary;

using Syroot.BinaryData.Core;
using Syroot.BinaryData;
using Syroot.BinaryData.Memory;

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
using PDTools.Files.Models.ModelSet3.PackedMesh;
using PDTools.Files.Models.ShapeStream;

using ShapeStreamData = PDTools.Files.Models.ShapeStream.ShapeStream;
using System.Reflection;
using BCnEncoder.Shared;

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
        public List<PackedMeshKey> PackedMeshKeys { get; set; } = new();
        public PackedMeshHeader PackedMesh { get; set; } = new();
        public MDL3ShapeStreamingManager StreamingInfo { get; set; }
        public ShapeStreamData ShapeStream { get; set; }

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
            ushort packedMeshKeyCount = bs.ReadUInt16();
            uint packedMeshKeysOffset = bs.ReadUInt32();
            uint packedMeshHeaderOffset = bs.ReadUInt32();
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

            //modelSet.ReadVMBytecode(bs, basePos, vmBytecodeOffset, vmBytecodeSize);
            //modelSet.ReadVMRegisterValues(bs, basePos, registerValOffset, registerValCount);
            //modelSet.VirtualMachine.Print(modelSet.VMHostMethodEntries);
            modelSet.ReadTextureKeys(bs, basePos, textureKeysOffset, textureKeyCount);
            modelSet.ReadWingData(bs, basePos, wingDataOffset, wingDataCount);
            modelSet.ReadWingKeys(bs, basePos, wingKeysOffset, wingKeysCount);
            modelSet.ReadUnkVMData(bs, basePos, unkVMDataOffset, modelCount);
            modelSet.ReadUnkVMData2(bs, basePos, unkVMDataOffset2, 1);
            modelSet.ReadUnkVMContext(bs, basePos, vm_related_offset_0xbc, 1);
            modelSet.ReadPackedMeshKeys(bs, basePos, packedMeshKeysOffset, packedMeshKeyCount);
            modelSet.ReadPackedMesh(bs, basePos, packedMeshHeaderOffset, 1);
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
            StreamingInfo = MDL3ShapeStreamingManager.FromStream(bs, baseMdlPos, Version);
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

        private void ReadPackedMeshKeys(BinaryStream bs, long baseMdlPos, uint offset, uint count)
        {
            for (var i = 0; i < count; i++)
            {
                bs.Position = baseMdlPos + offset + (i * PackedMeshKey.GetSize());
                var key = PackedMeshKey.FromStream(bs, baseMdlPos, Version);
                PackedMeshKeys.Add(key);
            }
        }

        private void ReadPackedMesh(BinaryStream bs, long baseMdlPos, uint offset, uint count)
        {
            if (Version < 9)
                return;

            if (offset != 0)
            {
                bs.Position = baseMdlPos + offset;
                PackedMesh = PackedMeshHeader.FromStream(bs, baseMdlPos, Version);
            }
        }

        /// <summary>
        /// Gets all the vertices for a specified mesh.
        /// </summary>
        /// <param name="meshIndex"></param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        /// <exception cref="NotSupportedException"></exception>
        public Vector3[] GetVerticesOfMesh(ushort meshIndex)
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

                if (field.FieldType != CELL_GCM_VERTEX_TYPE.CELL_GCM_VERTEX_F && field.FieldType != CELL_GCM_VERTEX_TYPE.CELL_GCM_VERTEX_S1 && field.FieldType != CELL_GCM_VERTEX_TYPE.CELL_GCM_VERTEX_SF)
                    throw new NotSupportedException("Expected vector 3 with CELL_GCM_VERTEX_F or CELL_GCM_VERTEX_S1");

                var arr = new Vector3[mesh.VertexCount];
                if (mesh.VerticesOffset != 0 && Stream.CanRead)
                {
                    Span<byte> vertBuffer = new byte[field.ArrayIndex == 0 ? fvfDef.VertexSize : fvfDef.ArrayDefinition.VertexSize];
                    for (int i = 0; i < mesh.VertexCount; i++)
                    {
                        GetVerticesData(mesh, fvfDef, field, i, vertBuffer);
                        arr[i] = GetFVFFieldVector3(vertBuffer, field.FieldType, field.StartOffset, field.ElementCount);
                    }
                }
                else if (ShapeStream != null)
                {
                    // Try shapestream
                    var ssMesh = ShapeStream.GetMeshByIndex(meshIndex);
                    if (ssMesh is null)
                        return arr;

                    Span<byte> vertBuffer = new byte[field.ArrayIndex == 0 ? fvfDef.VertexSize : fvfDef.ArrayDefinition.VertexSize];
                    for (int i = 0; i < mesh.VertexCount; i++)
                    {
                        GetShapeStreamVerticesData(ssMesh, fvfDef, field, i, vertBuffer);
                        arr[i] = GetFVFFieldVector3(vertBuffer, field.FieldType, field.StartOffset, field.ElementCount);
                    }
                }

                return arr;
            }
            else if (Version >= 9 && PackedMesh != null && mesh.PackedMeshRef != null)
            {
                PackedMeshEntry entry = PackedMesh.Entries[mesh.PackedMeshRef.PackedMeshEntryIndex];
                PackedMeshFlexVertexDefinition flexDef = PackedMesh.StructDeclarations[entry.StructDeclarationID];
                PackedMeshFlexVertexElementDefinition element = flexDef.GetElement("position");

                if (element is null)
                    return null;

                if (element.IsPacked)
                {
                    PackedMeshElementBitLayoutArray bitLayouts = PackedMesh.BitLayoutDefinitionArray[entry.ElementBitLayoutDefinitionID];
                    var arr = new Vector3[entry.Data.PackedFlexVertCount];

                    PackedMeshElementBitLayout bitDef = GetPackedBitLayoutOfField(bitLayouts, flexDef, element.Name);
                    for (int i = 0; i < entry.Data.PackedFlexVertCount; i++)
                    {
                        var v4 = ReadPackedElement(entry, flexDef, bitLayouts, bitDef, element, i);
                        arr[i] = new Vector3(v4.X, v4.Y, v4.Z);
                    }

                    return arr;
                }
                else
                {
                    Span<byte> vertBuffer = new byte[flexDef.NonPackedStride];
                    var arr = new Vector3[entry.Data.PackedFlexVertCount];

                    for (int i = 0; i < entry.Data.PackedFlexVertCount; i++)
                    {
                        GetPackedMeshRawElementBuffer(entry, flexDef, i, vertBuffer);
                        arr[i] = GetFVFFieldVector3(vertBuffer, element.Type, element.OutputFlexOffset, element.ElementCount);
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// Gets all the tris for a specified mesh.
        /// </summary>
        /// <param name="meshIndex"></param>
        /// <returns></returns>
        public List<Tri> GetTrisOfMesh(ushort meshIndex)
        {
            MDL3Mesh mesh = Meshes[meshIndex];
            var list = new List<Tri>();

            if (mesh.TriOffset != 0 && Stream.CanRead)
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
            else if (ShapeStream != null)
            {
                // Try shapestream
                var ssMesh = ShapeStream.GetMeshByIndex(meshIndex);
                if (ssMesh is null)
                    return null;

                SpanReader meshReader = new SpanReader(ssMesh.MeshData.Span, Endian.Big);
                meshReader.Position = (int)ssMesh.TriOffset;

                for (int i = 0; i < mesh.TriCount; i++)
                {
                    ushort a = meshReader.ReadUInt16();
                    ushort b = meshReader.ReadUInt16();
                    ushort c = meshReader.ReadUInt16();
                    if (a < mesh.VertexCount && b < mesh.VertexCount && c < mesh.VertexCount)
                    {
                        list.Add(new(a, b, c));
                    }
                    else
                    {
                        return null; // Tristrip - FIX ME
                    }
                }

                return list;
            }

            return list;
        }

        /* NOTE for UVs:
        * Sometimes the scale is off, it's been a rabbit hole figuring out why
        * Turns out it's handled by shader programs (yes).
        * Order of fetching is:
        * 1. Mesh -> Material ID -> Material Entry
        * 2. Material Entry -> Material Data ID -> Material Data Entry
        * 3. Material Data Entry -> 0x14 Entry -> Shader Entry Index -> Shader Entry 0x3C or Shader Def Entry
        * 4. Shader Def Entry -> Shader Program ID -> Shader Program Entry
        * 5. Shader Program -> Actual Program data.
        * 
        * UV stuff is handling there, not sure how that's done yet. The floats don't seem to be directly it - didn't seem to work with GT6 midfield
        * */
        /// <summary>
        /// Gets all the UVs for a specified mesh. Read comment above this function as there are some issues
        /// </summary>
        /// <param name="meshIndex"></param>
        /// <returns></returns>
        public Vector2[] GetUVsOfMesh(ushort meshIndex)
        {
            var mesh = Meshes[meshIndex];
            Vector2[] arr;

            var mat = this.Materials.Definitions[mesh.MaterialIndex];
            MDL3MaterialData matData = this.Materials.MaterialDatas[mat.MaterialDataID];
            ShaderDefinition shader = this.Shaders.Definitions[matData._0x14.ShaderID];
            var prog = this.Shaders.Programs0x20[shader.ProgramID];

            /*
            float scaleX = BinaryPrimitives.ReadSingleBigEndian(prog.Program.AsSpan(0x20));
            float scaleY = BinaryPrimitives.ReadSingleBigEndian(prog.Program.AsSpan(0x24));
            float scaleZ = BinaryPrimitives.ReadSingleBigEndian(prog.Program.AsSpan(0x28));
            */

            if (mesh.FVFIndex != -1)
            {
                MDL3FlexibleVertexDefinition fvfDef = FlexibleVertexFormats[mesh.FVFIndex];
                if (!fvfDef.Elements.TryGetValue("map1", out var field) &&
                    !fvfDef.Elements.TryGetValue("map12", out field) &&
                    !fvfDef.Elements.TryGetValue("map12_2", out field))
                    return Array.Empty<Vector2>();

                arr = new Vector2[mesh.VertexCount];
                if (mesh.VerticesOffset != 0 && Stream.CanRead)
                {
                    Span<byte> buffer = new byte[field.ArrayIndex == 0 ? fvfDef.VertexSize : fvfDef.ArrayDefinition.VertexSize];
                    for (int i = 0; i < mesh.VertexCount; i++)
                    {
                        GetVerticesData(mesh, fvfDef, field, i, buffer);
                        arr[i] = GetFVFFieldVector2(buffer, field.FieldType, field.StartOffset, field.ElementCount);
                    }
                }
                else if (ShapeStream != null)
                {
                    // Try shapestream
                    var ssMesh = ShapeStream.GetMeshByIndex(meshIndex);
                    if (ssMesh is null)
                        return arr;

                    Span<byte> buffer = new byte[field.ArrayIndex == 0 ? fvfDef.VertexSize : fvfDef.ArrayDefinition.VertexSize];
                    for (int i = 0; i < mesh.VertexCount; i++)
                    {
                        GetShapeStreamVerticesData(ssMesh, fvfDef, field, i, buffer);
                        arr[i] = GetFVFFieldVector2(buffer, field.FieldType, field.StartOffset, field.ElementCount);
                    }
                }
            }
            else if (Version >= 9 && PackedMesh != null && mesh.PackedMeshRef != null)
            {
                PackedMeshEntry entry = PackedMesh.Entries[mesh.PackedMeshRef.PackedMeshEntryIndex];
                PackedMeshFlexVertexDefinition flexDef = PackedMesh.StructDeclarations[entry.StructDeclarationID];
                PackedMeshFlexVertexElementDefinition element = flexDef.GetElement("map12");

                if (element is null)
                    return null;

                if (element.IsPacked)
                {
                    PackedMeshElementBitLayoutArray bitLayouts = PackedMesh.BitLayoutDefinitionArray[entry.ElementBitLayoutDefinitionID];
                    arr = new Vector2[entry.Data.PackedFlexVertCount];

                    PackedMeshElementBitLayout bitDef = GetPackedBitLayoutOfField(bitLayouts, flexDef, element.Name);
                    for (int i = 0; i < entry.Data.PackedFlexVertCount; i++)
                    {
                        var v4 = ReadPackedElement(entry, flexDef, bitLayouts, bitDef, element, i);
                        arr[i] = new Vector2(v4.X, v4.Y);
                    }
                }
                else
                {
                    Span<byte> vertBuffer = new byte[flexDef.NonPackedStride];
                    arr = new Vector2[entry.Data.PackedFlexVertCount];

                    for (int i = 0; i < entry.Data.PackedFlexVertCount; i++)
                    {
                        GetPackedMeshRawElementBuffer(entry, flexDef, i, vertBuffer);
                        arr[i] = GetFVFFieldVector2(vertBuffer, element.Type, element.OutputFlexOffset, element.ElementCount);
                    }
                }
            }
            else
            {
                return null;
            }

            return arr;
        }

        /// <summary>
        /// Gets all the normals for a specified mesh.
        /// </summary>
        /// <param name="meshIndex"></param>
        /// <returns></returns>
        public (uint, uint, uint)[] GetNormalsOfMesh(ushort meshIndex)
        {
            var mesh = Meshes[meshIndex];
            if (mesh.FVFIndex != -1)
            {
                MDL3FlexibleVertexDefinition fvfDef = FlexibleVertexFormats[mesh.FVFIndex];
                if (!fvfDef.Elements.TryGetValue("normal", out var field))
                    return Array.Empty<(uint, uint, uint)>();

                if (mesh.VerticesOffset != 0 && Stream.CanRead)
                {
                    var arr = new (uint, uint, uint)[mesh.VertexCount];
                    Span<byte> buffer = new byte[field.ArrayIndex == 0 ? fvfDef.VertexSize : fvfDef.ArrayDefinition.VertexSize];
                    for (int i = 0; i < mesh.VertexCount; i++)
                    {
                        GetVerticesData(mesh, fvfDef, field, i, buffer);
                        arr[i] = GetFVFFieldXYZ(buffer, field.FieldType, field.StartOffset, field.ElementCount);
                    }

                    return arr;
                }
                else if (ShapeStream != null)
                {
                    var arr = new (uint, uint, uint)[mesh.VertexCount];

                    // Try shapestream
                    var ssMesh = ShapeStream.GetMeshByIndex(meshIndex);
                    if (ssMesh is null)
                        return arr;

                    Span<byte> buffer = new byte[field.ArrayIndex == 0 ? fvfDef.VertexSize : fvfDef.ArrayDefinition.VertexSize];
                    for (int i = 0; i < mesh.VertexCount; i++)
                    {
                        GetShapeStreamVerticesData(ssMesh, fvfDef, field, i, buffer);
                        arr[i] = GetFVFFieldXYZ(buffer, field.FieldType, field.StartOffset, field.ElementCount);
                    }

                    return arr;
                }

            }
            else if (Version >= 9 && PackedMesh != null && mesh.PackedMeshRef != null)
            {
                PackedMeshEntry entry = PackedMesh.Entries[mesh.PackedMeshRef.PackedMeshEntryIndex];
                PackedMeshFlexVertexDefinition flexDef = PackedMesh.StructDeclarations[entry.StructDeclarationID];
                PackedMeshFlexVertexElementDefinition element = flexDef.GetElement("normal");

                if (element is null)
                    return null;

                if (element.IsPacked)
                {
                    var arr = new (uint, uint, uint)[entry.Data.PackedFlexVertCount];

                    PackedMeshElementBitLayoutArray bitLayouts = PackedMesh.BitLayoutDefinitionArray[entry.ElementBitLayoutDefinitionID];
                    PackedMeshElementBitLayout bitDef = GetPackedBitLayoutOfField(bitLayouts, flexDef, element.Name);

                    for (int i = 0; i < entry.Data.PackedFlexVertCount; i++)
                    {
                        var v4 = ReadPackedElement(entry, flexDef, bitLayouts, bitDef, element, i);
                        arr[i] = ((uint)v4.X, (uint)v4.Y, (uint)v4.Z);
                    }

                    return arr;
                }
                else
                {
                    var arr = new (uint, uint, uint)[entry.Data.NonPackedFlexVertCount];
                    Span<byte> vertBuffer = new byte[flexDef.NonPackedStride];

                    for (int i = 0; i < entry.Data.NonPackedFlexVertCount; i++)
                    {
                        GetPackedMeshRawElementBuffer(entry, flexDef, i, vertBuffer);
                        arr[i] = GetFVFFieldXYZ(vertBuffer, element.Type, element.OutputFlexOffset, element.ElementCount);
                    }

                    return arr;
                }
            }

            return null;
        }

        /// <summary>
        /// Gets the BBox of a mesh.
        /// </summary>
        /// <param name="meshIndex"></param>
        /// <returns></returns>
        public Vector3[]? GetBBoxOfMesh(ushort meshIndex)
        {
            var mesh = Meshes[meshIndex];

            if (mesh.BBox == null && ShapeStream != null)
            {
                // Try shapestream
                var ssMesh = ShapeStream.GetMeshByIndex(meshIndex);
                if (ssMesh is null)
                    return null;

                SpanReader meshReader = new SpanReader(ssMesh.MeshData.Span, Endian.Big);

                meshReader.Position = (int)ssMesh.BBoxOffset;
                mesh.BBox = new Vector3[8];
                for (var i = 0; i < 8; i++)
                    mesh.BBox[i] = new Vector3(meshReader.ReadSingle(), meshReader.ReadSingle(), meshReader.ReadSingle());
            }

            return mesh.BBox;
        }

        private Vector4 ReadPackedElement(PackedMeshEntry entry, 
            PackedMeshFlexVertexDefinition flexDef, 
            PackedMeshElementBitLayoutArray bitLayouts, 
            PackedMeshElementBitLayout bitDef,
            PackedMeshFlexVertexElementDefinition element,
            int vertIndex)
        {
            
            Stream.Position = entry.Data.PackedFlexVertsOffset + entry.Data.GetOffsetOfPackedElement(bitLayouts, flexDef, element.Name);
            Stream.Position += (bitDef.TotalBitCount * vertIndex) / 8;
            int rem = (bitDef.TotalBitCount * vertIndex) % 8;

            Span<byte> vertBuffer = new byte[((bitDef.TotalBitCount + 7) / 8) + 1];
            Stream.Read(vertBuffer);

            BitStream bs = new BitStream(BitStreamMode.Read, vertBuffer);
            bs.ReadBits(rem);

            ulong packX = bs.ReadBits(bitDef.XBitCount);
            ulong packY = bs.ReadBits(bitDef.YBitCount);
            ulong packZ = bs.ReadBits(bitDef.ZBitCount);
            ulong packW = bs.ReadBits(bitDef.WBitCount);

            float x = packX / (float)MiscUtils.GetMaxSignedForBitCount(bitDef.XBitCount);
            float y = packY / (float)MiscUtils.GetMaxSignedForBitCount(bitDef.YBitCount);
            float z = packZ / (float)MiscUtils.GetMaxSignedForBitCount(bitDef.ZBitCount);
            float w = packW / (float)MiscUtils.GetMaxSignedForBitCount(bitDef.WBitCount);

            x = ((x + 2f) * bitDef.ScaleX) + bitDef.OffsetX;
            y = ((y + 2f) * bitDef.ScaleY) + bitDef.OffsetY;
            z = ((z + 2f) * bitDef.ScaleZ) + bitDef.OffsetZ;
            w = ((w + 2f) * bitDef.ScaleW) + bitDef.OffsetW;

            return new Vector4(x, y, z, w);
        }

        /// <summary>
        /// Gets a flex vertex
        /// </summary>
        /// <param name="meshInfo"></param>
        /// <param name="fvfDef"></param>
        /// <param name="field"></param>
        /// <param name="vertIndex"></param>
        /// <param name="buffer"></param>
        public void GetVerticesData(MDL3Mesh meshInfo, MDL3FlexibleVertexDefinition fvfDef, MDL3FVFElementDefinition field, int vertIndex, Span<byte> buffer)
        {
            if (field.ArrayIndex == 0)
            {
                Stream.Position = BaseOffset + meshInfo.VerticesOffset + (vertIndex * fvfDef.VertexSize);
                Stream.Read(buffer);
            }
            else
            {
                Stream.Position = BaseOffset + meshInfo.VerticesOffset + fvfDef.ArrayDefinition.DataOffset + (fvfDef.ArrayDefinition.ArrayElementSize * field.ArrayIndex);
                Stream.Position += vertIndex * fvfDef.ArrayDefinition.VertexSize;
                Stream.Read(buffer);
            }
        }

        /// <summary>
        /// Gets a flex vertex stride from a shapestream
        /// </summary>
        /// <param name="ssMeshInfo"></param>
        /// <param name="fvfDef"></param>
        /// <param name="field"></param>
        /// <param name="vertIndex"></param>
        /// <param name="buffer"></param>
        public void GetShapeStreamVerticesData(ShapeStreamMesh ssMeshInfo, MDL3FlexibleVertexDefinition fvfDef, MDL3FVFElementDefinition field, int vertIndex, Span<byte> buffer)
        {
            SpanReader meshReader = new SpanReader(ssMeshInfo.MeshData.Span);
            if (field.ArrayIndex == 0)
            {
                meshReader.Position = (int)(ssMeshInfo.VerticesOffset + (vertIndex * fvfDef.VertexSize));
                meshReader.Span.Slice(meshReader.Position, fvfDef.VertexSize).CopyTo(buffer);
            }
            else
            {
                meshReader.Position = (int)(ssMeshInfo.VerticesOffset + fvfDef.ArrayDefinition.DataOffset + (fvfDef.ArrayDefinition.ArrayElementSize * field.ArrayIndex));
                meshReader.Position += vertIndex * fvfDef.ArrayDefinition.VertexSize;
                meshReader.Span.Slice(meshReader.Position, fvfDef.ArrayDefinition.VertexSize).CopyTo(buffer);
            }
        }

        public void GetPackedMeshRawElementBuffer(PackedMeshEntry entry, PackedMeshFlexVertexDefinition flexStruct, int vertIndex, Span<byte> buffer)
        {
            Stream.Position = BaseOffset + entry.Data.NonPackedFlexVertsOffset + (vertIndex * flexStruct.NonPackedStride);
            Stream.Read(buffer);
        }

        private PackedMeshElementBitLayout GetPackedBitLayoutOfField(PackedMeshElementBitLayoutArray bitLayouts, PackedMeshFlexVertexDefinition flexStruct, string type)
        {
            int currentLayoutIndex = 0;
            foreach (var elem in flexStruct.PackedElements)
            {
                if (elem.Key == "colorSet1")
                    continue;

                if (elem.Key == type)
                    return bitLayouts.Layouts[currentLayoutIndex];

                currentLayoutIndex++;
            }

            return null;
        }

        /// <summary>
        /// Reads a vector 3 from a field of a flex vertex
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="fieldType"></param>
        /// <param name="startOffset"></param>
        /// <param name="elementCount"></param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        /// <exception cref="NotImplementedException"></exception>
        public Vector3 GetFVFFieldVector3(Span<byte> buffer, CELL_GCM_VERTEX_TYPE fieldType, int startOffset, int elementCount)
        {
            float v1, v2, v3;

            if (elementCount != 3)
                throw new InvalidOperationException("Expected 3 elements for Vector3");

            SpanReader sr = new SpanReader(buffer, Endian.Big);
            sr.Position = startOffset;

            if (fieldType == CELL_GCM_VERTEX_TYPE.CELL_GCM_VERTEX_F)
            {
                v1 = sr.ReadSingle();
                v2 = sr.ReadSingle();
                v3 = sr.ReadSingle();
            }
            else if (fieldType == CELL_GCM_VERTEX_TYPE.CELL_GCM_VERTEX_S1)
            {
                v1 = sr.ReadUInt16() * (1f / short.MaxValue);
                v2 = sr.ReadUInt16() * (1f / short.MaxValue);
                v3 = sr.ReadUInt16() * (1f / short.MaxValue);
            }
            else if (fieldType == CELL_GCM_VERTEX_TYPE.CELL_GCM_VERTEX_UB)
            {
                v1 = sr.ReadByte() * (1f / sbyte.MaxValue);
                v2 = sr.ReadByte() * (1f / sbyte.MaxValue);
                v3 = sr.ReadByte() * (1f / sbyte.MaxValue);
            }
            else if (fieldType == CELL_GCM_VERTEX_TYPE.CELL_GCM_VERTEX_SF)
            {
                var bytes = sr.ReadBytes(2);
                var bytes2 = sr.ReadBytes(2);
                var bytes3 = sr.ReadBytes(2);
                v1 = (float)(sr.Endian == Endian.Big ? BinaryPrimitives.ReadHalfBigEndian(bytes) : BinaryPrimitives.ReadHalfLittleEndian(bytes));
                v2 = (float)(sr.Endian == Endian.Big ? BinaryPrimitives.ReadHalfBigEndian(bytes2) : BinaryPrimitives.ReadHalfLittleEndian(bytes2));
                v3 = (float)(sr.Endian == Endian.Big ? BinaryPrimitives.ReadHalfBigEndian(bytes3) : BinaryPrimitives.ReadHalfLittleEndian(bytes3));
            }
            else
            {
                throw new NotImplementedException($"Unimplemented field type {fieldType}");
            }

            return new Vector3(v1, v2, v3);
        }

        /// <summary>
        /// Reads 3 uints from a field of a flex vertex
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="fieldType"></param>
        /// <param name="startOffset"></param>
        /// <param name="elementCount"></param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        /// <exception cref="NotImplementedException"></exception>
        public (uint, uint, uint) GetFVFFieldXYZ(Span<byte> buffer, CELL_GCM_VERTEX_TYPE fieldType, int startOffset, int elementCount)
        {
            if (elementCount != 1)
                throw new InvalidOperationException("Expected 1 element");

            SpanReader sr = new SpanReader(buffer, Endian.Big);
            sr.Position = startOffset;

            if (fieldType == CELL_GCM_VERTEX_TYPE.CELL_GCM_VERTEX_CMP)
            {
                uint data = sr.ReadUInt32();
                return (data & 0b11_11111111, (data >> 10 & 0b111_11111111), (data >> 21 & 0b111_11111111));
            }
            else
            {
                throw new NotImplementedException($"Unimplemented field type {fieldType}");
            }
        }

        /// <summary>
        /// Reads a vector 2 from a field of a flex vertex
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="fieldType"></param>
        /// <param name="startOffset"></param>
        /// <param name="elementCount"></param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        /// <exception cref="NotImplementedException"></exception>
        public Vector2 GetFVFFieldVector2(Span<byte> buffer, CELL_GCM_VERTEX_TYPE fieldType, int startOffset, int elementCount)
        {
            float v1 = 0, v2 = 0;

            SpanReader sr = new SpanReader(buffer, Endian.Big); // Fix me..
            sr.Position = startOffset;

            if (fieldType == CELL_GCM_VERTEX_TYPE.CELL_GCM_VERTEX_F)
            {
                if (elementCount == 4)
                    ; // TODO: Check whats up with this, GT6 PS3 tracks uses 4 elements for map12 sometimes

                v1 = sr.ReadSingle();
                v2 = sr.ReadSingle();
            }
            else if (fieldType == CELL_GCM_VERTEX_TYPE.CELL_GCM_VERTEX_S1)
            {
                v1 = (float)sr.ReadInt16() / 16384f;
                v2 = (float)sr.ReadInt16() / 16384f;
            }
            else if (fieldType == CELL_GCM_VERTEX_TYPE.CELL_GCM_VERTEX_UB)
            {
                v1 = sr.ReadByte() * (1f / byte.MaxValue);
                v2 = sr.ReadByte() * (1f / byte.MaxValue);
            }
            else if (fieldType == CELL_GCM_VERTEX_TYPE.CELL_GCM_VERTEX_SF)
            {
                if (elementCount == 4)
                    ; // TODO: Check whats up with this, GT5 PS3 tracks uses 4 elements for map12 sometimes

                var bytes = sr.ReadBytes(2);
                var bytes2 = sr.ReadBytes(2);
                v1 = (float)(sr.Endian == Endian.Big ? BinaryPrimitives.ReadHalfBigEndian(bytes) : BinaryPrimitives.ReadHalfLittleEndian(bytes));
                v2 = (float)(sr.Endian == Endian.Big ? BinaryPrimitives.ReadHalfBigEndian(bytes2) : BinaryPrimitives.ReadHalfLittleEndian(bytes2));
            }
            else
            {
                throw new NotImplementedException($"Unimplemented field type {fieldType}");
            }

            return new Vector2(v1, v2);
        }

        public static int GetHeaderSize()
        {
            return 0xE4;
        }
    }
}

