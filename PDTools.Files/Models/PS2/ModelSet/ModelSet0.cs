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
/// Model Set. Used by GT2K
/// </summary>
public class ModelSet0 : ModelSetPS2Base
{
    /// <summary>
    /// Magic - "GTM0".
    /// </summary>
    public const uint MAGIC = 0x304D5447;

    /// <summary>
    /// Materials, for meshes.
    /// </summary>
    public List<PGLUmaterial> Materials { get; set; } = [];

    public void FromStream(Stream stream)
    {
       uint basePos = (uint)stream.Position;

        var bs = new BinaryStream(stream, ByteConverter.Little);

        if (bs.ReadUInt32() != MAGIC)
            throw new InvalidDataException("Not a model set 0 stream.");

        bs.ReadUInt32(); // Reloc ptr

        ushort modelCount = bs.ReadUInt16();
        ushort materialCount = bs.ReadUInt16();
        uint modelTableOffset = bs.ReadUInt32();
        uint materialsOffset = bs.ReadUInt32();

        for (int i = 0; i < modelCount; i++)
        {
            bs.Position = basePos + modelTableOffset + (i * ModelSet0Model.GetSize());

            var model = new ModelSet0Model();
            model.FromStream(bs, basePos);
            Models.Add(model);
        }

        for (int i = 0; i < materialCount; i++)
        {
            bs.Position = basePos + materialsOffset + i * PGLUmaterial.GetSize();
            var material = new PGLUmaterial();
            material.FromStream(bs, basePos);
            Materials.Add(material);
        }
    }

    public override int GetNumModels()
    {
        return Models.Count;
    }

    public override List<TextureSet1> GetTextureSetList()
    {
        throw new NotSupportedException("Not supported by ModelSet0.");
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

    /// <summary>
    /// Not supported
    /// </summary>
    /// <param name="varIndex"></param>
    /// <returns></returns>
    /// <exception cref="NotSupportedException"></exception>
    public override List<PGLUmaterial> GetVariationMaterials(int varIndex)
    {
        throw new NotSupportedException("Not supported by ModelSet0.");
    }

    /// <summary>
    /// Not supported
    /// </summary>
    /// <param name="varIndex"></param>
    /// <returns></returns>
    /// <exception cref="NotSupportedException"></exception>
    public override int GetNumVariations()
    {
        throw new NotSupportedException("Not supported by ModelSet0.");
    }
}
