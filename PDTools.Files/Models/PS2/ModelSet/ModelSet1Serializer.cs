using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

using Syroot.BinaryData;

using PDTools.Files.Textures.PS2;

namespace PDTools.Files.Models.PS2.ModelSet
{
    public class ModelSet1Serializer
    {
        private ModelSet1 _modelSet { get; set; }

        private Dictionary<int, uint> TexSetHashToOffset = new Dictionary<int, uint>();
        private Dictionary<int, uint> TexSetListHashToOffset = new Dictionary<int, uint>();

        public ModelSet1Serializer(ModelSet1 modelSet)
        {
            _modelSet = modelSet;
        }

        public void Write(Stream stream)
        {
            BinaryStream bs = new BinaryStream(stream, ByteConverter.Little);

            long basePos = bs.Position;

            // Skip header for now
            bs.Position = basePos + 0x50;

            WriteMaterials(bs, basePos);

            // Skip variation materials table offset table for now
            long variationMaterialTableOffset = bs.Position;
            bs.Position += _modelSet.VariationMaterialsTable.Count * sizeof(uint);
            bs.Align(0x10, grow: true);
            WriteVariationMaterialTables(bs, basePos, variationMaterialTableOffset);

            // Skip model offset table for now
            long modelTableOffset = bs.Position;
            bs.Position += _modelSet.Models.Count * sizeof(uint);
            bs.Align(0x10, grow: true);

            // Skip shape offset table for now
            long shapeTableOffset = bs.Position;
            bs.Position += _modelSet.Shapes.Count * sizeof(uint);
            bs.Align(0x10, grow: true);

            WriteBoundings(bs, basePos);
            WriteModels(bs, basePos, modelTableOffset);
            WriteShapes(bs, basePos, shapeTableOffset);
            bs.Align(0x10, grow: true);

            // Skip variation tex set table for now
            long variationTexSetTableOffset = bs.Position;
            bs.Position += _modelSet.VariationTexSet.Count * sizeof(uint);

            // Skip tex set table for now
            long texSetTableOffset = bs.Position;
            bs.Position += _modelSet.TextureSets.Count * sizeof(uint);
            bs.Align(0x40, grow: true);

            WriteTextureSets(bs, basePos, texSetTableOffset);
            WriteVariationTextureSets(bs, basePos, variationTexSetTableOffset);

            long lastPos = bs.Position;

            // Finish header.
            bs.Position = basePos;
            bs.WriteUInt32(ModelSet1.MAGIC);

            bs.Position = lastPos;
        }

        private void WriteMaterials(BinaryStream bs, long baseModelSetPos)
        {
            long materialsOffset = bs.Position;

            for (int i = 0; i < _modelSet.Materials.Count; i++)
            {
                PGLUmaterial material = _modelSet.Materials[i];
                material.Write(bs);
            }

            long lastPos = bs.Position;

            bs.Position = baseModelSetPos + 0x14;
            bs.WriteUInt16((ushort)_modelSet.Materials.Count);

            bs.Position = baseModelSetPos + 0x28;
            bs.WriteUInt32((uint)(materialsOffset - baseModelSetPos));

            bs.Position = lastPos;
        }

        private void WriteBoundings(BinaryStream bs, long baseModelSetPos)
        {
            long boundingsOffset = bs.Position;

            for (int i = 0; i < _modelSet.Models.Count; i++)
            {
                ModelSet1Bounding bounding = _modelSet.Boundings[i];
                bounding.Write(bs);
            }

            long lastPos = bs.Position;

            bs.Position = baseModelSetPos + 0x30;
            bs.WriteUInt32((uint)(boundingsOffset - baseModelSetPos));

            bs.Position = lastPos;
        }

        private void WriteVariationMaterialTables(BinaryStream bs, long baseModelSetPos, long variationMaterialTableOffset)
        {
            long lastDataOffset = bs.Position;

            // Materials that are not different are only written once
            Dictionary<int, uint> tableHashToOffset = new Dictionary<int, uint>();
            for (int i = 0; i < _modelSet.VariationMaterialsTable.Count; i++)
            {
                List<PGLUmaterial> matTable = _modelSet.VariationMaterialsTable[i];
                int hash = GetMatTableHashCode(matTable);

                bs.Position = variationMaterialTableOffset + i * sizeof(uint);
                if (tableHashToOffset.TryGetValue(hash, out uint offset))
                {
                    bs.WriteUInt32(offset);
                    bs.Position = lastDataOffset;
                }
                else
                {
                    uint matOffset = (uint)(lastDataOffset - baseModelSetPos);
                    tableHashToOffset.Add(hash, matOffset);
                    bs.WriteUInt32(matOffset);

                    bs.Position = lastDataOffset;
                    for (int j = 0; j < _modelSet.VariationMaterialsTable[i].Count; j++)
                        _modelSet.VariationMaterialsTable[i][j].Write(bs);
                }

                lastDataOffset = bs.Position;
            }

            bs.Align(0x10, grow: true);
            lastDataOffset = bs.Position;

            bs.Position = baseModelSetPos + 0x1A;
            bs.WriteUInt16((ushort)_modelSet.VariationMaterialsTable.Count);

            bs.Position = baseModelSetPos + 0x38;
            bs.WriteUInt32((uint)(variationMaterialTableOffset - baseModelSetPos));

            bs.Position = lastDataOffset;
        }

        private static int GetMatTableHashCode(List<PGLUmaterial> mat)
        {
            int hash = 0;
            for (int i = 0; i < mat.Count; i++)
                hash += mat[i].GetHashCode();
            return hash;
        }

        private void WriteModels(BinaryStream bs, long baseModelSetOffset, long modelTableOffset)
        {
            long lastDataOffset = bs.Position;
            for (int i = 0; i < _modelSet.Models.Count; i++)
            {
                bs.Position = modelTableOffset + i * sizeof(uint);
                bs.WriteUInt32((uint)(lastDataOffset - baseModelSetOffset));

                bs.Position = lastDataOffset;
                ModelSet1Model model = (ModelSet1Model)_modelSet.Models[i];
                model.Write(bs);

                lastDataOffset = bs.Position;
            }
            bs.Align(0x10, grow: true);
            lastDataOffset = bs.Position;

            bs.Position = baseModelSetOffset + 0x10;
            bs.WriteUInt16((ushort)_modelSet.Models.Count);

            bs.Position = baseModelSetOffset + 0x20;
            bs.WriteUInt32((uint)(modelTableOffset - baseModelSetOffset));

            bs.Position = lastDataOffset;
        }

        private void WriteShapes(BinaryStream bs, long baseModelSetOffset, long shapeTableOffset)
        {
            long lastDataOffset = bs.Position;
            for (int i = 0; i < _modelSet.Shapes.Count; i++)
            {
                bs.Position = shapeTableOffset + i * sizeof(uint);
                bs.WriteUInt32((uint)(lastDataOffset - baseModelSetOffset));

                bs.Position = lastDataOffset;
                PGLUshape shape = _modelSet.Shapes[i];
                shape.Write(bs, baseModelSetOffset);

                lastDataOffset = bs.Position;
            }

            bs.Position = baseModelSetOffset + 0x12;
            bs.WriteUInt16((ushort)_modelSet.Shapes.Count);

            bs.Position = baseModelSetOffset + 0x24;
            bs.WriteUInt32((uint)(shapeTableOffset - baseModelSetOffset));

            bs.Position = lastDataOffset;
        }

        private void WriteTextureSets(BinaryStream bs, long baseModelSetOffset, long textureSetsOffset)
        {
            int texSetListHash = GetTexSetListHash(_modelSet.TextureSets);
            TexSetListHashToOffset.Add(texSetListHash, (uint)textureSetsOffset);

            long lastDataOffset = bs.Position;
            for (int i = 0; i < _modelSet.TextureSets.Count; i++)
            {
                TextureSet1 texSet = _modelSet.TextureSets[i];

                bs.Position = textureSetsOffset + i * sizeof(uint);
                bs.WriteUInt32((uint)(lastDataOffset - baseModelSetOffset));

                TexSetHashToOffset.TryAdd(texSet.GetHashCode(), (uint)lastDataOffset);

                bs.Position = lastDataOffset;
                texSet.Serialize(bs);
                bs.Align(0x40, grow: true);

                lastDataOffset = bs.Position;
            }

            bs.Position = baseModelSetOffset + 0x16;
            bs.WriteUInt16((ushort)_modelSet.TextureSets.Count);

            bs.Position = baseModelSetOffset + 0x2C;
            bs.WriteUInt32((uint)(textureSetsOffset - baseModelSetOffset));

            bs.Position = lastDataOffset;
        }

        private void WriteVariationTextureSets(BinaryStream bs, long baseModelSetOffset, long textureSetsOffset)
        {
            long lastDataOffset = bs.Position;
            for (int i = 0; i < _modelSet.VariationTexSet.Count; i++)
            {
                int listHash = GetTexSetListHash(_modelSet.VariationTexSet[i]);
                if (TexSetListHashToOffset.TryGetValue(listHash, out uint texSetTableOffset))
                {
                    bs.Position = textureSetsOffset + i * sizeof(uint);
                    bs.WriteUInt32((uint)(texSetTableOffset - baseModelSetOffset));
                }
                else
                {
                    bs.Position = textureSetsOffset + i * sizeof(uint);
                    bs.WriteUInt32((uint)(lastDataOffset - baseModelSetOffset));

                    List<TextureSet1> list = _modelSet.VariationTexSet[i];

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
                            bs.WriteUInt32((uint)(texSetOffset - baseModelSetOffset));
                        }
                        else
                        {
                            bs.Position = texOffsetTable + j * sizeof(uint);
                            bs.WriteUInt32((uint)(lastDataOffset - baseModelSetOffset));

                            bs.Position = lastDataOffset;
                            TexSetHashToOffset.Add(texSetHash, (uint)lastDataOffset);

                            texSet.Serialize(bs);
                            bs.Align(0x10, grow: true);

                            lastDataOffset = bs.Position;
                        }
                    }

                }
            }

            bs.Position = baseModelSetOffset + 0x18;
            bs.WriteUInt16((ushort)_modelSet.VariationTexSet.Count);

            if (_modelSet.VariationTexSet.Count > 0)
            {
                bs.Position = baseModelSetOffset + 0x34;
                bs.WriteUInt32((uint)(textureSetsOffset - baseModelSetOffset));
            }

            bs.Position = lastDataOffset;
        }

        private int GetTexSetListHash(List<TextureSet1> texSet1)
        {
            int hash = 0;
            foreach (var i in texSet1)
                hash += i.GetHashCode();

            return hash;
        }
    }
}
