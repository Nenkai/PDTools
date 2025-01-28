using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

using Syroot.BinaryData;

using PDTools.Files.Models.PS2.ModelSet;
using PDTools.Files.Textures.PS2;

namespace PDTools.Files.Models.PS2.CarModel1;

public class CarModel0
{
    public const int HeaderSize = 0x20;

    public ModelSet0 ModelSet { get; set; }
    public UTextureSet TextureSet { get; set; } = new();

    public void FromStream(Stream stream)
    {
        long basePos = stream.Position;

        if (stream.Length < HeaderSize)
            throw new InvalidDataException("Not a valid car model file.");

        var bs = new BinaryStream(stream, ByteConverter.Little);
        bs.ReadUInt32();

        uint mainModelOffset = bs.ReadUInt32();
        uint texturesOffset = bs.ReadUInt32();
        uint unkOffset3 = bs.ReadUInt32();
        uint unkOffset4 = bs.ReadUInt32();
        uint unkOffset5 = bs.ReadUInt32();

        if (mainModelOffset != 0)
        {
            bs.Position = basePos + mainModelOffset;
            ModelSet = new ModelSet0();
            ModelSet.FromStream(bs);
        }

        if (texturesOffset != 0)
        {
            bs.Position = basePos + texturesOffset;
            UTextureSet utexSet = new UTextureSet();
            utexSet.FromStream(bs);
            TextureSet = utexSet;
        }
    }
}
