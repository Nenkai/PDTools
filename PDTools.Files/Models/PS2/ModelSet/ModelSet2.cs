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
using PDTools.Files.Textures;

namespace PDTools.Files.Models.PS2.ModelSet;

/// <summary>
/// Model Set 2. Used by GT4
/// </summary>
public class ModelSet2 : ModelSetPS2Base
{
    /// <summary>
    /// "MDLS"
    /// </summary>
    public const uint MAGIC = 0x534C444D;
    public const uint HeaderSize = 0x80;

    public RelocatorBase Relocator { get; set; }

    public byte InstanceFlags { get; set; }
    public ushort InstanceOutRegisterCount { get; set; }
    public ushort InstanceUnkRegisterCount { get; set; }
    public ushort HostMethodInfoCount { get; set; }

    public List<PGLUmaterial> Materials { get; set; } = [];
    public List<List<PGLUmaterial>> VariationMaterials { get; set; } = [];

    public List<RegisterInfo> OutRegisterInfos { get; set; } = [];
    public List<RegisterInfo> ExternalInfos { get; set; } = [];
    public List<RegisterInfo> HostMethodInfos { get; set; } = [];

    /// <summary>
    /// Textures. Each texture set within a list represents seemingly one lod level.
    /// </summary>
    public List<List<TextureSet1>> TextureSetLists { get; set; } = [];

    public void FromStream(Stream stream)
    {
        var bs = new BinaryStream(stream);
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

        // Counts
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
        byte variationMaterialCount = bs.Read1Byte();
        byte currentColorIndex = bs.Read1Byte();
        ushort bindMatrixCount = bs.ReadUInt16();
        bs.Position += 0x06;

        ushort instanceSize = bs.ReadUInt16();

        // Offsets
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
        uint variationMaterialsOffset = bs.ReadUInt32();

        bs.Position = basePos + shapesOffset;

        bs.Position = basePos + relocatorInfoOffset;
        Relocator = RelocatorBase.FromStream(bs, basePos);
        Relocator.MakeRelocatableGroups();

        ReadMaterials(bs, basePos, materialsOffset, materialCount);
        ReadVariationMaterials(bs, basePos, variationMaterialsOffset, variationMaterialCount);
        ReadModels(bs, basePos, modelsOffset, modelCount);
        ReadShapes(bs, basePos, shapesOffset, shapeCount);
        ReadTextureSets(bs, basePos, pgluTexSetsOffset, colorCount, texSetCount);
        ReadHostMethodInfos(bs, basePos, hostMethodInfosOffset, HostMethodInfoCount);
        ReadExternalInfos(bs, basePos, externalInfoOffset, externalInfoCount);
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

    private void ReadVariationMaterials(BinaryStream bs, long baseMdlPos, uint offset, uint count)
    {
        for (ushort i = 0; i < count; i++)
        {
            bs.Position = baseMdlPos + offset + (i * 4);

            uint materialsOffset = bs.ReadUInt32();
            bs.Position = baseMdlPos + materialsOffset;
            VariationMaterials.Add([]);

            for (int j = 0; j < Materials.Count; j++)
            {
                var material = new PGLUmaterial();
                material.FromStream(bs, baseMdlPos);
                VariationMaterials[i].Add(material);
            }
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
            shape.FromStream(bs);
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
            HostMethodInfos.Add(registerInfo);
        }
    }

    private void ReadExternalInfos(BinaryStream bs, long baseMdlPos, uint offset, uint count)
    {
        for (ushort i = 0; i < count; i++)
        {
            bs.Position = baseMdlPos + offset + i * RegisterInfo.GetSize();

            RegisterInfo registerInfo = new RegisterInfo();
            registerInfo.FromStream(bs, baseMdlPos);
            ExternalInfos.Add(registerInfo);
        }
    }

    private void ReadOutRegisterInfo(BinaryStream bs, long baseMdlPos, uint offset, uint count)
    {
        for (ushort i = 0; i < count; i++)
        {
            bs.Position = baseMdlPos + offset + i * RegisterInfo.GetSize();

            RegisterInfo registerInfo = new RegisterInfo();
            registerInfo.FromStream(bs, baseMdlPos);
            OutRegisterInfos.Add(registerInfo);
        }
    }

    private void ReadTextureSets(BinaryStream bs, long baseMdlPos, uint offset, uint listCount, uint textureLodLevels)
    {
        for (int i = 0; i < listCount; i++)
        {
            bs.Position = baseMdlPos + offset + i * sizeof(int);
            int entriesOffset = bs.ReadInt32();

            // One texture set per lod level
            List<TextureSet1> list = new();
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

    public override List<TextureSet1> GetTextureSetList()
    {
        if (TextureSetLists.Count == 0)
            TextureSetLists.Add([]);

        return TextureSetLists[0];
    }

    public override uint AddShape(PGLUshape shape)
    {
        Shapes.Add(shape);
        return (uint)(Shapes.Count - 1);
    }

    public override int AddMaterial(PGLUmaterial material)
    {
        Materials.Add(material);
        return Materials.Count - 1;
    }

    public override int GetMaterialCount()
    {
        return Materials.Count;
    }

    public int GetInstanceSize()
    {
        int size = 0x20; // header/meta
        size += (OutRegisterInfos.Count + InstanceUnkRegisterCount + HostMethodInfos.Count) * sizeof(uint);

        int maxRegisterSize = 0;
        foreach (var hostMethodInfo in HostMethodInfos)
        {
            int registerSize = hostMethodInfo.RegisterIndex + hostMethodInfo.ArrayLength;
            if (maxRegisterSize < registerSize)
                maxRegisterSize = registerSize;
        }

        int totalRegisterSize = 0;
        foreach (var hostMethodInfo in HostMethodInfos)
        {
            int registerSize = hostMethodInfo.RegisterIndex + hostMethodInfo.ArrayLength;
            if (maxRegisterSize < registerSize)
                maxRegisterSize = registerSize;
        }

        size += totalRegisterSize * sizeof(int);
        return size;
    }

    public override List<PGLUmaterial> GetVariationMaterials(int varIndex)
    {
        return varIndex == 0 ? Materials : VariationMaterials[varIndex];
    }

    public override int GetNumModels()
    {
        return Models.Count;
    }

    public override PGLUshape GetShape(int shapeIndex)
    {
        return Shapes[shapeIndex];
    }

    public override int GetNumShapes()
    {
        return Shapes.Count;
    }

    public override int GetNumVariations()
    {
        return Math.Max(1, VariationMaterials.Count);
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
