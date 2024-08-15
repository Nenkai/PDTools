using PDTools.Files.Models.PS2.CarModel1;
using PDTools.Files.Models.PS2.ModelSet;
using Syroot.BinaryData;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDTools.Files.Models.PS2;

public class GT2KCarData
{
    public List<CarModel0> CarModels { get; set; } = [];

    public void FromStream(Stream stream)
    {
        long basePos = stream.Position;

        if (stream.Length < 0x20)
            throw new InvalidDataException("Not a valid GT2K car data file.");

        var bs = new BinaryStream(stream, ByteConverter.Little);
        bs.ReadUInt32(); // Reloc Ptr

        uint[] carModel0Offsets = bs.ReadUInt32s(6);

        for (int i = 0; i < carModel0Offsets.Length; i++)
        {
            bs.Position = basePos + (int)carModel0Offsets[i];
            var carModel0 = new CarModel0();
            carModel0.FromStream(bs);
            CarModels.Add(carModel0);
        }
    }
}
