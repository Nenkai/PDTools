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
    public class ModelSet2Serializer
    {
        private ModelSet2 _modelSet { get; set; }

        private Dictionary<int, uint> TexSetHashToOffset = new Dictionary<int, uint>();
        private Dictionary<int, uint> TexSetListHashToOffset = new Dictionary<int, uint>();

        public ModelSet2Serializer(ModelSet2 modelSet)
        {
            _modelSet = modelSet;
        }

        
        public void Write(Stream stream)
        {
            BinaryStream bs = new BinaryStream(stream, ByteConverter.Little);

            long basePos = bs.Position;

            // Skip header for now
            bs.Position = basePos + 0xA0;

            WriteMaterials(bs, basePos);
            WriteVariationMaterials(bs, basePos);
            //WriteModels(bs, basePos);

            // Finish header.
            bs.Position = basePos;
            bs.WriteUInt32(ModelSet1.MAGIC);

            //bs.Position = lastPos;
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

            bs.Position = baseModelSetPos + 0x40;
            bs.WriteUInt32((uint)(materialsOffset - baseModelSetPos));

            bs.Position = lastPos;
        }

        private void WriteVariationMaterials(BinaryStream bs, long baseModelSetPos)
        {
            long variationMaterialsOffset = bs.Position;

            for (int i = 0; i < _modelSet.Materials.Count; i++)
            {
                PGLUmaterial material = _modelSet.Materials[i];
                material.Write(bs);
            }

            long lastPos = bs.Position;
            bs.Position = baseModelSetPos + 0x60;
            bs.WriteUInt32((uint)(variationMaterialsOffset - baseModelSetPos));

            bs.Position = lastPos;
        }

        private void WriteModels(BinaryStream bs, long baseModelSetOffset, long modelTableOffset)
        {
            long lastDataOffset = bs.Position;
            for (int i = 0; i < _modelSet.Models.Count; i++)
            {
                bs.Position = baseModelSetOffset + modelTableOffset + (i * ModelSet2Model.GetSize());
                ModelSet2Model model = _modelSet.Models[i];
                model.Write(bs);

                lastDataOffset = bs.Position;
            }

            bs.Align(0x10, grow: true);
            lastDataOffset = bs.Position;

            bs.Position = baseModelSetOffset + 0x38;
            bs.WriteUInt16((ushort)_modelSet.Models.Count);

            bs.Position = baseModelSetOffset + 0x16;
            bs.WriteUInt32((uint)(modelTableOffset - baseModelSetOffset));

            bs.Position = lastDataOffset;
        }


        /*
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

        private static int GetMatTableHashCode(List<PGLUmaterial> mat)
        {
            int hash = 0;
            for (int i = 0; i < mat.Count; i++)
                hash += mat[i].GetHashCode();
            return hash;
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
        */
    }
}
