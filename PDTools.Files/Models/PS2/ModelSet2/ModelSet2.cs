using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

using Syroot.BinaryData;

using PDTools.Files.Textures.PS2;

namespace PDTools.Files.Models.PS2.ModelSet2
{
    /// <summary>
    /// Model Set 2. Used by GT4
    /// </summary>
    public class ModelSet2
    {
        const string MAGIC = "MDLS";

        public RelocatorBase Relocator { get; set; }

        public List<ModelSet2Model> Models { get; set; } = new List<ModelSet2Model>();
        public List<PGLUshape> Shapes { get; set; } = new List<PGLUshape>();

        /// <summary>
        /// Textures. Each texture set within a list represents seemingly one lod level.
        /// </summary>
        public List<List<TextureSet1>> TextureSetLists { get; set; } = new List<List<TextureSet1>>();

        public ModelSet2 FromStream(Stream stream)
        {
            using var bs = new BinaryStream(stream);
            long basePos = bs.Position;

            string magic = bs.ReadString(4);
            if (magic != MAGIC)
                throw new InvalidDataException("Not a valid ModelSet2 stream.");

            /* HEADER - 0xE4 */
            ModelSet2 modelSet = new();

            int relocatorInfoOffset = bs.ReadInt32();
            int relocatorDataSize = bs.ReadInt32();
            int relocationBase = bs.ReadInt32();
            int fileSize = bs.ReadInt32();

            ushort unk = bs.ReadUInt16();
            ushort modelCount = bs.ReadUInt16();
            ushort shapeCount = bs.ReadUInt16();
            ushort pgluMatTableCount = bs.ReadUInt16();
            ushort textureSetLodLevelCount = bs.ReadUInt16();
            ushort textureSetListCount = bs.ReadUInt16();

            bs.Position = basePos + 0x38;
            uint modelsOffset = bs.ReadUInt32();
            uint shapesOffset = bs.ReadUInt32();
            uint pgluMatTableOffset = bs.ReadUInt32();
            uint pgluTexSetsOffset = bs.ReadUInt32();

            bs.Position = basePos + shapesOffset;

            bs.Position = basePos + relocatorInfoOffset;
            Relocator = RelocatorBase.FromStream(bs, basePos);

            ReadModels(bs, basePos, modelsOffset, modelCount);
            ReadShapes(bs, basePos, shapesOffset, shapeCount);
            ReadTextureSets(bs, basePos, pgluTexSetsOffset, textureSetListCount, textureSetLodLevelCount);


            return modelSet;
        }

        private void ReadModels(BinaryStream bs, long baseMdlPos, uint offset, uint count)
        {
            for (ushort i = 0; i < count; i++)
            {
                bs.Position = baseMdlPos + offset + i * ModelSet2Model.GetSize();

                var model = ModelSet2Model.FromStream(bs, baseMdlPos);
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
    }
}
