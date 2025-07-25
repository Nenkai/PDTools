﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Buffers.Binary;

using Syroot.BinaryData.Core;
using Syroot.BinaryData;
using Syroot.BinaryData.Memory;

using PDTools.Files.Textures;
using PDTools.Files.Models.VM;
using PDTools.Utils;
using PDTools.Files.Models.Shaders;
using PDTools.Files.Models.ShapeStream;

using ShapeStreamData = PDTools.Files.Models.ShapeStream.ShapeStream;

using PDTools.Files.Models.PS3.ModelSet3.FVF;
using PDTools.Files.Models.PS3.ModelSet3.Materials;
using PDTools.Files.Models.PS3.ModelSet3.Shapes;
using PDTools.Files.Models.PS3.ModelSet3.PackedMesh;
using PDTools.Files.Models.PS3.ModelSet3.ShapeStream;
using PDTools.Files.Models.PS3.ModelSet3.Wing;
using PDTools.Files.Courses.PS3;
using PDTools.Files.Models.PS3.ModelSet3.Textures;
using PDTools.Files.Models.PS3.ModelSet3.Models;

namespace PDTools.Files.Models.PS3.ModelSet3;

public class ModelSet3
{
    const string MAGIC = "MDL3";
    const string MAGIC_LE = "3LDM";

    public const int HeaderSize = 0xE4;

    public ushort Version { get; set; }
    public long BaseOffset { get; set; }

    public List<ModelSet3Model> Models { get; set; } = [];
    public List<MDL3ModelKey> ModelKeys { get; set; } = [];

    public List<MDL3Shape> Shapes { get; set; } = [];
    public List<MDL3ShapeKey> ShapeKeys { get; set; } = [];
    public List<MDL3FlexibleVertexDefinition> FlexibleVertexFormats { get; set; } = [];
    public MDL3Materials Materials { get; set; } = new();
    public TextureSet3 TextureSet { get; set; }
    public ShadersHeader Shaders { get; set; }
    public List<MDL3Bone> Bones { get; set; } = [];
    public ushort _0x68Size { get; set; }
    public ushort VMStackSize { get; set; }
    public VMBytecode VirtualMachine { get; set; } = new VMBytecode();
    public Dictionary<ushort, RegisterInfo> VMHostMethodEntries { get; set; } = [];
    public List<MDL3TextureKey> TextureKeys { get; set; } = [];
    public List<MDL3WingData> WingData { get; set; } = [];
    public List<MDL3WingKey> WingKeys { get; set; } = [];
    public List<MDL3ModelVMUnk> UnkVMData { get; set; } = [];
    public MDL3ModelVMUnk2 UnkVMData2 { get; set; }
    public MDL3ModelVMContext VMContext { get; set; }
    public List<PackedMeshKey> PackedMeshKeys { get; set; } = [];
    public PackedMeshHeader PackedMesh { get; set; } = new();
    public MDL3ShapeStreamingManager StreamingInfo { get; set; }
    public ShapeStreamData ShapeStream { get; set; }

    public CourseDataFile ParentCourseData { get; set; }
    public BinaryStream Stream { get; set; }

    public static ModelSet3 FromStream(Stream stream, int txsPos = 0)
    {
        var bs = new BinaryStream(stream);
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
        ushort shapeCount = bs.ReadUInt16();
        ushort shapeKeyCount = bs.ReadUInt16();
        ushort flexibleVertexDefinitionCount = bs.ReadUInt16();
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
        uint shapesOffset = bs.ReadUInt32();
        uint shapeKeysOffset = bs.ReadUInt32();
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

        modelSet.ReadShapes(bs, basePos, shapesOffset, shapeCount);
        modelSet.ReadShapeKeys(bs, basePos, shapeKeysOffset, shapeKeyCount);

        modelSet.ReadFlexibleVertices(bs, basePos, flexibleVerticesOffset, flexibleVertexDefinitionCount);

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
        foreach (MDL3ModelKey modelKey in ModelKeys)
            Models[(ushort)modelKey.ModelID].Name = modelKey.Name;

        foreach (MDL3ShapeKey shapeKey in ShapeKeys)
            Shapes[(ushort)shapeKey.ShapeID].Name = shapeKey.Name;

        foreach (MDL3WingKey wingKey in WingKeys)
            WingData[(ushort)wingKey.WingDataID].Name = wingKey.Name;

        foreach (var shape in Shapes)
        {
            if (shape.FVFIndex != -1)
                shape.FVF = FlexibleVertexFormats[shape.FVFIndex];

            if (shape.MaterialIndex != -1)
                shape.Material = Materials.Definitions[shape.MaterialIndex];
        }
    }

    private void ReadModels(BinaryStream bs, long baseMdlPos, uint offset, uint count)
    {
        for (ushort i = 0; i < count; i++)
        {
            bs.Position = baseMdlPos + offset + i * ModelSet3Model.GetSize();
            Models.Add(ModelSet3Model.FromStream(bs, baseMdlPos, Version));
        }
    }

    private void ReadModelKeys(BinaryStream bs, long baseMdlPos, uint offset, uint count)
    {
        for (ushort i = 0; i < count; i++)
        {
            bs.Position = baseMdlPos + offset + i * MDL3ModelKey.GetSize();

            var key = MDL3ModelKey.FromStream(bs, baseMdlPos, Version);
            ModelKeys.Add(key);
        }
    }

    private void ReadShapes(BinaryStream bs, long baseMdlPos, uint offset, uint count)
    {
        for (ushort i = 0; i < count; i++)
        {
            bs.Position = baseMdlPos + offset + i * MDL3Shape.GetSize();
            Shapes.Add(MDL3Shape.FromStream(bs, baseMdlPos, Version));
        }
    }

    private void ReadShapeKeys(BinaryStream bs, long baseMdlPos, uint offset, uint count)
    {
        for (ushort i = 0; i < count; i++)
        {
            bs.Position = baseMdlPos + offset + i * MDL3ShapeKey.GetSize();

            var key = MDL3ShapeKey.FromStream(bs, baseMdlPos, Version);
            ShapeKeys.Add(key);

            Shapes[(ushort)key.ShapeID].Name = key.Name;
        }
    }

    private void ReadFlexibleVertices(BinaryStream bs, long baseMdlPos, uint offset, uint count)
    {
        for (ushort i = 0; i < count; i++)
        {
            bs.Position = baseMdlPos + offset + i * MDL3FlexibleVertexDefinition.GetSize();
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

            var hostMethodEntry = new RegisterInfo();
            hostMethodEntry.FromStream(bs, baseMdlPos);
            VMHostMethodEntries.Add(hostMethodEntry.RegisterIndex, hostMethodEntry);
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
            bs.Position = baseMdlPos + offset + i * MDL3Bone.GetSize();
            MDL3Bone bone = MDL3Bone.FromStream(bs, baseMdlPos);
            Bones.Add(bone);
        }
    }

    private void ReadTextureKeys(BinaryStream bs, long baseMdlPos, uint offset, uint count)
    {
        for (var i = 0; i < count; i++)
        {
            bs.Position = baseMdlPos + offset + i * MDL3TextureKey.GetSize();
            var key = MDL3TextureKey.FromStream(bs, baseMdlPos);
            TextureKeys.Add(key);
        }
    }

    private void ReadWingData(BinaryStream bs, long baseMdlPos, uint offset, uint count)
    {
        for (var i = 0; i < count; i++)
        {
            bs.Position = baseMdlPos + offset + i * MDL3WingData.GetSize();
            var data = MDL3WingData.FromStream(bs, baseMdlPos, Version);
            WingData.Add(data);
        }
    }

    private void ReadWingKeys(BinaryStream bs, long baseMdlPos, uint offset, uint count)
    {
        for (var i = 0; i < count; i++)
        {
            bs.Position = baseMdlPos + offset + i * MDL3WingKey.GetSize();
            var key = MDL3WingKey.FromStream(bs, baseMdlPos, Version);
            WingKeys.Add(key);
        }
    }

    private void ReadUnkVMData(BinaryStream bs, long baseMdlPos, uint offset, uint count)
    {
        for (var i = 0; i < count; i++)
        {
            bs.Position = baseMdlPos + offset + i * MDL3ModelVMUnk.GetSize();
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
            bs.Position = baseMdlPos + offset + i * PackedMeshKey.GetSize();
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
    /// Gets all the vertices for a specified shape.
    /// </summary>
    /// <param name="shapeIndex"></param>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException"></exception>
    /// <exception cref="NotSupportedException"></exception>
    public Vector3[] GetVerticesOfShape(ushort shapeIndex)
    {
        if ((short)shapeIndex == -1)
            throw new InvalidOperationException("Shape Index was -1.");

        var shape = Shapes[shapeIndex];

        if (shape.FVFIndex != -1)
        {
            MDL3FlexibleVertexDefinition fvfDef = FlexibleVertexFormats[shape.FVFIndex];
            if (!fvfDef.Elements.TryGetValue("position", out var field)
                && !fvfDef.Elements.TryGetValue("position_1", out field)
                && !fvfDef.Elements.TryGetValue("position_2", out field))
                throw new InvalidOperationException("FVF does not contain 'position' field.");

            if (field.FieldType != CELL_GCM_VERTEX_TYPE.CELL_GCM_VERTEX_F && field.FieldType != CELL_GCM_VERTEX_TYPE.CELL_GCM_VERTEX_S1 && field.FieldType != CELL_GCM_VERTEX_TYPE.CELL_GCM_VERTEX_SF)
                throw new NotSupportedException("Expected vector 3 with CELL_GCM_VERTEX_F or CELL_GCM_VERTEX_S1");

            var arr = new Vector3[shape.VertexCount];
            if (shape.VerticesOffset != 0 && Stream.CanRead)
            {
                Span<byte> vertBuffer = new byte[field.ArrayIndex == 0 ? fvfDef.VertexSize : fvfDef.ArrayDefinition.VertexSize];
                for (int i = 0; i < shape.VertexCount; i++)
                {
                    GetVerticesData(shape, fvfDef, field, i, vertBuffer);
                    arr[i] = GetFVFFieldVector3(vertBuffer, field.FieldType, field.StartOffset, field.ElementCount);
                }
            }
            else if (ShapeStream != null)
            {
                // Try shapestream
                var ssShape = ShapeStream.GetShapeByIndex(shapeIndex);
                if (ssShape is null)
                    return arr;

                Span<byte> vertBuffer = new byte[field.ArrayIndex == 0 ? fvfDef.VertexSize : fvfDef.ArrayDefinition.VertexSize];
                for (int i = 0; i < shape.VertexCount; i++)
                {
                    GetShapeStreamVerticesData(ssShape, fvfDef, field, i, vertBuffer);
                    arr[i] = GetFVFFieldVector3(vertBuffer, field.FieldType, field.StartOffset, field.ElementCount);
                }
            }

            return arr;
        }
        else if (Version >= 9 && PackedMesh != null && shape.PackedMeshRef != null)
        {
            PMSHMesh entry = PackedMesh.Meshes[shape.PackedMeshRef.PackedMeshEntryIndex];
            PMSHFlexVertexDefinition flexDef = PackedMesh.StructDeclarations[entry.FlexVertexDeclarationID];
            PMSHFlexVertexElementDefinition element = flexDef.GetElement("position");

            if (element is null)
                return null;

            if (element.IsPacked)
            {
                PMSHElementBitLayoutArray bitLayouts = PackedMesh.BitLayoutDefinitionArray[entry.ElementBitLayoutDefinitionID];
                var arr = new Vector3[entry.DataList[0].PackedFlexVertCount];

                PMSHElementBitLayout bitDef = GetPackedBitLayoutOfField(bitLayouts, flexDef, element.Name);
                for (int i = 0; i < entry.DataList[0].PackedFlexVertCount; i++)
                {
                    var v4 = ReadPackedElement(entry, flexDef, bitLayouts, bitDef, element, i);
                    arr[i] = new Vector3(v4.X, v4.Y, v4.Z);
                }

                return arr;
            }
            else
            {
                Span<byte> vertBuffer = new byte[flexDef.NonPackedStride];
                var arr = new Vector3[entry.DataList[0].PackedFlexVertCount];

                for (int i = 0; i < entry.DataList[0].PackedFlexVertCount; i++)
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
        MDL3Shape mesh = Shapes[meshIndex];
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
            var ssShape = ShapeStream.GetShapeByIndex(meshIndex);
            if (ssShape is null)
                return null;

            SpanReader shapeReader = new SpanReader(ssShape.MeshData.Span, Endian.Big);
            shapeReader.Position = (int)ssShape.TriOffset;

            for (int i = 0; i < mesh.TriCount; i++)
            {
                ushort a = shapeReader.ReadUInt16();
                ushort b = shapeReader.ReadUInt16();
                ushort c = shapeReader.ReadUInt16();
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
    * 1. Shape -> Material ID -> Material Entry
    * 2. Material Entry -> Material Data ID -> Material Data Entry
    * 3. Material Data Entry -> 0x14 Entry -> Shader Entry Index -> Shader Entry 0x3C or Shader Def Entry
    * 4. Shader Def Entry -> Shader Program ID -> Shader Program Entry
    * 5. Shader Program -> Actual Program data.
    * 
    * UV stuff is handling there, not sure how that's done yet. The floats don't seem to be directly it - didn't seem to work with GT6 midfield
    * */
    /// <summary>
    /// Gets all the UVs for a specified shape. Read comment above this function as there are some issues
    /// </summary>
    /// <param name="shapeIndex"></param>
    /// <returns></returns>
    public Vector2[] GetUVsOfMesh(ushort shapeIndex)
    {
        var shape = Shapes[shapeIndex];
        Vector2[] arr;

        var mat = Materials.Definitions[shape.MaterialIndex];
        MDL3MaterialData matData = Materials.MaterialDatas[mat.MaterialDataID];
        ShaderDefinition shader = Shaders.Definitions[matData._0x14.ShaderID];
        var prog = Shaders.Programs0x20[shader.ProgramID];

        /*
        float scaleX = BinaryPrimitives.ReadSingleBigEndian(prog.Program.AsSpan(0x20));
        float scaleY = BinaryPrimitives.ReadSingleBigEndian(prog.Program.AsSpan(0x24));
        float scaleZ = BinaryPrimitives.ReadSingleBigEndian(prog.Program.AsSpan(0x28));
        */

        if (shape.FVFIndex != -1)
        {
            MDL3FlexibleVertexDefinition fvfDef = FlexibleVertexFormats[shape.FVFIndex];
            if (!fvfDef.Elements.TryGetValue("map1", out var field) &&
                !fvfDef.Elements.TryGetValue("map12", out field) &&
                !fvfDef.Elements.TryGetValue("map12_2", out field))
                return [];

            arr = new Vector2[shape.VertexCount];
            if (shape.VerticesOffset != 0 && Stream.CanRead)
            {
                Span<byte> buffer = new byte[field.ArrayIndex == 0 ? fvfDef.VertexSize : fvfDef.ArrayDefinition.VertexSize];
                for (int i = 0; i < shape.VertexCount; i++)
                {
                    GetVerticesData(shape, fvfDef, field, i, buffer);
                    arr[i] = GetFVFFieldVector2(buffer, field.FieldType, field.StartOffset, field.ElementCount);
                }
            }
            else if (ShapeStream != null)
            {
                // Try shapestream
                var ssMesh = ShapeStream.GetShapeByIndex(shapeIndex);
                if (ssMesh is null)
                    return arr;

                Span<byte> buffer = new byte[field.ArrayIndex == 0 ? fvfDef.VertexSize : fvfDef.ArrayDefinition.VertexSize];
                for (int i = 0; i < shape.VertexCount; i++)
                {
                    GetShapeStreamVerticesData(ssMesh, fvfDef, field, i, buffer);
                    arr[i] = GetFVFFieldVector2(buffer, field.FieldType, field.StartOffset, field.ElementCount);
                }
            }
        }
        else if (Version >= 9 && PackedMesh != null && shape.PackedMeshRef != null)
        {
            PMSHMesh entry = PackedMesh.Meshes[shape.PackedMeshRef.PackedMeshEntryIndex];
            PMSHFlexVertexDefinition flexDef = PackedMesh.StructDeclarations[entry.FlexVertexDeclarationID];
            PMSHFlexVertexElementDefinition element = flexDef.GetElement("map12");

            if (element is null)
                return null;

            if (element.IsPacked)
            {
                PMSHElementBitLayoutArray bitLayouts = PackedMesh.BitLayoutDefinitionArray[entry.ElementBitLayoutDefinitionID];
                arr = new Vector2[entry.DataList[0].PackedFlexVertCount];

                PMSHElementBitLayout bitDef = GetPackedBitLayoutOfField(bitLayouts, flexDef, element.Name);
                for (int i = 0; i < entry.DataList[0].PackedFlexVertCount; i++)
                {
                    var v4 = ReadPackedElement(entry, flexDef, bitLayouts, bitDef, element, i);
                    arr[i] = new Vector2(v4.X, v4.Y);
                }
            }
            else
            {
                Span<byte> vertBuffer = new byte[flexDef.NonPackedStride];
                arr = new Vector2[entry.DataList[0].PackedFlexVertCount];

                for (int i = 0; i < entry.DataList[0].PackedFlexVertCount; i++)
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
    /// <param name="shapeIndex"></param>
    /// <returns></returns>
    public (uint, uint, uint)[] GetNormalsOfShape(ushort shapeIndex)
    {
        var shape = Shapes[shapeIndex];
        if (shape.FVFIndex != -1)
        {
            MDL3FlexibleVertexDefinition fvfDef = FlexibleVertexFormats[shape.FVFIndex];
            if (!fvfDef.Elements.TryGetValue("normal", out var field))
                return [];

            if (shape.VerticesOffset != 0 && Stream.CanRead)
            {
                var arr = new (uint, uint, uint)[shape.VertexCount];
                Span<byte> buffer = new byte[field.ArrayIndex == 0 ? fvfDef.VertexSize : fvfDef.ArrayDefinition.VertexSize];
                for (int i = 0; i < shape.VertexCount; i++)
                {
                    GetVerticesData(shape, fvfDef, field, i, buffer);
                    arr[i] = GetFVFFieldXYZ(buffer, field.FieldType, field.StartOffset, field.ElementCount);
                }

                return arr;
            }
            else if (ShapeStream != null)
            {
                var arr = new (uint, uint, uint)[shape.VertexCount];

                // Try shapestream
                var ssShape = ShapeStream.GetShapeByIndex(shapeIndex);
                if (ssShape is null)
                    return arr;

                Span<byte> buffer = new byte[field.ArrayIndex == 0 ? fvfDef.VertexSize : fvfDef.ArrayDefinition.VertexSize];
                for (int i = 0; i < shape.VertexCount; i++)
                {
                    GetShapeStreamVerticesData(ssShape, fvfDef, field, i, buffer);
                    arr[i] = GetFVFFieldXYZ(buffer, field.FieldType, field.StartOffset, field.ElementCount);
                }

                return arr;
            }

        }
        else if (Version >= 9 && PackedMesh != null && shape.PackedMeshRef != null)
        {
            PMSHMesh entry = PackedMesh.Meshes[shape.PackedMeshRef.PackedMeshEntryIndex];
            PMSHFlexVertexDefinition flexDef = PackedMesh.StructDeclarations[entry.FlexVertexDeclarationID];
            PMSHFlexVertexElementDefinition element = flexDef.GetElement("normal");

            if (element is null)
                return null;

            if (element.IsPacked)
            {
                var arr = new (uint, uint, uint)[entry.DataList[0].PackedFlexVertCount];

                PMSHElementBitLayoutArray bitLayouts = PackedMesh.BitLayoutDefinitionArray[entry.ElementBitLayoutDefinitionID];
                PMSHElementBitLayout bitDef = GetPackedBitLayoutOfField(bitLayouts, flexDef, element.Name);

                for (int i = 0; i < entry.DataList[0].PackedFlexVertCount; i++)
                {
                    var v4 = ReadPackedElement(entry, flexDef, bitLayouts, bitDef, element, i);
                    arr[i] = ((uint)v4.X, (uint)v4.Y, (uint)v4.Z);
                }

                return arr;
            }
            else
            {
                var arr = new (uint, uint, uint)[entry.DataList[0].NonPackedFlexVertCount];
                Span<byte> vertBuffer = new byte[flexDef.NonPackedStride];

                for (int i = 0; i < entry.DataList[0].NonPackedFlexVertCount; i++)
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
    /// Gets the BBox of a shape.
    /// </summary>
    /// <param name="shapeIndex"></param>
    /// <returns></returns>
    public Vector3[] GetBBoxOfShape(ushort shapeIndex)
    {
        var shape = Shapes[shapeIndex];

        if (shape.BBox == null && ShapeStream != null)
        {
            // Try shapestream
            var ssShape = ShapeStream.GetShapeByIndex(shapeIndex);
            if (ssShape is null)
                return null;

            SpanReader shapeReader = new SpanReader(ssShape.MeshData.Span, Endian.Big);

            shapeReader.Position = (int)ssShape.BBoxOffset;
            shape.BBox = new Vector3[8];
            for (var i = 0; i < 8; i++)
                shape.BBox[i] = new Vector3(shapeReader.ReadSingle(), shapeReader.ReadSingle(), shapeReader.ReadSingle());
        }

        return shape.BBox;
    }

    private Vector4 ReadPackedElement(PMSHMesh entry,
        PMSHFlexVertexDefinition flexDef,
        PMSHElementBitLayoutArray bitLayouts,
        PMSHElementBitLayout bitDef,
        PMSHFlexVertexElementDefinition element,
        int vertIndex)
    {

        Stream.Position = entry.DataList[0].PackedFlexVertCount + entry.DataList[0].GetOffsetOfPackedElement(bitLayouts, flexDef, element.Name);
        Stream.Position += bitDef.TotalBitCount * vertIndex / 8;
        int rem = bitDef.TotalBitCount * vertIndex % 8;

        Span<byte> vertBuffer = new byte[(bitDef.TotalBitCount + 7) / 8 + 1];
        Stream.ReadExactly(vertBuffer);

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

        x = (x + 2f) * bitDef.ScaleX + bitDef.OffsetX;
        y = (y + 2f) * bitDef.ScaleY + bitDef.OffsetY;
        z = (z + 2f) * bitDef.ScaleZ + bitDef.OffsetZ;
        w = (w + 2f) * bitDef.ScaleW + bitDef.OffsetW;

        return new Vector4(x, y, z, w);
    }

    /// <summary>
    /// Gets a flex vertex
    /// </summary>
    /// <param name="shape"></param>
    /// <param name="fvfDef"></param>
    /// <param name="field"></param>
    /// <param name="vertIndex"></param>
    /// <param name="buffer"></param>
    public void GetVerticesData(MDL3Shape shape, MDL3FlexibleVertexDefinition fvfDef, MDL3FVFElementDefinition field, int vertIndex, Span<byte> buffer)
    {
        if (field.ArrayIndex == 0)
        {
            Stream.Position = BaseOffset + shape.VerticesOffset + vertIndex * fvfDef.VertexSize;
            Stream.ReadExactly(buffer);
        }
        else
        {
            Stream.Position = BaseOffset + shape.VerticesOffset + fvfDef.ArrayDefinition.DataOffset + fvfDef.ArrayDefinition.ArrayElementSize * field.ArrayIndex;
            Stream.Position += vertIndex * fvfDef.ArrayDefinition.VertexSize;
            Stream.ReadExactly(buffer);
        }
    }

    /// <summary>
    /// Gets a flex vertex stride from a shapestream
    /// </summary>
    /// <param name="ssShapeInfo"></param>
    /// <param name="fvfDef"></param>
    /// <param name="field"></param>
    /// <param name="vertIndex"></param>
    /// <param name="buffer"></param>
    public static void GetShapeStreamVerticesData(ShapeStreamShape ssShapeInfo, MDL3FlexibleVertexDefinition fvfDef, MDL3FVFElementDefinition field, int vertIndex, Span<byte> buffer)
    {
        SpanReader shapeReader = new SpanReader(ssShapeInfo.MeshData.Span);
        if (field.ArrayIndex == 0)
        {
            shapeReader.Position = (int)(ssShapeInfo.VerticesOffset + vertIndex * fvfDef.VertexSize);
            shapeReader.Span.Slice(shapeReader.Position, fvfDef.VertexSize).CopyTo(buffer);
        }
        else
        {
            shapeReader.Position = (int)(ssShapeInfo.VerticesOffset + fvfDef.ArrayDefinition.DataOffset + fvfDef.ArrayDefinition.ArrayElementSize * field.ArrayIndex);
            shapeReader.Position += vertIndex * fvfDef.ArrayDefinition.VertexSize;
            shapeReader.Span.Slice(shapeReader.Position, fvfDef.ArrayDefinition.VertexSize).CopyTo(buffer);
        }
    }

    public void GetPackedMeshRawElementBuffer(PMSHMesh entry, PMSHFlexVertexDefinition flexStruct, int vertIndex, Span<byte> buffer)
    {
        // TODO: Don't use [0]!
        Stream.Position = BaseOffset + entry.DataList[0].NonPackedFlexVertCount + vertIndex * flexStruct.NonPackedStride;
        Stream.ReadExactly(buffer);
    }

    private static PMSHElementBitLayout GetPackedBitLayoutOfField(PMSHElementBitLayoutArray bitLayouts, PMSHFlexVertexDefinition flexStruct, string type)
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
    public static Vector3 GetFVFFieldVector3(Span<byte> buffer, CELL_GCM_VERTEX_TYPE fieldType, int startOffset, int elementCount)
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
    public static (uint, uint, uint) GetFVFFieldXYZ(Span<byte> buffer, CELL_GCM_VERTEX_TYPE fieldType, int startOffset, int elementCount)
    {
        if (elementCount != 1)
            throw new InvalidOperationException("Expected 1 element");

        SpanReader sr = new SpanReader(buffer, Endian.Big);
        sr.Position = startOffset;

        if (fieldType == CELL_GCM_VERTEX_TYPE.CELL_GCM_VERTEX_CMP)
        {
            uint data = sr.ReadUInt32();
            return (data & 0b11_11111111, data >> 10 & 0b111_11111111, data >> 21 & 0b111_11111111);
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
    public static Vector2 GetFVFFieldVector2(Span<byte> buffer, CELL_GCM_VERTEX_TYPE fieldType, int startOffset, int elementCount)
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
            v1 = sr.ReadInt16() / 16384f;
            v2 = sr.ReadInt16() / 16384f;
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

