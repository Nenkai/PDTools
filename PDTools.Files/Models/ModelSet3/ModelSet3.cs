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

namespace PDTools.Files.Models.ModelSet3
{
    public class ModelSet3
    {
        const string MAGIC = "MDL3";
        const string MAGIC_LE = "3LDM";

        public ushort VersionMajor { get; set; }
        public long BaseOffset { get; set; }

        public List<ModelSet3Model> Models { get; set; } = new();
        public Dictionary<uint, MDL3ModelKey> ModelKeys { get; set; } = new();

        public List<MDL3Mesh> Meshes { get; set; } = new();
        public Dictionary<uint, MDL3MeshKey> MeshKeys { get; set; } = new();

        public List<MDL3FlexibleVertexDefinition> FlexibleVertexFormats { get; set; } = new();
        
        public MDL3Materials Materials { get; set; } = new();
        public TextureSet3 TextureSet { get; set; }
        public ShadersHeader Shaders { get; set; }

        public VMBytecode VirtualMachine { get; set; } = new VMBytecode();
        public Dictionary<short, VMHostMethodEntry> VMHostMethodEntries { get; set; } = new();

        public MDL3ShapeStreamingMap StreamingInfo { get; set; }

        public BinaryStream Stream { get; set; }
        public CourseDataFile ParentCourseData { get; set; }

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
            modelSet.VersionMajor = bs.ReadUInt16();
            bs.ReadUInt16(); // Runtime Flags
            ushort modelCount = bs.ReadUInt16();
            ushort modelKeyCount = bs.ReadUInt16();
            ushort meshesCount = bs.ReadUInt16();
            ushort meshKeysCount = bs.ReadUInt16();
            ushort flexibleVerticesCount = bs.ReadUInt16();
            ushort bonesCount = bs.ReadUInt16();
            ushort unk = bs.ReadUInt16();
            ushort registerValCount = bs.ReadUInt16();
            ushort stackSize = bs.ReadUInt16(); // Unk
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

            modelSet.ReadVMBytecode(bs, basePos, vmBytecodeOffset, vmBytecodeSize);
            modelSet.ReadVMRegisterValues(bs, basePos, registerValOffset, registerValCount);
            modelSet.VirtualMachine.Print(modelSet.VMHostMethodEntries);

            modelSet.ReadStreamInfo(bs, basePos, shapeStreamMapOffset, 1);

            // link everything together
            modelSet.LinkAll();

            return modelSet;
        }

        public void Write(BinaryStream bs)
        {
            long basePos = bs.Position;

            // Seek to start of data
            bs.Position = 0xE4;

            WriteMaterialsTOC(bs, basePos);

            WriteShaderHeader(bs, basePos);

            // WRITE TODO: WRITE PMSH HEADER

            bs.Position = 0x200;
            WriteModels(bs, basePos);

            OptimizedStringTable modelKeysTable = new OptimizedStringTable();
            WriteModelKeys(bs, basePos, modelKeysTable);

            WriteMeshes(bs, basePos);

            OptimizedStringTable meshKeysTable = new OptimizedStringTable();
            WriteMeshKeys(bs, basePos, meshKeysTable);

            OptimizedStringTable fvfNameTable = new OptimizedStringTable();
            WriteFlexibleVertices(bs, basePos, fvfNameTable);

            OptimizedStringTable materialsNameTable = new OptimizedStringTable();
            WriteMaterials(bs, basePos, materialsNameTable);
        }

        private void WriteMaterialsTOC(BinaryStream bs, long baseModelSetOffset)
        {
            bs.WriteInt16((short)Materials.Materials.Count);
            bs.WriteInt16(0); // TODO
            bs.WriteByte(0); // TODO
            bs.WriteByte(0); // TODO
            bs.WriteInt16((short)Materials.TextureInfos.Count);

            // Offsets which we will write later
            bs.WriteInt32(0);
            bs.WriteInt32(0);
            bs.WriteInt32(0);
            bs.WriteInt32(0);
        }

        private void WriteShaderHeader(BinaryStream bs, long baseModelSetOffset)
        {
            long baseShdsOffset = bs.Position;
            bs.WriteString("SHDS", StringCoding.Raw);
            bs.WriteInt32(0);
            bs.WriteInt32((int)baseShdsOffset);
            bs.WriteInt32(0); // Empty

            // Skip everything, we'll be writing them way later on
            bs.Position = baseShdsOffset + 0x40;

            // Align main header
            bs.Align(0x40, grow: true);

            // Those are writen right after the main header, data is not here though
            bs.Position += Shaders.Programs0x40.Count * 0x20;
        }

        private void WriteModels(BinaryStream bs, long baseModelSetOffset)
        {
            long baseMdlPos = bs.Position;
            for (int i = 0; i < Models.Count; i++)
            {
                bs.Position = baseMdlPos + (i * 0x30);

                ModelSet3Model? model = Models[i];
                bs.WriteSingle(model.Unk);
                bs.WriteSingle(model.Origin.X); bs.WriteSingle(model.Origin.Y); bs.WriteSingle(model.Origin.Z);
                bs.WriteByte(0);
                bs.WriteByte(0);
                bs.WriteInt16((short)model.Bounds.Count);
                bs.WriteInt32(0); // Bounds offset, write right after
                bs.WriteInt32(0); // Commands offset, write right after
                bs.WriteInt32(0); // Commands length, write right after
                bs.WriteInt32(model.InitInstance_VMInstructionPtr);
                bs.WriteInt32(model.OnUpdate_VMInstructionPtr);
                bs.WriteInt32(model.Unk_VMInstructionPtr);
                bs.WriteInt16(model.Unk_0x2C);
                bs.WriteInt16(model.Flags_0x2E);
            }

            // Write bboxes & commands
            bs.Position = baseMdlPos + (Models.Count * 0x30);
            long lastOffset = bs.Position;
            for (int i = 0; i < Models.Count; i++)
            {
                bs.Position = lastOffset;
                ModelSet3Model? model = Models[i];

                // Write BBox - (NOTE: BBox starts aligned on 0x10)
                bs.Align(0x10, grow: true);

                int bboxOffset = (int)bs.Position;
                for (var j = 0; j < model.Bounds.Count; j++)
                {
                    bs.WriteSingle(model.Bounds[j].X);
                    bs.WriteSingle(model.Bounds[j].Y);
                    bs.WriteSingle(model.Bounds[j].Z);
                }

                lastOffset = bs.Position;

                // Write offset to BBox in model pointers
                bs.Position = baseMdlPos + (i * 0x30) + 0x14;
                bs.WriteInt32((int)bboxOffset);

                // Write commands
                bs.Position = lastOffset;
                int commandsOffset = (int)bs.Position;
                foreach (var cmd in model.Commands)
                {
                    bs.WriteByte(cmd.Opcode);
                    cmd.Write(bs);
                }

                long commandsLength = bs.Position - lastOffset;
                bs.Align(0x08, grow: true);
                lastOffset = bs.Position;

                // Write offset to commands in model pointers
                bs.Position = baseMdlPos + (i * 0x30) + 0x18;
                bs.WriteInt32((int)commandsOffset);
                bs.WriteInt32((int)commandsLength);

                bs.Position = lastOffset;
            }

            bs.Position = baseModelSetOffset + 0x10;
            bs.WriteInt16((short)Models.Count);

            bs.Position = baseModelSetOffset + 0x30;
            bs.WriteInt32((int)baseMdlPos);

            bs.Position = lastOffset;
        }

        private void WriteModelKeys(BinaryStream bs, long baseModelSetOffset, OptimizedStringTable strTable)
        {
            long baseKeysPos = bs.Position;

            int i = 0;
            foreach (var key in ModelKeys)
            {
                bs.Position = baseKeysPos + (i * 0x08);
                bs.WriteInt32(0);
                bs.WriteUInt32(key.Value.ModelID);

                strTable.AddString(key.Value.Name);

                i++;
            }

            bs.Position = baseModelSetOffset + 0x12;
            bs.WriteInt16((short)ModelKeys.Count);

            bs.Position = baseModelSetOffset + 0x34;
            bs.WriteInt32((int)baseKeysPos);

            bs.Position = baseKeysPos + (ModelKeys.Count * 0x08);
        }

        private void WriteMeshes(BinaryStream bs, long baseModelSetOffset)
        {
            bs.Align(0x10, grow: true); // Start is aligned to 0x10

            long baseMdlPos = bs.Position;
            for (int i = 0; i < Meshes.Count; i++)
            {
                bs.Position = baseMdlPos + (i * 0x30);

                MDL3Mesh mesh = Meshes[i];
                bs.WriteUInt16(mesh.Flags);
                bs.WriteInt16(mesh.FVFIndex);
                bs.WriteInt16(mesh.MaterialIndex);
                bs.WriteByte(0);
                bs.WriteByte(0);
                bs.WriteUInt32(mesh.VertexCount);
                bs.WriteInt32(0); // Vert offset write later
                bs.WriteInt32(0);
                bs.WriteUInt32(mesh.TriLength);
                bs.WriteInt32(0);
                bs.WriteInt32(0); // Unknown offset
                bs.WriteInt32(0); // BBox write later
                bs.WriteInt32(0); // PMSH ref offset write later
            }

            // Write bboxes & pmsh ref
            bs.Position = baseMdlPos + (Meshes.Count * 0x30);
            long lastOffset = bs.Position;
            for (int i = 0; i < Meshes.Count; i++)
            {
                bs.Position = lastOffset;
                MDL3Mesh mesh = Meshes[i];

                // Write BBox - (NOTE: BBox starts aligned on 0x10)
                bs.Align(0x10, grow: true);

                if (mesh.BBox != null)
                {
                    int bboxOffset = (int)bs.Position;
                    for (var j = 0; j < mesh.BBox.Length; j++)
                    {
                        bs.WriteSingle(mesh.BBox[j].X);
                        bs.WriteSingle(mesh.BBox[j].Y);
                        bs.WriteSingle(mesh.BBox[j].Z);
                    }

                    lastOffset = bs.Position;

                    // Write offset to BBox in mesh pointers
                    bs.Position = baseMdlPos + (i * 0x30) + 0x28;
                    bs.WriteInt32((int)bboxOffset);
                }

                // Write unknown data
                bs.Position = lastOffset;

                if (mesh.Unk != null)
                {
                    int unkOffset = (int)bs.Position;
                    bs.WriteSingles(mesh.Unk.Values);
                    bs.WriteInt32(0); // TODO: offset to unknown
                    bs.WriteInt32(mesh.Unk.PMSHEntryIndex);
                    bs.Align(0x10, grow: true);

                    lastOffset = bs.Position;

                    // Write offset to commands in model pointers
                    bs.Position = baseMdlPos + (i * 0x30) + 0x2C;
                    bs.WriteInt32((int)unkOffset);
                }

                bs.Position = lastOffset;
            }

            bs.Position = baseModelSetOffset + 0x14;
            bs.WriteInt16((short)Meshes.Count);

            bs.Position = baseModelSetOffset + 0x38;
            bs.WriteInt32((int)baseMdlPos);

            bs.Position = lastOffset;
        }

        private void WriteMeshKeys(BinaryStream bs, long baseModelSetOffset, OptimizedStringTable strTable)
        {
            long baseKeysPos = bs.Position;

            int i = 0;
            foreach (var key in MeshKeys)
            {
                bs.Position = baseKeysPos + (i * 0x08);
                bs.WriteInt32(0);
                bs.WriteUInt32(key.Value.MeshID);

                strTable.AddString(key.Value.Name);

                i++;
            }

            bs.Position = baseModelSetOffset + 0x16;
            bs.WriteInt16((short)MeshKeys.Count);

            bs.Position = baseModelSetOffset + 0x3C;
            bs.WriteInt32((int)baseKeysPos);

            bs.Position = baseKeysPos + (MeshKeys.Count * 0x08);
        }

        private void WriteFlexibleVertices(BinaryStream bs, long baseModelSetOffset, OptimizedStringTable strTable)
        {
            long baseFVFPos = bs.Position;
            for (int i = 0; i < FlexibleVertexFormats.Count; i++)
            {
                bs.Position = baseFVFPos + (i * 0x78);

                MDL3FlexibleVertexDefinition def = FlexibleVertexFormats[i];
                strTable.AddString(def.Name);

                bs.WriteInt32(0); // Name offset write later
                bs.Write(def.Unk0x04);
                bs.WriteInt32(0); // Fields offset write later
                bs.WriteInt16(0);
                bs.WriteInt16(0);
                bs.WriteInt32(0);
                bs.WriteInt32(0); // Unk offset 0x14
                bs.WriteByte((byte)def.Elements.Count);
                bs.WriteByte(def.VertexSize);
                bs.WriteInt16(0); // Field used at runtime
                bs.Position += 22 * sizeof(int);
                bs.WriteInt32(0); // FVF Array offset write later
            }

            
            bs.Align(0x10, grow: true);
            long lastOffset = bs.Position;

            for (int i = 0; i < FlexibleVertexFormats.Count; i++)
            {
                MDL3FlexibleVertexDefinition def = FlexibleVertexFormats[i];

                int j = 0;
                int elementsOffset = (int)bs.Position;

                foreach (var element in def.Elements)
                {
                    bs.Position = lastOffset + (j * 0x08);
                    strTable.AddString(element.Key);

                    bs.WriteInt32(0); // String offset to be written later
                    bs.WriteByte(element.Value.StartOffset);
                    bs.WriteByte(element.Value.ElementCount);
                    bs.WriteByte((byte)element.Value.FieldType);
                    bs.WriteByte((byte)element.Value.ArrayIndex);

                    lastOffset = bs.Position;
                }

                // Write offset to elements in fvf struct
                bs.Position = baseFVFPos + (i * 0x78) + 0x08;
                bs.WriteInt32((int)elementsOffset);

                if (def.ArrayDefinition != null)
                {
                    bs.Position = lastOffset;
                    bs.Align(0x10, grow: true);

                    long arrayDefOffset = bs.Position;
                    bs.WriteUInt32(def.ArrayDefinition.DataOffset);
                    bs.WriteUInt32(def.ArrayDefinition.ArrayLength);
                    bs.WriteUInt32(def.ArrayDefinition.ArrayElementSize);
                    bs.WriteByte(def.ArrayDefinition.VertexSize);
                    bs.WriteByte(def.ArrayDefinition.VertexSizeDefault);
                    bs.Position += 2; // Pad

                    lastOffset = bs.Position;

                    // Write offset
                    bs.Position = baseFVFPos + (i * 0x78) + 0x74;
                    bs.WriteInt32((int)arrayDefOffset);
                }
            }

            bs.Position = baseModelSetOffset + 0x18;
            bs.WriteInt16((short)FlexibleVertexFormats.Count);

            bs.Position = baseModelSetOffset + 0x40;
            bs.WriteInt32(FlexibleVertexFormats.Count > 0 ? (int)baseFVFPos : 0);

            bs.Position = lastOffset;
        }

        private void WriteMaterials(BinaryStream bs, long baseModelSetOffset, OptimizedStringTable strTable)
        {
            long baseMaterialDefPos = bs.Position;

            // Write Material Infos
            long lastOffset = bs.Position;
            for (int i = 0; i < Materials.Materials.Count; i++)
            {
                MDL3Material? def = Materials.Materials[i];
                strTable.AddString(def.Name);

                bs.Position = baseMaterialDefPos + (i * 0x34);
                bs.WriteInt32(0); // Name offset write later
                bs.WriteInt16(def.MaterialDataID);
                bs.WriteInt16(def.CellGcmParamsID);
                bs.WriteUInt16(def.Flags);
                bs.WriteInt16((short)def.ImageEntries.Count);
                bs.WriteInt32(0); // Keys offset write later
                bs.Position += 0x24; // Unused in file

                lastOffset = bs.Position;
            }

           
            bs.Position = 0xE4;
            bs.WriteInt16((short)Materials.Materials.Count);

            bs.Position = 0xE4 + 0x08;
            bs.WriteInt32(Materials.Materials.Count > 0 ? (int)baseMaterialDefPos : 0);

            // Data (0x0C) entries
            bs.Position = lastOffset;
            int dataOffset = (int)lastOffset;
            for (int i = 0; i < Materials.MaterialDatas.Count; i++)
            {
                MDL3MaterialData? entry = Materials.MaterialDatas[i];
                strTable.AddString(entry.Name);

                bs.WriteInt32(0); // Name offset write later
                bs.WriteInt32(entry.UnkIndex);
                bs.WriteByte(entry.Unk0x01);
                bs.WriteByte(entry.Version);
                bs.WriteInt16((short)entry.Keys.Count);

                // Skip offsets that we cannot write yet
                bs.WriteInt32(0);
                bs.WriteInt32(0);
                bs.WriteInt32(0);
                bs.WriteInt32(0);
                bs.WriteInt32(0);
                bs.WriteInt16(entry.Unk0x20);
                bs.WriteInt16(0); // Count we don't know of yet
                bs.WriteInt32(0); // Offset skip

                lastOffset = bs.Position;
            }

           
            bs.Position = 0xE4 + 0x02;
            bs.WriteInt16((short)Materials.MaterialDatas.Count);

            bs.Position = 0xE4 + 0x0C;
            bs.WriteInt32(Materials.MaterialDatas.Count > 0 ? (int)dataOffset : 0);

            bs.Position = lastOffset;

            // Sub data entries
            for (int i = 0; i < Materials.MaterialDatas.Count; i++)
            {
                MDL3MaterialData entry = Materials.MaterialDatas[i];

                long tocPos = bs.Position;

                // Write Data->0x18 entries
                lastOffset = (int)(tocPos + (entry._0x18.Count * 0x08));
                if (entry._0x18.Count > 0)
                {
                    for (int j = 0; j < entry._0x18.Count; j++)
                    {
                        MDL3MaterialData_0x18 entry_ = entry._0x18[j];

                        int lastEntryOffset = (int)bs.Position;
                        bs.Position = lastOffset;
                        bs.WriteInt16s(entry_.Data);
                        bs.Align(0x08, grow: true);
                        lastOffset = bs.Position;

                        bs.Position = tocPos + (j * 0x08);
                        bs.WriteInt32(entry_.Data.Length);
                        bs.WriteInt32(entry_.Data.Length > 0 ? lastEntryOffset : 0);
                    }

                    bs.Position = dataOffset + (i * 0x28) + 0x18;
                    bs.WriteInt32((int)tocPos);
                }

                // Write Data->0x1C entry
                if (entry._0x1C != null)
                {
                    bs.Position = lastOffset;
                    int entry0x0COffset = (int)bs.Position;

                    bs.WriteInt32(entry._0x1C.Unk);
                    bs.WriteInt32(entry._0x1C.Unk2);
                    lastOffset = bs.Position;

                    bs.Position = dataOffset + (i * 0x28) + 0x1C;
                    bs.WriteInt32(entry0x0COffset);
                }

                bs.Position = lastOffset;
            }

            
            bs.Position = lastOffset;
            int gcmParamsOffset = (int)bs.Position;

            // Write Gcm Params (0x10) entries
            for (int i = 0; i < Materials.GcmParams.Count; i++)
            {
                CellGcmParams param = Materials.GcmParams[i];
                bs.WriteInt32s(param.Params);

                lastOffset = bs.Position;
            }

            bs.Position = 0xE4 + 0x05;
            bs.WriteByte((byte)Materials.GcmParams.Count);

            bs.Position = 0xE4 + 0x10;
            bs.WriteInt32(Materials.GcmParams.Count > 0 ? (int)gcmParamsOffset : 0);

            bs.Position = lastOffset;

            // Write pglu texture infos
            int textureInfosOffset = (int)bs.Position;
            for (int i = 0; i < Materials.TextureInfos.Count; i++)
            {
                PGLUTextureInfo textureInfo = Materials.TextureInfos[i];
                textureInfo.Write(bs);
                lastOffset = bs.Position;
            }

            bs.Position = 0xE4 + 0x06;
            bs.WriteInt16((byte)Materials.TextureInfos.Count);

            bs.Position = 0xE4 + 0x10;
            bs.WriteInt32(Materials.TextureInfos.Count > 0 ? (int)textureInfosOffset : 0);

            bs.Position = lastOffset;
        }

        private void LinkAll()
        {
            foreach (var modelKey in ModelKeys)
                Models[(ushort)modelKey.Value.ModelID].Key = modelKey.Value;

            foreach (var meshKey in MeshKeys)
                Meshes[(ushort)meshKey.Value.MeshID].Key = meshKey.Value;

            foreach (var mesh in Meshes)
            {
                if (mesh.FVFIndex != -1)
                    mesh.FVF = FlexibleVertexFormats[mesh.FVFIndex];

                if (mesh.MaterialIndex != -1)
                    mesh.Material = Materials.Materials[mesh.MaterialIndex];
            }
        }

        private void ReadModels(BinaryStream bs, long baseMdlPos, uint offset, uint count)
        {
            for (ushort i = 0; i < count; i++)
            {
                bs.Position = baseMdlPos + offset + i * 0x30;
                Models.Add(ModelSet3Model.FromStream(bs, baseMdlPos, VersionMajor));
            }
        }

        private void ReadModelKeys(BinaryStream bs, long baseMdlPos, uint offset, uint count)
        {
            for (ushort i = 0; i < count; i++)
            {
                bs.Position = baseMdlPos + offset + i * 0x08;

                var key = MDL3ModelKey.FromStream(bs, baseMdlPos, VersionMajor);
                ModelKeys.Add(key.ModelID, key);
            }
        }

        private void ReadMeshes(BinaryStream bs, long baseMdlPos, uint offset, uint count)
        {
            for (ushort i = 0; i < count; i++)
            {
                bs.Position = baseMdlPos + offset + i * 0x30;
                Meshes.Add(MDL3Mesh.FromStream(bs, baseMdlPos, VersionMajor));
            }
        }

        private void ReadMeshKeys(BinaryStream bs, long baseMdlPos, uint offset, uint count)
        {
            for (ushort i = 0; i < count; i++)
            {
                bs.Position = baseMdlPos + offset + i * 0x08;

                var key = MDL3MeshKey.FromStream(bs, baseMdlPos, VersionMajor);
                MeshKeys.Add(key.MeshID, key);

                Meshes[(ushort)key.MeshID].Key = key;
            }
        }

        private void ReadFlexibleVertices(BinaryStream bs, long baseMdlPos, uint offset, uint count)
        {
            for (ushort i = 0; i < count; i++)
            {
                bs.Position = baseMdlPos + offset + i * 0x78;
                FlexibleVertexFormats.Add(MDL3FlexibleVertexDefinition.FromStream(bs, baseMdlPos, VersionMajor));
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

                var hostMethodEntry = VMHostMethodEntry.FromStream(bs, baseMdlPos, VersionMajor);
                VMHostMethodEntries.Add(hostMethodEntry.StorageID, hostMethodEntry);
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
            Materials = MDL3Materials.FromStream(bs, baseMdlPos, VersionMajor);
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
    }
}

