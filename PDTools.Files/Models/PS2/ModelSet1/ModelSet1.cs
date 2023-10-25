using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

using Syroot.BinaryData;

using PDTools.Files.Textures.PS2;
using PDTools.Utils;

namespace PDTools.Files.Models.PS2.ModelSet1
{
    /// <summary>
    /// Model Set. Used by GT3/C and earlier
    /// </summary>
    public class ModelSet1
    {
        /// <summary>
        /// Magic - "GTM1".
        /// </summary>
        public const uint MAGIC = 0x314D5447;

        public List<ModelSet1Model> Models { get; set; } = new List<ModelSet1Model>();
        public List<PGLUshape> Shapes { get; set; } = new List<PGLUshape>();
        public List<PGLUmaterial> Materials { get; set; } = new List<PGLUmaterial>();
        public List<TextureSet1> TextureSets { get; set; } = new List<TextureSet1>();
        public List<ModelSet1Bounding> Boundings { get; set; } = new List<ModelSet1Bounding>();

        public void FromStream(Stream stream)
        {
            long basePos = stream.Position;

            using var bs = new BinaryStream(stream, ByteConverter.Little);

            if (bs.ReadUInt32() != MAGIC)
                throw new InvalidDataException("Not a model set stream.");

            bs.ReadUInt32(); // Reloc ptr
            bs.Position += 8; // Empty

            ushort modelCount = bs.ReadUInt16();
            ushort shapeCount = bs.ReadUInt16();
            ushort materialCount = bs.ReadUInt16();
            ushort texSetCount = bs.ReadUInt16();
            ushort unkCount = bs.ReadUInt16();
            ushort unkCount2 = bs.ReadUInt16();
            bs.Position += 4;

            uint modelTableOffset = bs.ReadUInt32();
            uint shapeTableOffset = bs.ReadUInt32();
            uint materialsOffset = bs.ReadUInt32();
            uint texSetTableOffset = bs.ReadUInt32();
            uint boundingsOffset = bs.ReadUInt32();
            uint unkOffset0x34 = bs.ReadUInt32(); // Boundings may be used if this is set maybe? GT3 EU: 0x2261b0
            uint unkOffset0x38 = bs.ReadUInt32();

            bs.Position = basePos + modelTableOffset;
            int[] modelOffsets = bs.ReadInt32s(modelCount);
            for (int i = 0; i < modelCount; i++)
            {
                bs.Position = basePos + modelOffsets[i];

                var model = new ModelSet1Model();
                model.FromStream(bs);
                Models.Add(model);
            }

            bs.Position = basePos + shapeTableOffset;
            int[] shapeOffsets = bs.ReadInt32s(shapeCount);
            for (int i = 0; i < shapeCount; i++)
            {
                bs.Position = basePos + shapeOffsets[i];

                var shape = new PGLUshape();
                shape.FromStream(bs, basePos);
                Shapes.Add(shape);
            }

            for (int i = 0; i < materialCount; i++)
            {
                bs.Position = basePos + materialsOffset + i * PGLUmaterial.GetSize();
                var material = new PGLUmaterial();
                material.FromStream(bs, basePos);
                Materials.Add(material);
            }

            bs.Position = basePos + texSetTableOffset;
            int[] texSetOffsets = bs.ReadInt32s(texSetCount);
            for (int i = 0; i < texSetCount; i++)
            {
                if (texSetOffsets[i] == 0)
                    continue;

                bs.Position = basePos + texSetOffsets[i];

                var texSet = new TextureSet1();
                texSet.FromStream(bs);
                TextureSets.Add(texSet);
            }

            for (int i = 0; i < modelCount; i++)
            {
                bs.Position = basePos + boundingsOffset + (i * ModelSet1Bounding.GetSize());

                var bounding = new ModelSet1Bounding();
                bounding.FromStream(bs);
                Boundings.Add(bounding);
            }
        }
    }
}
