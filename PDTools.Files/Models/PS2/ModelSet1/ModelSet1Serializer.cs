using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

using Syroot.BinaryData;

using PDTools.Files.Textures.PS2;

namespace PDTools.Files.Models.PS2.ModelSet1
{
    public class ModelSet1Serializer
    {
        private ModelSet1 _modelSet { get; set; }

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

            // Skip tex set table for now
            long texSetTableOffset = bs.Position;
            bs.Position += _modelSet.TextureSets.Count * sizeof(uint);
            bs.Align(0x10, grow: true);

            WriteTextureSets(bs, basePos, texSetTableOffset);

            // Finish header.
            bs.Position = basePos;
            bs.WriteUInt32(ModelSet1.MAGIC);
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
            bs.WriteUInt32((uint)materialsOffset);

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
            bs.WriteUInt32((uint)boundingsOffset);

            bs.Position = lastPos;
        }

        private void WriteModels(BinaryStream bs, long baseModelSetOffset, long modelTableOffset)
        {
            long lastDataOffset = bs.Position;
            for (int i = 0; i < _modelSet.Models.Count; i++)
            {
                bs.Position = modelTableOffset + (i * sizeof(uint));
                bs.WriteUInt32((uint)(lastDataOffset - baseModelSetOffset));

                bs.Position = lastDataOffset;
                ModelSet1Model model = _modelSet.Models[i];
                model.Write(bs);

                lastDataOffset = bs.Position;
            }
            bs.Align(0x10, grow: true);
            lastDataOffset = bs.Position;

            bs.Position = baseModelSetOffset + 0x10;
            bs.WriteUInt16((ushort)_modelSet.Models.Count);

            bs.Position = baseModelSetOffset + 0x20;
            bs.WriteUInt32((uint)modelTableOffset);

            bs.Position = lastDataOffset;
        }

        private void WriteShapes(BinaryStream bs, long baseModelSetOffset, long shapeTableOffset)
        {
            long lastDataOffset = bs.Position;
            for (int i = 0; i < _modelSet.Shapes.Count; i++)
            {
                bs.Position = shapeTableOffset + (i * sizeof(uint));
                bs.WriteUInt32((uint)(lastDataOffset - baseModelSetOffset));

                bs.Position = lastDataOffset;
                PGLUshape shape = _modelSet.Shapes[i];
                shape.Write(bs, baseModelSetOffset);

                lastDataOffset = bs.Position;
            }

            bs.Position = baseModelSetOffset + 0x12;
            bs.WriteUInt16((ushort)_modelSet.Shapes.Count);

            bs.Position = baseModelSetOffset + 0x24;
            bs.WriteUInt32((uint)shapeTableOffset);

            bs.Position = lastDataOffset;
        }

        private void WriteTextureSets(BinaryStream bs, long baseModelSetOffset, long textureSetsOffset)
        {
            long lastDataOffset = bs.Position;
            for (int i = 0; i < _modelSet.TextureSets.Count; i++)
            {
                bs.Position = textureSetsOffset + (i * sizeof(uint));
                bs.WriteUInt32((uint)(lastDataOffset - baseModelSetOffset));

                bs.Position = lastDataOffset;
                TextureSet1 texSet = _modelSet.TextureSets[i];
                texSet.Serialize(bs);

                lastDataOffset = bs.Position;
            }

            bs.Position = baseModelSetOffset + 0x16;
            bs.WriteUInt16((ushort)_modelSet.TextureSets.Count);

            bs.Position = baseModelSetOffset + 0x2C;
            bs.WriteUInt32((uint)textureSetsOffset);

            bs.Position = lastDataOffset;
        }
    }
}
