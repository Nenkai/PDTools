using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

using Syroot.BinaryData;

using PDTools.Files.Textures.PS2;

namespace PDTools.Files.Models.PS2.ModelSet;

public class ModelSet2Serializer
{
    private ModelSet2 _modelSet { get; set; }
    private long _baseMdlPos;

    private readonly Dictionary<int, uint> TexSetHashToOffset = [];
    private readonly Dictionary<int, uint> TexSetListHashToOffset = [];

    private List<uint> RelocatableOffsets = [];

    private RelocatorBase _relocator = new RelocatorBase();

    public ModelSet2Serializer(ModelSet2 modelSet)
    {
        _modelSet = modelSet;
    }

    
    public void Write(Stream stream)
    {
        BinaryStream bs = new BinaryStream(stream, ByteConverter.Little);

        _baseMdlPos = bs.Position;

        // Skip header for now
        bs.Position = _baseMdlPos + ModelSet2.HeaderSize;

        WriteInstance(bs);
        WriteMaterials(bs);
        WriteVariationMaterials(bs);

        // Skip shape offset table for now
        long shapeTableOffset = bs.Position;
        bs.Position += _modelSet.Shapes.Count * sizeof(uint);
        bs.Align(0x10, grow: true);

        WriteModels(bs);
        WriteShapes(bs, shapeTableOffset);
        // Bind matrices
        WriteVariationTextureSets(bs);

        long relocationInfoOffset = bs.Position;
        _relocator.WriteRelocationData(bs);
        uint relocationDataSize = (uint)(bs.Position - relocationInfoOffset);

        long lastPos = bs.Position;

        // Finish header.
        bs.Position = _baseMdlPos;
        bs.WriteUInt32(ModelSet2.MAGIC);
        bs.WriteUInt32((uint)(relocationInfoOffset - _baseMdlPos));
        bs.WriteUInt32(relocationDataSize);
        bs.WriteUInt32(0); // Relocation base
        bs.WriteUInt32((uint)lastPos); // File size
        bs.WriteByte(0); // 0x14: unk
        bs.WriteByte(_modelSet.InstanceFlags); // 0x15: InstanceFlags (preserves original or 0 for new models)

        bs.Position = lastPos;
    }

    private void WriteInstance(BinaryStream bs)
    {
        long instanceOffset = bs.Position;
        int size = _modelSet.GetInstanceSize();

        // Instance header: 0x20 bytes (explicit zero-init; runtime fills +0x00 and +0x10)
        bs.WriteInt32(0); // +0x00: parentModelSetPtr (runtime fills this)
        long outRegsPtrPos = bs.Position;
        bs.WriteInt32(0); // +0x04: outRegistersPtr (patched below if registers exist)
        long unkRegsPtrPos = bs.Position;
        bs.WriteInt32(0); // +0x08: unkRegistersPtr (patched below if registers exist)
        long hostMethodRegsPtrPos = bs.Position;
        bs.WriteInt32(0); // +0x0C: hostMethodRegistersPtr (patched below if registers exist)
        bs.WriteInt32(0); // +0x10: hostMethodInfosFuncs (runtime fills this)
        bs.WriteInt32(0); // +0x14
        bs.WriteInt32(0); // +0x18
        bs.WriteInt32(0); // +0x1C

        // Register arrays follow the 0x20-byte header
        long outRegistersOffset = bs.Position;
        for (int i = 0; i < _modelSet.OutRegisterInfos.Count; i++)
            bs.WriteUInt32(0);

        long unkRegistersOffset = bs.Position;
        for (int i = 0; i < _modelSet.InstanceUnkRegisterCount; i++) // Fixed: was missing entirely
            bs.WriteUInt32(0);

        long hostMethodRegistersOffset = bs.Position;
        for (int i = 0; i < _modelSet.HostMethodInfos.Count; i++)
            bs.WriteUInt32(0);

        long lastPos = bs.Position;

        // Patch register pointers in instance header
        if (_modelSet.OutRegisterInfos.Count > 0)
        {
            bs.Position = outRegsPtrPos;
            WriteOffset32(bs, (uint)(outRegistersOffset - _baseMdlPos));
        }

        if (_modelSet.InstanceUnkRegisterCount > 0) // Fixed: was missing entirely
        {
            bs.Position = unkRegsPtrPos;
            WriteOffset32(bs, (uint)(unkRegistersOffset - _baseMdlPos));
        }

        if (_modelSet.HostMethodInfos.Count > 0)
        {
            bs.Position = hostMethodRegsPtrPos;
            WriteOffset32(bs, (uint)(hostMethodRegistersOffset - _baseMdlPos));
        }

        // Write instance offset and size to MDLS header
        bs.Position = _baseMdlPos + 0x7C;
        WriteOffset32(bs, (uint)(instanceOffset - _baseMdlPos));

        bs.Position = _baseMdlPos + 0x36;
        bs.WriteInt16((short)-size);

        bs.Position = lastPos;
    }

    private void WriteMaterials(BinaryStream bs)
    {
        long materialsOffset = bs.Position;

        for (int i = 0; i < _modelSet.Materials.Count; i++)
        {
            PGLUmaterial material = _modelSet.Materials[i];
            material.Write(bs);
        }

        long lastPos = bs.Position;

        if (_modelSet.Materials.Count > 0)
        {
            bs.Position = _baseMdlPos + 0x1A;
            bs.WriteUInt16((ushort)_modelSet.Materials.Count);

            bs.Position = _baseMdlPos + 0x40;
            WriteOffset32(bs, ((uint)(materialsOffset - _baseMdlPos)));
        }

        bs.Position = lastPos;
    }

    private void WriteVariationMaterials(BinaryStream bs)
    {
        long variationMaterialsOffset = bs.Position;

        long dataOffset = bs.Position + (_modelSet.VariationMaterials.Count * sizeof(int));
        for (int i = 0; i < _modelSet.VariationMaterials.Count; i++)
        {
            bs.Position = variationMaterialsOffset + (i * 0x04);
            WriteOffset32(bs, (uint)(dataOffset - _baseMdlPos)); // Fixed: was _baseMdlPos - dataOffset

            bs.Position = dataOffset;
            for (int j = 0; j < _modelSet.VariationMaterials[i].Count; j++) // Fixed: j < count, [i] not [j]
            {
                PGLUmaterial material = _modelSet.VariationMaterials[i][j]; // Fixed: [i][j] not [j][i]
                material.Write(bs);
            }
            dataOffset = bs.Position;
        }

        long lastPos = bs.Position;

        if (_modelSet.VariationMaterials.Count > 0)
        {
            bs.Position = _baseMdlPos + 0x60;
            WriteOffset32(bs, ((uint)(variationMaterialsOffset - _baseMdlPos)));

            bs.Position = _baseMdlPos + 0x2C;
            bs.WriteByte((byte)_modelSet.VariationMaterials.Count);
        }

        bs.Position = lastPos;
    }

    private void WriteModels(BinaryStream bs)
    {
        long modelTableOffset = bs.Position;
        long lastDataOffset = bs.Position + (_modelSet.Models.Count * ModelSet2Model.GetSize());
        for (int i = 0; i < _modelSet.Models.Count; i++)
        {
            long modelOffset = modelTableOffset + (i * ModelSet2Model.GetSize());
            bs.Position = modelOffset;
            ModelSet2Model model = (ModelSet2Model)_modelSet.Models[i];
            model.Write(bs);

            // Write bounds
            if (model.Bounds.Count > 0)
            {
                bs.Position = lastDataOffset;
                long boundsOffset = bs.Position;
                model.WriteBounds(bs);
                lastDataOffset = bs.Position;

                bs.Position = modelOffset + 0x04;
                WriteOffset32(bs, ((uint)(boundsOffset - _baseMdlPos)));

                bs.Position = lastDataOffset;
            }

            // Write commands
            long commandsOffset = bs.Position;
            model.WriteCommands(bs);
            bs.Align(0x04, grow: true);
            lastDataOffset = bs.Position;

            bs.Position = modelOffset + 0x18;
            WriteOffset32(bs, ((uint)(commandsOffset - _baseMdlPos)));

            bs.Position = lastDataOffset;
        }

        bs.Align(0x10, grow: true);
        lastDataOffset = bs.Position;

        if (_modelSet.Models.Count > 0)
        {
            bs.Position = _baseMdlPos + 0x38;
            WriteOffset32(bs, ((uint)(modelTableOffset - _baseMdlPos)));

            bs.Position = _baseMdlPos + 0x16;
            bs.WriteUInt16((ushort)_modelSet.Models.Count);
        }

        bs.Position = lastDataOffset;
    }

    private void WriteShapes(BinaryStream bs, long shapeTableOffset)
    {
        long lastDataOffset = bs.Position;
        for (int i = 0; i < _modelSet.Shapes.Count; i++)
        {
            bs.Position = shapeTableOffset + i * sizeof(uint);
            WriteOffset32(bs, (uint)(lastDataOffset - _baseMdlPos));

            bs.Position = lastDataOffset;
            PGLUshape shape = _modelSet.Shapes[i];
            shape.Write(bs, _baseMdlPos);

            lastDataOffset = bs.Position;
        }

        if (_modelSet.Shapes.Count > 0)
        {
            bs.Position = _baseMdlPos + 0x18;
            bs.WriteUInt16((ushort)_modelSet.Shapes.Count);

            bs.Position = _baseMdlPos + 0x3C;
            WriteOffset32(bs, ((uint)(shapeTableOffset - _baseMdlPos)));
        }

        bs.Position = lastDataOffset;
    }

    /*

    private static int GetMatTableHashCode(List<PGLUmaterial> mat)
    {
        int hash = 0;
        for (int i = 0; i < mat.Count; i++)
            hash += mat[i].GetHashCode();
        return hash;
    }
    */

    private void WriteVariationTextureSets(BinaryStream bs)
    {
        long colorsOffset = bs.Position;
        long lastDataOffset = colorsOffset + (_modelSet.TextureSetLists.Count * sizeof(uint));
        for (int i = 0; i < _modelSet.TextureSetLists.Count; i++)
        {
            int listHash = GetTexSetListHash(_modelSet.TextureSetLists[i]);
            if (TexSetListHashToOffset.TryGetValue(listHash, out uint texSetTableOffset))
            {
                bs.Position = colorsOffset + i * sizeof(uint);
                WriteOffset32(bs, (uint)(texSetTableOffset - _baseMdlPos));
            }
            else
            {
                bs.Position = colorsOffset + i * sizeof(uint);
                WriteOffset32(bs, (uint)(lastDataOffset - _baseMdlPos));

                List<TextureSet1> list = _modelSet.TextureSetLists[i];

                long texOffsetTable = lastDataOffset;
                bs.Position = lastDataOffset;
                bs.Position += sizeof(int) * list.Count;
                bs.Align(0x10, grow: true);

                lastDataOffset = bs.Position;

                for (int j = 0; j < list.Count; j++)
                {
                    TextureSet1 texSet = list[j];
                    int texSetHash = texSet.GetHashCode();

                    if (TexSetHashToOffset.TryGetValue(texSetHash, out uint texSetOffset))
                    {
                        bs.Position = texOffsetTable + j * sizeof(uint);
                        WriteOffset32(bs, (uint)(texSetOffset - _baseMdlPos));
                    }
                    else
                    {
                        bs.Position = texOffsetTable + j * sizeof(uint);
                        WriteOffset32(bs, (uint)(lastDataOffset - _baseMdlPos));

                        bs.Position = lastDataOffset;
                        TexSetHashToOffset.Add(texSetHash, (uint)lastDataOffset);

                        texSet.Serialize(bs);
                        bs.Align(0x10, grow: true);

                        lastDataOffset = bs.Position;
                    }
                }

                TexSetListHashToOffset.Add(listHash, (uint)texOffsetTable); // Fixed: was never populated
            }
        }

        if (_modelSet.TextureSetLists.Count > 0)
        {
            bs.Position = _baseMdlPos + 0x1C;
            bs.WriteUInt16((ushort)_modelSet.TextureSetLists[0].Count);

            bs.Position = _baseMdlPos + 0x1E;
            bs.WriteUInt16((ushort)_modelSet.TextureSetLists.Count);

            bs.Position = _baseMdlPos + 0x44;
            WriteOffset32(bs, (uint)(colorsOffset - _baseMdlPos));
        }

        bs.Position = lastDataOffset;
    }

    private static int GetTexSetListHash(List<TextureSet1> texSet1)
    {
        int hash = 0;
        foreach (var i in texSet1)
            hash += i.GetHashCode();

        return hash;
    }

    private void WriteOffset32(BinaryStream bs, uint offset)
    {
        uint writeTarget = (uint)(bs.Position - _baseMdlPos);
        _relocator.Add(RelocatePointerType.Update4, writeTarget); // Where we are writing to
        bs.WriteUInt32(offset);
    }
}
