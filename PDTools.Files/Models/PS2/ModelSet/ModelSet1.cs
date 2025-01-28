using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Syroot.BinaryData;
using System.Numerics;
using System.Runtime.InteropServices;

using PDTools.Files.Textures.PS2;
using PDTools.Utils;
using PDTools.Files.Models.PS2.Commands;

using SixLabors.ImageSharp;
using System.Reflection;
using System.Xml.Linq;
using PDTools.Files.Models.PS3.ModelSet3;

namespace PDTools.Files.Models.PS2.ModelSet;

/// <summary>
/// Model Set. Used by GT3/C
/// </summary>
public class ModelSet1 : ModelSetPS2Base
{
    /// <summary>
    /// Magic - "GTM1".
    /// </summary>
    public const uint MAGIC = 0x314D5447;

    /// <summary>
    /// Materials, for meshes.
    /// </summary>
    public List<PGLUmaterial> Materials { get; set; } = [];

    /// <summary>
    /// Texture sets for this model.
    /// </summary>
    public List<TextureSet1> TextureSets { get; set; } = [];

    public List<ModelSet1Bounding> Boundings { get; set; } = [];

    /// <summary>
    /// Texture set for each variation - car color.
    /// </summary>
    public List<List<TextureSet1>> VariationTexSet { get; set; } = [];

    /// <summary>
    /// Materials per car variation - car color.
    /// </summary>
    public List<List<PGLUmaterial>> VariationMaterialsTable { get; set; } = [];

    public void FromStream(Stream stream)
    {
        long basePos = stream.Position;

        var bs = new BinaryStream(stream, ByteConverter.Little);

        if (bs.ReadUInt32() != MAGIC)
            throw new InvalidDataException("Not a model set stream.");

        bs.ReadUInt32(); // Reloc ptr
        bs.Position += 4; // Empty
        bs.Position += 4; // Empty

        ushort modelCount = bs.ReadUInt16();
        ushort shapeCount = bs.ReadUInt16();
        ushort materialCount = bs.ReadUInt16();
        ushort texSetCount = bs.ReadUInt16();
        ushort variationTexSetCount = bs.ReadUInt16();
        ushort variationMaterialsCount = bs.ReadUInt16();
        bs.Position += 4;

        uint modelTableOffset = bs.ReadUInt32();
        uint shapeTableOffset = bs.ReadUInt32();
        uint materialsOffset = bs.ReadUInt32();
        uint texSetTableOffset = bs.ReadUInt32();
        uint boundingsOffset = bs.ReadUInt32();
        uint variationTexSetTableOffset = bs.ReadUInt32(); // Boundings may be used if this is set maybe? GT3 EU: 0x2261b0
        uint variationMaterialsOffset = bs.ReadUInt32();

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
            shape.FromStream(bs);
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

        bs.Position = basePos + variationTexSetTableOffset;
        int[] variationTexSetOffsets = bs.ReadInt32s(variationTexSetCount);
        for (int i = 0; i < variationTexSetCount; i++)
        {
            if (variationTexSetOffsets[i] == 0)
                continue;

            bs.Position = basePos + variationTexSetOffsets[i];
            int[] texSetsOffsets = bs.ReadInt32s(texSetCount);

            List<TextureSet1> texSets = [];
            for (int j = 0; j < texSetCount; j++)
            {
                bs.Position = basePos + texSetsOffsets[j];
                var texSet = new TextureSet1();
                texSet.FromStream(bs);
                texSets.Add(texSet);
            }

            VariationTexSet.Add(texSets);
        }

        for (int i = 0; i < modelCount; i++)
        {
            bs.Position = basePos + boundingsOffset + i * ModelSet1Bounding.GetSize();

            var bounding = new ModelSet1Bounding();
            bounding.FromStream(bs);
            Boundings.Add(bounding);
        }

        bs.Position = basePos + variationMaterialsOffset;
        int[] materialVariationOffsets = bs.ReadInt32s(variationMaterialsCount);
        for (int i = 0; i < variationMaterialsCount; i++)
        {
            bs.Position = basePos + materialVariationOffsets[i];

            List<PGLUmaterial> materialsForThisVariation = new List<PGLUmaterial>();
            for (int j = 0; j < materialCount; j++)
            {
                var material = new PGLUmaterial();
                material.FromStream(bs, basePos);
                materialsForThisVariation.Add(material);
            }

            VariationMaterialsTable.Add(materialsForThisVariation);
        }
    }

    public override int GetNumModels()
    {
        return Models.Count;
    }

    public override List<TextureSet1> GetTextureSetList()
    {
        return TextureSets;
    }

    public override uint AddShape(PGLUshape shape)
    {
        Shapes.Add(shape);
        return (uint)(Shapes.Count - 1);
    }

    public override PGLUshape GetShape(int shapeIndex)
    {
        return Shapes[shapeIndex];
    }

    public override int GetNumShapes()
    {
        return Shapes.Count;
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

    public override List<PGLUmaterial> GetVariationMaterials(int varIndex)
    {
        return VariationMaterialsTable[varIndex];
    }

    public override int GetNumVariations()
    {
        return Math.Max(TextureSets[0].ClutPatchSet.Count, 1);
    }
}
