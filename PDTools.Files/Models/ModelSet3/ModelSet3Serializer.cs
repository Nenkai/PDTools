using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using PDTools.Utils;
using PDTools.Files.Models.ModelSet3.FVF;
using PDTools.Files.Models.ModelSet3.Materials;
using PDTools.Files.Models.ModelSet3.Meshes;
using PDTools.Files.Models.Shaders;
using PDTools.Files.Textures;

using Syroot.BinaryData;
using System.Security.Cryptography.X509Certificates;
using System.Diagnostics;
using PDTools.Files.Models.ModelSet3.Wing;

namespace PDTools.Files.Models.ModelSet3
{
    public class ModelSet3Serializer
    {
        private ModelSet3 ModelSet { get; set; }

        // Both of these are to keep track of where the programs are - they're written in the shaders header first
        // Materials refer back to it for which shader program to use, easiest way to point back is to keep where they were written
        private Dictionary<ShadersProgram_0x20, int> _shaderProgram0x20Offsets = new();
        private Dictionary<ShadersProgram_0x2C, int> _shaderProgram0x2COffsets = new();

        public List<MDL3ModelKey> _modelKeys { get; set; } = new();
        public List<MDL3MeshKey> _meshKeys { get; set; } = new();
        public List<MDL3WingKey> _wingKeys { get; set; } = new();

        public ModelSet3Serializer(ModelSet3 modelSet)
        {
            ModelSet = modelSet;
        }

        public void Write(BinaryStream bs)
        {
            long basePos = bs.Position;

            // Seek to start of data
            bs.Position = ModelSet3.HeaderSize;

            WriteMaterialsTOC(bs, basePos);

            long shadersHeaderOffset = bs.Position;
            WriteShaderHeader(bs, basePos);

            // WRITE TODO: WRITE PMSH HEADER

            bs.Position = 0x200;

            WriteModels(bs, basePos);
            WriteModelKeys(bs, basePos);
            WriteMeshes(bs, basePos);
            WriteMeshKeys(bs, basePos);

            OptimizedStringTable fvfNameTable = new OptimizedStringTable();
            WriteFlexibleVertices(bs, basePos, fvfNameTable);

            OptimizedStringTable materialsNameTable = new OptimizedStringTable();
            WriteMaterials(bs, basePos, materialsNameTable);

            ModelSet.TextureSet.WriteToStream(bs, (int)bs.Position, writeImageData: false);

            OptimizedStringTable shadersNameTable = new OptimizedStringTable();
            WriteShadersStructures(bs, basePos, shadersHeaderOffset, shadersNameTable);

            OptimizedStringTable bonesNameTable = new OptimizedStringTable();
            WriteBones(bs, basePos, bonesNameTable);

            WriteVMCommands(bs, basePos);

            AllocateVMInstanceSize(bs, basePos);

            WriteMaterialStructures2(bs, basePos);

            WriteMaterialKeys(bs, basePos);

            WriteCustomWingData(bs, basePos);

            WriteUnkVMData(bs, basePos);
        }

        private void WriteMaterialsTOC(BinaryStream bs, long baseModelSetOffset)
        {
            bs.WriteInt16((short)ModelSet.Materials.Definitions.Count);
            bs.WriteInt16(0); // Count, Write later
            bs.WriteByte(0); //  Count, Write later
            bs.WriteByte(0); //  Count, Write later
            bs.WriteInt16((short)ModelSet.Materials.TextureInfos.Count);

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
            bs.Position += ModelSet.Shaders.Programs0x40.Count * 0x20;
        }

        private void WriteModels(BinaryStream bs, long baseModelSetOffset)
        {
            long baseMdlPos = bs.Position;
            for (int i = 0; i < ModelSet.Models.Count; i++)
            {
                bs.Position = baseMdlPos + (i * 0x30);

                ModelSet3Model? model = ModelSet.Models[i];
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
            bs.Position = baseMdlPos + (ModelSet.Models.Count * 0x30);
            long lastOffset = bs.Position;
            for (int i = 0; i < ModelSet.Models.Count; i++)
            {
                bs.Position = lastOffset;
                ModelSet3Model? model = ModelSet.Models[i];

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
            bs.WriteInt16((short)ModelSet.Models.Count);

            bs.Position = baseModelSetOffset + 0x30;
            bs.WriteInt32((int)baseMdlPos);

            bs.Position = lastOffset;
        }

        private void WriteModelKeys(BinaryStream bs, long baseModelSetOffset)
        {
            for (int i = 0; i < ModelSet.Models.Count; i++)
            {
                ModelSet3Model? model = ModelSet.Models[i];
                var key = new MDL3ModelKey();
                key.Name = model.Name;
                key.ModelID = (uint)i;
                _modelKeys.Add(key);
            }

            _modelKeys.Sort(MDL3ModelKeyComparer.Default);

            int keysOffset = (int)bs.Position;
            long lastOffset = bs.Position;
            for (int i = 0; i < _modelKeys.Count; i++)
            {
                bs.WriteInt32(0); // Name offset, write later
                bs.WriteInt32((int)_modelKeys[i].ModelID);

                lastOffset = bs.Position;
            }

            bs.Position = baseModelSetOffset + 0x12;
            bs.WriteInt16((short)_modelKeys.Count);

            bs.Position = baseModelSetOffset + 0x34;
            bs.WriteInt32((int)keysOffset);

            bs.Position = lastOffset;
        }

        private void WriteMeshes(BinaryStream bs, long baseModelSetOffset)
        {
            bs.Align(0x10, grow: true); // Start is aligned to 0x10

            long baseMdlPos = bs.Position;
            for (int i = 0; i < ModelSet.Meshes.Count; i++)
            {
                bs.Position = baseMdlPos + (i * 0x30);

                MDL3Mesh mesh = ModelSet.Meshes[i];
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
            bs.Position = baseMdlPos + (ModelSet.Meshes.Count * 0x30);
            long lastOffset = bs.Position;
            for (int i = 0; i < ModelSet.Meshes.Count; i++)
            {
                bs.Position = lastOffset;
                MDL3Mesh mesh = ModelSet.Meshes[i];

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
            bs.WriteInt16((short)ModelSet.Meshes.Count);

            bs.Position = baseModelSetOffset + 0x38;
            bs.WriteInt32((int)baseMdlPos);

            bs.Position = lastOffset;
        }

        private void WriteMeshKeys(BinaryStream bs, long baseModelSetOffset)
        {
            for (int i = 0; i < ModelSet.Meshes.Count; i++)
            {
                MDL3Mesh? mesh = ModelSet.Meshes[i];
                var key = new MDL3MeshKey();
                key.Name = mesh.Name;
                key.MeshID = (uint)i;
                _meshKeys.Add(key);
            }

            _meshKeys.Sort(MDL3MeshKeyComparer.Default);

            int keysOffset = (int)bs.Position;
            long lastOffset = bs.Position;
            for (int i = 0; i < _meshKeys.Count; i++)
            {
                bs.WriteInt32(0); // Name offset, write later
                bs.WriteInt32((int)_meshKeys[i].MeshID);

                lastOffset = bs.Position;
            }

            bs.Position = baseModelSetOffset + 0x16;
            bs.WriteInt16((short)_meshKeys.Count);

            bs.Position = baseModelSetOffset + 0x3C;
            bs.WriteInt32((int)keysOffset);

            bs.Position = lastOffset;
        }

        private void WriteFlexibleVertices(BinaryStream bs, long baseModelSetOffset, OptimizedStringTable strTable)
        {
            long baseFVFPos = bs.Position;
            for (int i = 0; i < ModelSet.FlexibleVertexFormats.Count; i++)
            {
                bs.Position = baseFVFPos + (i * 0x78);

                MDL3FlexibleVertexDefinition def = ModelSet.FlexibleVertexFormats[i];
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

            for (int i = 0; i < ModelSet.FlexibleVertexFormats.Count; i++)
            {
                MDL3FlexibleVertexDefinition def = ModelSet.FlexibleVertexFormats[i];

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
            bs.WriteInt16((short)ModelSet.FlexibleVertexFormats.Count);

            bs.Position = baseModelSetOffset + 0x40;
            bs.WriteInt32(ModelSet.FlexibleVertexFormats.Count > 0 ? (int)baseFVFPos : 0);

            bs.Position = lastOffset;
        }

        private void WriteMaterials(BinaryStream bs, long baseModelSetOffset, OptimizedStringTable strTable)
        {
            long baseMaterialDefPos = bs.Position;

            // Write Material Infos
            long lastOffset = bs.Position;
            for (int i = 0; i < ModelSet.Materials.Definitions.Count; i++)
            {
                MDL3Material? def = ModelSet.Materials.Definitions[i];
                strTable.AddString(def.Name);

                bs.Position = baseMaterialDefPos + (i * MDL3Material.GetSize());
                bs.WriteInt32(0); // Name offset write later
                bs.WriteInt16(def.MaterialDataID);
                bs.WriteInt16(def.CellGcmParamsID);
                bs.WriteUInt16(def.Flags);
                bs.WriteInt16((short)def.ImageEntries.Count);
                bs.WriteInt32(0); // Keys offset write later
                bs.Position += 0x24; // Unused in file

                lastOffset = bs.Position;
            }


            bs.Position = ModelSet3.HeaderSize;
            bs.WriteInt16((short)ModelSet.Materials.Definitions.Count);

            bs.Position = ModelSet3.HeaderSize + 0x08;
            bs.WriteInt32(ModelSet.Materials.Definitions.Count > 0 ? (int)baseMaterialDefPos : 0);

            // Data (0x0C) entries
            bs.Position = lastOffset;
            int dataOffset = (int)lastOffset;
            for (int i = 0; i < ModelSet.Materials.MaterialDatas.Count; i++)
            {
                MDL3MaterialData? entry = ModelSet.Materials.MaterialDatas[i];
                strTable.AddString(entry.Name);

                bs.WriteInt32(0); // Name offset write later
                bs.WriteInt32(entry.UnkIndex);
                bs.WriteByte(entry.Unk0x01);
                bs.WriteByte(entry.Version);
                bs.WriteInt16((short)entry.TextureKeys.Count);

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


            bs.Position = ModelSet3.HeaderSize + 0x02;
            bs.WriteInt16((short)ModelSet.Materials.MaterialDatas.Count);

            bs.Position = ModelSet3.HeaderSize + 0x0C;
            bs.WriteInt32(ModelSet.Materials.MaterialDatas.Count > 0 ? (int)dataOffset : 0);

            bs.Position = lastOffset;

            // Sub data entries
            for (int i = 0; i < ModelSet.Materials.MaterialDatas.Count; i++)
            {
                MDL3MaterialData entry = ModelSet.Materials.MaterialDatas[i];

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
            for (int i = 0; i < ModelSet.Materials.GcmParams.Count; i++)
            {
                CellGcmParams param = ModelSet.Materials.GcmParams[i];
                bs.WriteInt32s(param.Params);

                lastOffset = bs.Position;
            }

            bs.Position = ModelSet3.HeaderSize + 0x05;
            bs.WriteByte((byte)ModelSet.Materials.GcmParams.Count);

            bs.Position = ModelSet3.HeaderSize + 0x10;
            bs.WriteInt32(ModelSet.Materials.GcmParams.Count > 0 ? (int)gcmParamsOffset : 0);

            bs.Position = lastOffset;

            // Write pglu texture infos
            int textureInfosOffset = (int)bs.Position;
            for (int i = 0; i < ModelSet.Materials.TextureInfos.Count; i++)
            {
                PGLUTextureInfo textureInfo = ModelSet.Materials.TextureInfos[i];
                textureInfo.Write(bs);
                lastOffset = bs.Position;
            }

            bs.Position = ModelSet3.HeaderSize + 0x06;
            bs.WriteInt16((byte)ModelSet.Materials.TextureInfos.Count);

            bs.Position = ModelSet3.HeaderSize + 0x10;
            bs.WriteInt32(ModelSet.Materials.TextureInfos.Count > 0 ? (int)textureInfosOffset : 0);

            bs.Position = lastOffset;
        }

        private void WriteShadersStructures(BinaryStream bs, long baseModelSetOffset, long shadersHeaderOffset, OptimizedStringTable strTable)
        {
            long baseShaderDefPos = bs.Position;
            long lastOffset = bs.Position;

            // Write definitions
            for (var i = 0; i < ModelSet.Shaders.Definitions.Count; i++)
            {
                ShaderDefinition def = ModelSet.Shaders.Definitions[i];
                strTable.AddString(def.Name);

                bs.WriteInt32(0); // Name offset, write later
                bs.WriteInt32(def.UnkID);
                bs.WriteInt16(def.ProgramID);
                bs.WriteInt16(def.Unk0x24_Or_0x2C_EntryID);

                lastOffset = bs.Position;
            }

            bs.Position = baseModelSetOffset + shadersHeaderOffset + 0x16;
            bs.WriteInt16((short)ModelSet.Shaders.Definitions.Count);

            bs.Position = baseModelSetOffset + shadersHeaderOffset + 0x1C;
            bs.WriteInt32((int)baseShaderDefPos);

            bs.Position = lastOffset;
            int baseProgramsOffset = (int)bs.Position;

            // Write programs of offset 0x20
            for (var i = 0; i < ModelSet.Shaders.Programs0x20.Count; i++)
            {
                ShadersProgram_0x20 entry = ModelSet.Shaders.Programs0x20[i];
                _shaderProgram0x20Offsets.Add(entry, (int)bs.Position);

                bs.WriteInt32(0); // CgVertexProgramOffset, write later
                bs.WriteInt32(0); // CgVertexProgramSize, write later
                bs.WriteInt16(0); // Count 0x10
                bs.WriteInt16((short)entry._0x14.Count);
                bs.WriteInt16((short)entry._0x18.Count);
                bs.WriteInt16(0); // Empty
                bs.WriteInt32(0); // Offset 0x10
                bs.WriteInt32(0); // Offset 0x14, write later
                bs.WriteInt32(0); // Offset 0x18, write later
                bs.WriteInt32(0); // Offset 0x1C, write later
                bs.WriteInt32(entry.Unk); // Unk
                bs.WriteInt32(0); // Unknown

                lastOffset = bs.Position;
            }

            bs.Position = baseModelSetOffset + shadersHeaderOffset + 0x18;
            bs.WriteInt16((short)ModelSet.Shaders.Programs0x20.Count);

            bs.Position = baseModelSetOffset + shadersHeaderOffset + 0x20;
            bs.WriteInt32((int)baseProgramsOffset);

            bs.Position = lastOffset;
            bs.Align(0x10, grow: true);

            // Sub data entries
            for (int i = 0; i < ModelSet.Shaders.Programs0x20.Count; i++)
            {
                ShadersProgram_0x20 entry = ModelSet.Shaders.Programs0x20[i];

                // Write Data->0x14 entries
                if (entry._0x14.Count > 0)
                {
                    int entriesOffset = (int)bs.Position;
                    for (int j = 0; j < entry._0x14.Count; j++)
                    {
                        ShaderProgramEntry_0x20_0x14 entry_ = entry._0x14[j];

                        bs.WriteInt32(0); // Name offset write later
                        bs.WriteInt16(entry_.Unk);
                        bs.WriteInt16(entry_.Unk2);
                        bs.WriteInt16(entry_.Unk3);
                        bs.WriteInt16(entry_.Unk4);
                        bs.WriteInt32(entry_.Unk5);

                        lastOffset = bs.Position;
                    }

                    bs.Position = baseProgramsOffset + (i * ShadersProgram_0x20.GetSize()) + 0x0A;
                    bs.WriteInt16((short)entry._0x14.Count);

                    bs.Position = baseProgramsOffset + (i * ShadersProgram_0x20.GetSize()) + 0x14;
                    bs.WriteInt32(entriesOffset);
                }

                bs.Position = lastOffset;

                // Write Data->0x18 entries
                if (entry._0x18.Count > 0)
                {
                    int entriesOffset = (int)bs.Position;
                    for (int j = 0; j < entry._0x18.Count; j++)
                    {
                        ShaderProgramEntry_0x20_0x18 entry_ = entry._0x18[j];

                        bs.WriteInt32(0); // Name offset write later
                        bs.WriteInt16(entry_.Unk);
                        bs.WriteInt16(entry_.Unk2);
                        bs.WriteInt16(entry_.Unk3);
                        bs.WriteInt16(entry_.Unk4);
                        bs.WriteInt32(entry_.Unk5);

                        lastOffset = bs.Position;
                    }

                    bs.Position = baseProgramsOffset + (i * ShadersProgram_0x20.GetSize()) + 0x0C;
                    bs.WriteInt16((short)entry._0x18.Count);

                    bs.Position = baseProgramsOffset + (i * ShadersProgram_0x20.GetSize()) + 0x18;
                    bs.WriteInt32(entriesOffset);
                }

                bs.Position = lastOffset;
            }

            // Write 0x2C
            bs.Position = lastOffset;
            int base0x2COffset = (int)bs.Position;

            for (var i = 0; i < ModelSet.Shaders.Programs0x2C.Count; i++)
            {
                ShadersProgram_0x2C entry = ModelSet.Shaders.Programs0x2C[i];
                _shaderProgram0x2COffsets.Add(entry, (int)bs.Position);

                bs.WriteInt32(0); // CgVertexProgramOffset, write later
                bs.WriteInt32(0); // CgVertexProgramSize, write later
                bs.WriteInt16(0); // Count 0x10, write later
                bs.WriteByte(0); // Count 0x38, write later
                bs.WriteByte(0); // Count 0x14, write later
                bs.WriteInt16(0); // Count 0x18, write later
                bs.WriteInt16(entry.Unk); // Unk
                bs.WriteInt32(0); // Offset 0x10
                bs.WriteInt32(0); // Offset 0x14, write later
                bs.WriteInt32(0); // Offset 0x18, write later
                bs.WriteInt32(0); // Offset 0x1C, write later
                bs.Position += 6 * sizeof(int);
                bs.WriteInt32(0); // Offset 0x38, write later
                bs.WriteInt32(entry.Unk2); // Unknown
                bs.WriteInt32(0); // Unknown
                bs.WriteInt32(0); // Offset 0x44, write later

                lastOffset = bs.Position;
            }

            bs.Position = shadersHeaderOffset + 0x1A;
            bs.WriteInt16((short)ModelSet.Shaders.Programs0x2C.Count);

            bs.Position = shadersHeaderOffset + 0x2C;
            bs.WriteInt32((int)base0x2COffset);

            bs.Position = lastOffset;
            bs.Align(0x10, grow: true);

            // Sub data entries
            for (int i = 0; i < ModelSet.Shaders.Programs0x2C.Count; i++)
            {
                ShadersProgram_0x2C entry = ModelSet.Shaders.Programs0x2C[i];

                // Write Data->0x10 entries
                if (entry._0x10.Count > 0)
                {
                    int entriesOffset = (int)bs.Position;
                    for (int j = 0; j < entry._0x10.Count; j++)
                    {
                        ShaderProgramEntry_0x2C_0x10 entry_ = entry._0x10[j];

                        bs.WriteInt32(0); // Name offset write later
                        bs.WriteInt16(entry_.Unk);
                        bs.WriteInt16(entry_.Unk2);
                        bs.WriteInt16(entry_.Unk3);
                        bs.WriteInt16(entry_.Unk4);
                        bs.WriteInt32(entry_.Unk5);

                        lastOffset = bs.Position;
                    }

                    bs.Position = base0x2COffset + (i * ShadersProgram_0x2C.GetSize()) + 0x08;
                    bs.WriteInt16((short)entry._0x10.Count);

                    bs.Position = base0x2COffset + (i * ShadersProgram_0x2C.GetSize()) + 0x10;
                    bs.WriteInt32(entriesOffset);
                }

                bs.Position = lastOffset;

                // Write Data->0x14 entries
                if (entry._0x14.Count > 0)
                {
                    int entriesOffset = (int)bs.Position;
                    for (int j = 0; j < entry._0x14.Count; j++)
                    {
                        ShaderProgramEntry_0x2C_0x14 entry_ = entry._0x14[j];

                        bs.WriteInt32(0); // Name offset write later
                        bs.WriteInt16(entry_.Unk);
                        bs.WriteInt16(entry_.Unk2);
                        bs.WriteInt16(entry_.Unk3);
                        bs.WriteInt16(entry_.Unk4);
                        bs.WriteInt32(entry_.Unk5);

                        lastOffset = bs.Position;
                    }

                    bs.Position = base0x2COffset + (i * ShadersProgram_0x2C.GetSize()) + 0x0B;
                    bs.WriteByte((byte)entry._0x14.Count);

                    bs.Position = base0x2COffset + (i * ShadersProgram_0x2C.GetSize()) + 0x14;
                    bs.WriteInt32(entriesOffset);
                }

                bs.Position = lastOffset;

                // Write Data->0x18 entries
                if (entry._0x18.Count > 0)
                {
                    int entriesOffset = (int)bs.Position;
                    for (int j = 0; j < entry._0x38.Count; j++)
                    {
                        ShaderProgramEntry_0x2C_0x38 entry_ = entry._0x38[j];

                        bs.WriteInt32(0); // Name offset write later
                        bs.WriteInt16(entry_.Unk);
                        bs.WriteInt16(entry_.Unk2);
                        bs.WriteInt16(entry_.Unk3);
                        bs.WriteInt16(entry_.Unk4);
                        bs.WriteInt32(entry_.Unk5);

                        lastOffset = bs.Position;
                    }

                    bs.Position = base0x2COffset + (i * ShadersProgram_0x2C.GetSize()) + 0x0C;
                    bs.WriteInt16((short)entry._0x18.Count);

                    bs.Position = base0x2COffset + (i * ShadersProgram_0x2C.GetSize()) + 0x18;
                    bs.WriteInt32(entriesOffset);
                }

                bs.Position = lastOffset;

                // Write Data->0x18 entries
                if (entry._0x38.Count > 0)
                {
                    int entriesOffset = (int)bs.Position;
                    for (int j = 0; j < entry._0x18.Count; j++)
                    {
                        ShaderProgramEntry_0x2C_0x18 entry_ = entry._0x18[j];

                        bs.WriteInt32(0); // Name offset write later
                        bs.WriteInt16(entry_.Unk);
                        bs.WriteInt16(entry_.Unk2);
                        bs.WriteInt16(entry_.Unk3);
                        bs.WriteInt16(entry_.Unk4);
                        bs.WriteInt32(entry_.Unk5);

                        lastOffset = bs.Position;
                    }

                    bs.Position = base0x2COffset + (i * ShadersProgram_0x2C.GetSize()) + 0x0A;
                    bs.WriteByte((byte)entry._0x38.Count);

                    bs.Position = base0x2COffset + (i * ShadersProgram_0x2C.GetSize()) + 0x38;
                    bs.WriteInt32(entriesOffset);
                }

                bs.Position = lastOffset;
            }

            // Write 0x3C entries
            bs.Position = lastOffset;

            if (ModelSet.Shaders._0x3C.Count > 0)
            {
                int tocOffset = (int)bs.Position;
                lastOffset = bs.Position + (4 * ModelSet.Shaders._0x3C.Count);

                for (int i = 0; i < ModelSet.Shaders._0x3C.Count; i++)
                {
                    bs.Position = lastOffset;

                    int entryOffset = (int)bs.Position;

                    Shaders_0x3C entry = ModelSet.Shaders._0x3C[i];

                    bs.WriteInt32(0); // Name offset, write later
                    bs.WriteInt32(entry.Unk1);
                    bs.WriteInt32(entry.Unk2);
                    bs.WriteInt32(0); // Offset 0xC, write later
                    bs.WriteInt32(0);
                    bs.WriteInt32(0);
                    bs.WriteInt32(0);
                    bs.WriteInt32(0);
                    bs.WriteInt16(entry.Unk3);
                    bs.WriteInt16(entry.Unk4);
                    bs.WriteBytes(entry.Unk5);

                    int tmpLastOffset = (int)bs.Position;
                    bs.WriteByte(0);

                    bs.Position = lastOffset + 0x0C;
                    bs.WriteInt32(tmpLastOffset);

                    lastOffset = tmpLastOffset + 1;

                    bs.Position = tocOffset + (i * 4);
                    bs.WriteInt32(entryOffset);
                }
            }

            bs.Position = lastOffset;
        }

        private void WriteBones(BinaryStream bs, long baseModelSetOffset, OptimizedStringTable strTable)
        {
            int bonesOffset = (int)bs.Position;

            long lastOffset = bs.Position;
            for (var i = 0; i < ModelSet.Bones.Count; i++)
            {
                var bone = ModelSet.Bones[i];
                bs.WriteSingle(bone.Matrix.M11); bs.WriteSingle(bone.Matrix.M12); bs.WriteSingle(bone.Matrix.M13); bs.WriteSingle(bone.Matrix.M14);
                bs.WriteSingle(bone.Matrix.M21); bs.WriteSingle(bone.Matrix.M22); bs.WriteSingle(bone.Matrix.M23); bs.WriteSingle(bone.Matrix.M24);
                bs.WriteSingle(bone.Matrix.M31); bs.WriteSingle(bone.Matrix.M32); bs.WriteSingle(bone.Matrix.M33); bs.WriteSingle(bone.Matrix.M34);
                bs.WriteSingle(bone.Matrix.M41); bs.WriteSingle(bone.Matrix.M42); bs.WriteSingle(bone.Matrix.M43); bs.WriteSingle(bone.Matrix.M44);

                bs.WriteInt32(0); // name offset write later
                bs.WriteInt16(bone.ParentID);
                bs.WriteInt16(bone.ID);

                lastOffset = bs.Position;
            }

            bs.Position = baseModelSetOffset + 0x1A;
            bs.WriteInt16((short)ModelSet.Bones.Count);

            bs.Position = baseModelSetOffset + 0x50;
            bs.WriteInt32((short)ModelSet.Bones.Count > 0 ? bonesOffset : 0);

            bs.Position = lastOffset;
        }

        private void WriteVMCommands(BinaryStream bs, long baseModelSetOffset)
        {
            int vmCommandsOffset = (int)bs.Position;

            foreach (var cmd in ModelSet.VirtualMachine.Instructions)
            {
                bs.WriteByte((byte)cmd.Value.Opcode);
                cmd.Value.Write(bs);
            }

            int size = (int)(bs.Position - vmCommandsOffset);
            bs.WriteByte(0);

            long lastOffset = bs.Position;

            bs.Position = baseModelSetOffset + 0x60;
            bs.WriteInt32(size);
            bs.WriteInt32(vmCommandsOffset);

            bs.Position = lastOffset;
        }

        private void AllocateVMInstanceSize(BinaryStream bs, long baseModelSetOffset)
        {
            bs.Align(0x10, grow: true);

            // TODO

            bs.Position += 0x50;
        }

        private void WriteMaterialStructures2(BinaryStream bs, long baseModelSetOffset)
        {
            List<MDL3MaterialData_0x14> entries = ModelSet.Materials.MaterialDatas.Select(e => e._0x14).ToList();

            long lastOffset = bs.Position;

            // Get offset to material entries
            bs.Position = ModelSet3.HeaderSize + 0x0C;
            int materialsDataOffset = bs.ReadInt32();
            int offsetForHeader = (int)bs.Position;

            bs.Position = lastOffset;

            for (var i = 0; i < entries.Count; i++)
            {
                MDL3MaterialData_0x14 entry = entries[i];
                int entryOffset = (int)bs.Position;

                bs.WriteInt32(0); // Name offset, write later
                bs.WriteInt32(entry.UnkIndex);
                bs.WriteInt32(0);
                bs.WriteInt32(0); // Offset 0x0C, write later
                bs.WriteInt16(1);
                bs.WriteInt16(0);
                bs.WriteInt32(0); // Unk offset
                bs.WriteInt32(entry.Version);
                bs.WriteInt32(0); // Key offset
                bs.WriteInt32(0);
                bs.WriteInt32(0);

                lastOffset = bs.Position;

                bs.Position = materialsDataOffset + (i * MDL3MaterialData.GetSize()) + 0x14;
                bs.WriteInt32(entryOffset);

                bs.Position = lastOffset;
            }

            // Write main model header reference
            bs.Position = baseModelSetOffset + 0x2C;
            bs.WriteInt16((short)entries.Count);

            bs.Position = baseModelSetOffset + 0x88;
            bs.WriteInt32(offsetForHeader);

            bs.Position = lastOffset;

            // Write shader references
            for (int i = 0; i < ModelSet.Materials.MaterialDatas.Count; i++)
            {
                MDL3MaterialData? mat = ModelSet.Materials.MaterialDatas[i];
                var @ref = mat.ShaderReferences;
                int entryOffset = (int)bs.Position;

                bs.WriteInt32(@ref.UnkData != null ? 1 : 0);
                bs.WriteInt32(@ref.Unk);
                bs.WriteInt32(entryOffset + 0x20);

                int off0x2C = FindProgOffset2(@ref.ShaderProgram2);
                Debug.Assert(off0x2C != -1);
                bs.WriteInt32(off0x2C);

                int off0x20 = FindProgOffset(@ref.ShaderProgram);
                Debug.Assert(off0x20 != -1);
                bs.WriteInt32(off0x20);

                bs.WriteInt32(@ref.Unk2);
                bs.WriteInt16(@ref.Unk3);
                bs.WriteInt16(0);
                bs.WriteInt32(0);

                if (@ref.UnkData != null)
                {
                    bs.WriteInt32(@ref.UnkData.Length);
                    bs.WriteInt16s(@ref.UnkData);
                }

                lastOffset = bs.Position;

                // Write pointers to it, not just one

                bs.Position = materialsDataOffset + (i * MDL3MaterialData.GetSize()) + 0x24;
                bs.WriteInt32(entryOffset);

                bs.Position = materialsDataOffset + (i * MDL3MaterialData.GetSize()) + 0x14;
                int entry0x14Offset = bs.ReadInt32();

                bs.Position = entry0x14Offset + 0x0C;
                bs.WriteInt32(entryOffset);

                bs.Position = lastOffset;
            }

            bs.Position = lastOffset;

            int FindProgOffset(ShadersProgram_0x20 prog)
            {
                for (int i = 0; i < ModelSet.Shaders.Programs0x20.Count; i++)
                {
                    ShadersProgram_0x20? p = ModelSet.Shaders.Programs0x20[i];
                    if (p.Program.AsSpan().SequenceEqual(prog.Program))
                        return _shaderProgram0x20Offsets[p];
                }

                return -1;
            }

            int FindProgOffset2(ShadersProgram_0x2C prog)
            {
                for (int i = 0; i < ModelSet.Shaders.Programs0x20.Count; i++)
                {
                    ShadersProgram_0x2C? p = ModelSet.Shaders.Programs0x2C[i];
                    if (p.Program.AsSpan().SequenceEqual(prog.Program))
                        return _shaderProgram0x2COffsets[p];
                }

                return -1;
            }
        }

        private void WriteMaterialKeys(BinaryStream bs, long baseModelSetOffset)
        {
            long lastOffset = bs.Position;

            // Get offset to material entries
            bs.Position = ModelSet3.HeaderSize + 0x08;
            int materialsDefsOffset = bs.ReadInt32();
            int materialsDataOffset = bs.ReadInt32();

            bs.Position = lastOffset;

            int count = 0;
            int baseOffset = (int)bs.Position;

            List<(int Offset, MDL3TextureKey Key)?> writtenKeys = new List<(int, MDL3TextureKey)?>();

            // None of this is sorted for bsearch, it's fine to store as-is

            // Write texture keys from material definitions
            for (int i = 0; i < ModelSet.Materials.Definitions.Count; i++)
            {
                MDL3Material? mat = ModelSet.Materials.Definitions[i];
                int entriesOffset = (int)bs.Position;
                foreach (var key in mat.ImageEntries)
                {
                    int entryOffset = (int)bs.Position;

                    var k = writtenKeys.Find(e => e.Value.Key.Name == key.Name && e.Value.Key.TextureID == key.TextureID);
                    if (k is null)
                    {
                        bs.WriteInt32(0); // Name offset write later
                        bs.WriteUInt32(key.TextureID);

                        writtenKeys.Add((entryOffset, key));
                    }

                    lastOffset = bs.Position;
                }

                // Write pointer & count to it in defs
                bs.Position = materialsDefsOffset + (i * MDL3Material.GetSize()) + 0x0A;
                bs.WriteInt16((short)mat.ImageEntries.Count);
                bs.WriteInt32(mat.ImageEntries.Count > 0 ? entriesOffset : 0);
            }

            bs.Position = lastOffset;

            // Write texture keys from material datas
            for (int i = 0; i < ModelSet.Materials.MaterialDatas.Count; i++)
            {
                MDL3MaterialData? data = ModelSet.Materials.MaterialDatas[i];
                int entriesOffset = (int)bs.Position;
                foreach (var key in data.TextureKeys)
                {
                    int entryOffset = (int)bs.Position;

                    var k = writtenKeys.Find(e => e.Value.Key.Name == key.Name && e.Value.Key.TextureID == key.TextureID);
                    if (k is null)
                    {
                        bs.WriteInt32(0); // Name offset write later
                        bs.WriteUInt32(key.TextureID);

                        writtenKeys.Add((entryOffset, key));
                    }

                    lastOffset = bs.Position;
                }

                // Write pointer & count to it in defs
                bs.Position = materialsDataOffset + (i * MDL3MaterialData.GetSize()) + 0x0A;
                bs.WriteInt16((short)data.TextureKeys.Count);

                bs.Position = materialsDataOffset + (i * MDL3MaterialData.GetSize()) + 0x10;
                bs.WriteInt32(data.TextureKeys.Count > 0 ? entriesOffset : 0);

                if (data._0x14 != null)
                {
                    int offset0x14 = bs.ReadInt32();
                    bs.Position = offset0x14 + 0x18;
                    bs.WriteInt32((short)data.TextureKeys.Count);
                    bs.WriteInt32(data.TextureKeys.Count > 0 ? entriesOffset : 0);
                }
            }

            // Write in master header
            bs.Position = baseModelSetOffset + 0x8E;
            bs.WriteInt16((short)writtenKeys.Count);
            bs.WriteInt32(writtenKeys.Count > 0 ? baseOffset : 0);

            bs.Position = lastOffset;
        }

        private void WriteCustomWingData(BinaryStream bs, long baseModelSetOffset)
        {
            for (int i = 0; i < ModelSet.WingData.Count; i++)
            {
                MDL3WingData? wing = ModelSet.WingData[i];

                var key = new MDL3WingKey();
                key.Name = wing.Name;
                key.WingDataID = (uint)i;
                _wingKeys.Add(key);
            }

            // BSearch, important
            _wingKeys.Sort(MDL3WingKeyComparer.Default);

            int keysOffset = (int)bs.Position;
            long lastOffset = bs.Position;
            for (int i = 0; i < _wingKeys.Count; i++)
            {
                bs.WriteInt32(0); // Name offset, write later
                bs.WriteInt32((int)_wingKeys[i].WingDataID);

                lastOffset = bs.Position;
            }

            bs.Position = 0x9A;
            bs.WriteInt16((short)_wingKeys.Count);

            bs.Position = 0xA0;
            bs.WriteInt32(keysOffset);

            bs.Position = lastOffset;
        }

        private void WriteUnkVMData(BinaryStream bs, long baseModelSetOffset)
        {
            long dataPos = bs.Position + (ModelSet.UnkVMData.Count * MDL3ModelVMUnk.GetSize());
            long entriesPos = bs.Position;

            for (var i = 0; i < ModelSet.UnkVMData.Count; i++)
            {
                bs.Position = entriesPos + (i * MDL3ModelVMUnk.GetSize());
                bs.WriteInt32((int)dataPos);
                bs.Position += 0x2C;
                bs.WriteInt16((short)ModelSet.UnkVMData[i].UnkIndices.Length);

                bs.Position = dataPos;
                bs.WriteInt16s(ModelSet.UnkVMData[i].UnkIndices);

                dataPos = bs.Position;
            }

            // Write header pointer
            bs.Position = 0xB0;
            bs.WriteInt32((int)entriesPos);

            bs.Position = dataPos;
            bs.Align(0x10, grow: true);
        }
    }
}
