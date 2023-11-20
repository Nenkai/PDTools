using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

using Syroot.BinaryData;

using PDTools.Files.Textures.PS2;
using PDTools.Files.Models.PS2.Commands;
using PDTools.Files.Models.VM;

namespace PDTools.Files.Models.PS2.ModelSet
{
    /// <summary>
    /// Model Set 2. Used by GT4
    /// </summary>
    public class ModelSet2 : ModelSetPS2Base
    {
        /// <summary>
        /// "MDLS"
        /// </summary>
        public const uint MAGIC = 0x534C444D;

        public RelocatorBase Relocator { get; set; }

        public byte InstanceFlags { get; set; }
        public ushort InstanceOutRegisterCount { get; set; }
        public ushort InstanceUnkRegisterCount { get; set; }
        public ushort HostMethodInfoCount { get; set; }

        public List<ModelSet2Model> Models { get; set; } = new List<ModelSet2Model>();
        public List<PGLUshape> Shapes { get; set; } = new List<PGLUshape>();
        public List<PGLUmaterial> Materials { get; set; } = new List<PGLUmaterial>();

        public List<RegisterInfo> OutRegisterInfo { get; set; } = new List<RegisterInfo>();
        public List<RegisterInfo> HostMethodInfo { get; set; } = new List<RegisterInfo>();

        /// <summary>
        /// Textures. Each texture set within a list represents seemingly one lod level.
        /// </summary>
        public List<List<TextureSet1>> TextureSetLists { get; set; } = new List<List<TextureSet1>>();

        public void FromStream(Stream stream)
        {
            using var bs = new BinaryStream(stream);
            long basePos = bs.Position;

            uint magic = bs.ReadUInt32();
            if (magic != MAGIC)
                throw new InvalidDataException("Not a valid ModelSet2 stream.");

            /* HEADER - 0xE4 */
            int relocatorInfoOffset = bs.ReadInt32();
            int relocatorDataSize = bs.ReadInt32();
            int relocationBase = bs.ReadInt32();
            int fileSize = bs.ReadInt32();

            byte unk = bs.Read1Byte();
            InstanceFlags = bs.Read1Byte();
            ushort modelCount = bs.ReadUInt16();
            ushort shapeCount = bs.ReadUInt16();
            ushort materialCount = bs.ReadUInt16();
            ushort texSetCount = bs.ReadUInt16();
            ushort colorCount = bs.ReadUInt16();
            InstanceOutRegisterCount = bs.ReadUInt16();
            InstanceUnkRegisterCount = bs.ReadUInt16();
            HostMethodInfoCount = bs.ReadUInt16();
            ushort externalInfoCount = bs.ReadUInt16();
            ushort outRegisterInfoCount = bs.ReadUInt16();
            ushort symbolsCount = bs.ReadUInt16();
            
            bs.Position = basePos + 0x38;
            uint modelsOffset = bs.ReadUInt32();
            uint shapesOffset = bs.ReadUInt32();
            uint materialsOffset = bs.ReadUInt32();
            uint pgluTexSetsOffset = bs.ReadUInt32();
            uint bindMatricesOffset = bs.ReadUInt32();
            uint hostMethodInfosOffset = bs.ReadUInt32();
            uint externalInfoOffset = bs.ReadUInt32();
            uint outRegisterInfoOffset = bs.ReadUInt32();
            uint symbolsOffset = bs.ReadUInt32();
            uint vmBytecodeOffset = bs.ReadUInt32();

            bs.Position = basePos + shapesOffset;

            bs.Position = basePos + relocatorInfoOffset;
            Relocator = RelocatorBase.FromStream(bs, basePos);

            ReadMaterials(bs, basePos, materialsOffset, materialCount);
            ReadModels(bs, basePos, modelsOffset, modelCount);
            ReadShapes(bs, basePos, shapesOffset, shapeCount);
            ReadTextureSets(bs, basePos, pgluTexSetsOffset, colorCount, texSetCount);
            ReadHostMethodInfos(bs, basePos, hostMethodInfosOffset, HostMethodInfoCount);
            ReadOutRegisterInfo(bs, basePos, outRegisterInfoOffset, outRegisterInfoCount);

            Instance instance = new Instance();
            instance.ModelSet = this;

            if ((InstanceFlags & 1) != 0)
            {
                bs.Position = basePos + 0x7C;
                uint instanceOffset = bs.ReadUInt32();
                bs.Position = instanceOffset;
                instance.Read(bs, basePos);
            }
            else
            {
                instance.OutputRegisters = new RegisterVal[InstanceOutRegisterCount];
                instance.Unk2 = new RegisterVal[InstanceUnkRegisterCount];
                instance.HostMethodRegisters = new RegisterVal[HostMethodInfoCount];
            }

            bs.Position = basePos + vmBytecodeOffset;
            var bytecode = bs.ReadBytes(0x500);
            var vm = new VMContext(instance);
            vm.callVM(bytecode, 0, null);
        }

        private void ReadMaterials(BinaryStream bs, long baseMdlPos, uint offset, uint count)
        {
            for (ushort i = 0; i < count; i++)
            {
                bs.Position = baseMdlPos + offset + i * PGLUmaterial.GetSize();

                var material = new PGLUmaterial();
                material.FromStream(bs, baseMdlPos);
                Materials.Add(material);
            }
        }

        private void ReadModels(BinaryStream bs, long baseMdlPos, uint offset, uint count)
        {
            for (ushort i = 0; i < count; i++)
            {
                bs.Position = baseMdlPos + offset + i * ModelSet2Model.GetSize();

                var model = new ModelSet2Model();
                model.FromStream(bs, baseMdlPos);
                Models.Add(model);
            }
        }

        private void ReadShapes(BinaryStream bs, long baseMdlPos, uint offset, uint count)
        {
            for (ushort i = 0; i < count; i++)
            {
                bs.Position = baseMdlPos + offset + i * sizeof(int);
                uint shapeOffset = bs.ReadUInt32();

                bs.Position = baseMdlPos + shapeOffset;

                var shape = new PGLUshape();
                shape.FromStream(bs, baseMdlPos);
                Shapes.Add(shape);
            }
        }

        private void ReadHostMethodInfos(BinaryStream bs, long baseMdlPos, uint offset, uint count)
        {
            for (ushort i = 0; i < count; i++)
            {
                bs.Position = baseMdlPos + offset + i * RegisterInfo.GetSize();

                RegisterInfo registerInfo = new RegisterInfo();
                registerInfo.FromStream(bs, baseMdlPos);
                HostMethodInfo.Add(registerInfo);
            }
        }

        private void ReadOutRegisterInfo(BinaryStream bs, long baseMdlPos, uint offset, uint count)
        {
            for (ushort i = 0; i < count; i++)
            {
                bs.Position = baseMdlPos + offset + i * RegisterInfo.GetSize();

                RegisterInfo registerInfo = new RegisterInfo();
                registerInfo.FromStream(bs, baseMdlPos);
                OutRegisterInfo.Add(registerInfo);
            }
        }

        private void ReadTextureSets(BinaryStream bs, long baseMdlPos, uint offset, uint listCount, uint textureLodLevels)
        {
            for (int i = 0; i < listCount; i++)
            {
                bs.Position = baseMdlPos + offset + i * sizeof(int);
                int entriesOffset = bs.ReadInt32();

                // One texture set per lod level
                List<TextureSet1> list = new List<TextureSet1>();
                for (int j = 0; j < textureLodLevels; j++)
                {
                    bs.Position = baseMdlPos + entriesOffset + j * sizeof(int);

                    int off = bs.ReadInt32();
                    bs.Position = baseMdlPos + off;

                    TextureSet1 textureSet = new TextureSet1();
                    textureSet.FromStream(bs);
                    list.Add(textureSet);
                }

                TextureSetLists.Add(list);
            }
        }

        public void DumpModel(int modelIndex, string outdir)
        {
            ModelCommandContext context = new ModelCommandContext();
            context.ModelIndex = modelIndex;
            context.OutputDir = outdir;

            ModelSet2Model model = Models[modelIndex];

            ProcessCommands(context, model.Commands);
        }

        private void ProcessCommands(ModelCommandContext context, List<ModelSetupPS2Command> cmds)
        {
            foreach (ModelSetupPS2Command command in cmds)
            {
                switch (command.Opcode)
                {
                    case ModelSetupPS2Opcode.BBoxRender:
                        ProcessCommands(context, (command as Cmd_BBoxRender).CommandsOnRender);
                        context.FinishModel();
                        break;

                    case ModelSetupPS2Opcode.LODSelect:
                        var lodSel = command as Cmd_LODSelect;
                        for (int i = 0; i < lodSel.CommandsPerLOD.Count; i++)
                        {
                            context.SetLOD(i);

                            ProcessCommands(context, lodSel.CommandsPerLOD[i]);

                            context.FinishModel();
                        }
                        break;

                    case ModelSetupPS2Opcode.CallModelCallback:
                        var callbackCmd = command as Cmd_CallModelCallback;
                        if (callbackCmd.Parameter == 0) // Tail Lamp
                        {
                            ProcessCommands(context, callbackCmd.Default);

                            context.ExtraName = "tail_lamp_off";
                            ProcessCommands(context, callbackCmd.CommandsPerBranch[0]);

                            context.ExtraName = "tail_lamp_on";
                            ProcessCommands(context, callbackCmd.CommandsPerBranch[1]);

                            context.ExtraName = null;
                        }

                        break;

                    case ModelSetupPS2Opcode.pgluSetTexTable_Byte:
                        byte index = (command as Cmd_pgluSetTexTable_Byte).TexSetTableIndex;
                        context.SetTexTable(index);
                        break;

                    case ModelSetupPS2Opcode.pgluCallShape_Byte:

                        if (context.ObjWriter is null)
                        {
                            if (context.LOD == -1)
                            {
                                context.SetupObjWriter($"model{context.ModelIndex}");
                                context.ObjWriter.WriteLine($"mtllib model{context.ModelIndex}.mtl");
                            }
                            else
                            {
                                context.SetupObjWriter($"model{context.ModelIndex}.lod{context.LOD}");
                                context.ObjWriter.WriteLine($"mtllib model{context.ModelIndex}.lod{context.LOD}.mtl");
                            }

                        }

                        var callShape = (command as Cmd_pgluCallShapeByte);
                        int shapeIndex = callShape.ShapeIndex;
                        PGLUshapeConverted shapeData = Shapes[shapeIndex].GetShapeData();

                        string name = $"shape{shapeIndex}";
                        if (!string.IsNullOrEmpty(context.ExtraName))
                            name += $"_{context.ExtraName}";

                        context.DumpShapeToObj(shapeData, shapeIndex, name);
                        break;
                }
            }
        }

        public override List<TextureSet1> GetTextureSetList()
        {
            throw new NotImplementedException();
        }

        public override int AddShape(PGLUshape shape)
        {
            throw new NotImplementedException();
        }

        public override int AddMaterial(PGLUmaterial material)
        {
            throw new NotImplementedException();
        }

        public override int GetMaterialCount()
        {
            throw new NotImplementedException();
        }

        public class Instance
        {
            public ModelSet2 ModelSet { get; set; }

            /// <summary>
            /// Info about each register provided in model set out register infos
            /// </summary>
            public RegisterVal[] OutputRegisters { get; set; }

            /// <summary>
            /// Maybe external?
            /// </summary>
            public RegisterVal[] Unk2 { get; set; }

            /// <summary>
            /// Engine provided from the specified model set host method infos
            /// </summary>
            public RegisterVal[] HostMethodRegisters { get; set; }

            public void Read(BinaryStream bs, long baseMdlPos)
            {
                int parentModelSetPtr = bs.ReadInt32();
                int outRegistersPtr = bs.ReadInt32();
                int unkRegistersPtr = bs.ReadInt32();
                int hostMethodRegistersPtr = bs.ReadInt32();
                int hostMethodInfosFuncs = bs.ReadInt32();
                // The rest we don't need

                bs.Position = baseMdlPos + outRegistersPtr;
                OutputRegisters = new RegisterVal[ModelSet.InstanceOutRegisterCount];
                for (int i = 0; i < ModelSet.InstanceOutRegisterCount; i++)
                    OutputRegisters[i] = new RegisterVal(bs.ReadInt32());

                bs.Position = baseMdlPos + unkRegistersPtr;
                Unk2 = new RegisterVal[ModelSet.InstanceUnkRegisterCount];
                for (int i = 0; i < ModelSet.InstanceUnkRegisterCount; i++)
                    Unk2[i] = new RegisterVal(bs.ReadInt32());

                bs.Position = baseMdlPos + hostMethodRegistersPtr;
                HostMethodRegisters = new RegisterVal[ModelSet.HostMethodInfoCount];
                for (int i = 0; i < ModelSet.HostMethodInfoCount; i++)
                    HostMethodRegisters[i] = new RegisterVal(bs.ReadInt32());
            }
        }
    }
}
