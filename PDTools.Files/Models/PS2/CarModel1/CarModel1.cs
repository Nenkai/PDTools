using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

using Syroot.BinaryData;

using PDTools.Files.Models.PS2.ModelSet;

namespace PDTools.Files.Models.PS2.CarModel1
{
    public class CarModel1
    {
        public const int HeaderSize = 0x20;

        public CarInfo CarInfo { get; set; }
        public ModelSet1 ModelSet { get; set; }
        public TireFile Tire { get; set; }
        public WheelFile Wheel { get; set; }

        public void FromStream(Stream stream)
        {
            long basePos = stream.Position;

            if (stream.Length < HeaderSize)
                throw new InvalidDataException("Not a valid car model file.");

            var bs = new BinaryStream(stream, ByteConverter.Little);
            bs.ReadUInt32();

            uint carInfoOffset = bs.ReadUInt32();
            uint mainModelOffset = bs.ReadUInt32();
            uint tireOffset = bs.ReadUInt32();
            uint wheelOffset = bs.ReadUInt32();
            uint dummyOffset = bs.ReadUInt32();

            if (carInfoOffset != 0)
            {
                bs.Position = basePos + carInfoOffset;
                CarInfo = new CarInfo();
                CarInfo.FromStream(bs);
            }

            if (mainModelOffset != 0)
            {
                bs.Position = basePos + mainModelOffset;
                ModelSet = new ModelSet1();
                ModelSet.FromStream(bs);
            }

            if (tireOffset != 0)
            {
                bs.Position = basePos + tireOffset;
                Tire = new TireFile();
                Tire.FromStream(bs);
            }

            if (wheelOffset != 0)
            {
                bs.Position = basePos + wheelOffset;
                Wheel = new WheelFile();
                Wheel.FromStream(bs);
            }
        }

        public void Write(Stream stream)
        {
            long basePos = stream.Position;

            var bs = new BinaryStream(stream, ByteConverter.Little);
            bs.Position += 0x40; // Skip header for now

            long carInfoOffset = bs.Position;
            CarInfo.Write(bs);
            bs.Align(0x80, grow: true);

            long mainModelOffset = bs.Position;
            var modelSetSerializer = new ModelSet1Serializer(ModelSet);
            modelSetSerializer.Write(stream);
            bs.Align(0x40, grow: true);

            long tireOffset = bs.Position;
            Tire.Write(stream);
            bs.Align(0x40, grow: true);

            long wheelOffset = bs.Position;
            Wheel.Write(stream);
            bs.Align(0x40, grow: true);

            // Why not
            long unkOffset = bs.Position;
            bs.WriteString("GT3 Custom Car Models since 2023 by Nenkai :)", StringCoding.Raw);
            bs.Align(0x40, grow: true);

            // Header
            bs.Position = basePos;
            bs.WriteUInt32(0); // Reloc ptr
            bs.WriteUInt32((uint)(carInfoOffset - basePos));
            bs.WriteUInt32((uint)(mainModelOffset - basePos));
            bs.WriteUInt32((uint)(tireOffset - basePos));
            bs.WriteUInt32((uint)(wheelOffset - basePos));
            bs.WriteUInt32((uint)(unkOffset - basePos));

            // Done!
        }

        public static void Split(Stream inputStream, string outputDir)
        {
            if (inputStream.Length < HeaderSize)
                throw new InvalidDataException("Not a valid car model file.");

            Directory.CreateDirectory(outputDir);

            using var bs = new BinaryStream(inputStream);
            bs.ReadUInt32();

            uint carInfoOffset = bs.ReadUInt32();
            uint mainModelOffset = bs.ReadUInt32();
            uint tireOffset = bs.ReadUInt32();
            uint wheelOffset = bs.ReadUInt32();
            uint dummyOffset = bs.ReadUInt32();

            using (var fs = new FileStream(Path.Combine(outputDir, "car_info"), FileMode.Create))
            {
                bs.Position = carInfoOffset;
                bs.Position += 0x0C;
                uint carInfoSize = bs.ReadUInt32();
                bs.Position = carInfoOffset;

                byte[] data = bs.ReadBytes((int)carInfoSize);
                fs.Write(data);
            }

            using (var fs = new FileStream(Path.Combine(outputDir, "car_model.mdl"), FileMode.Create))
            {
                bs.Position = mainModelOffset;
                byte[] data = bs.ReadBytes((int)(tireOffset - mainModelOffset));
                fs.Write(data);
            }

            using (var fs = new FileStream(Path.Combine(outputDir, "tire.bin"), FileMode.Create))
            {
                bs.Position = tireOffset;
                bs.Position += 0x0C;
                uint tireSize = bs.ReadUInt32();
                bs.Position = carInfoOffset;

                byte[] data = bs.ReadBytes((int)tireSize);
                fs.Write(data);
            }

            using (var fs = new FileStream(Path.Combine(outputDir, "wheel.bin"), FileMode.Create))
            {
                bs.Position = wheelOffset;
                bs.Position += 0x0C;
                uint wheelSize = bs.ReadUInt32();
                bs.Position = wheelOffset;

                byte[] data = bs.ReadBytes((int)(wheelSize));
                fs.Write(data);
            }

            using (var fs = new FileStream(Path.Combine(outputDir, "dummy"), FileMode.Create))
            {
                bs.Position = dummyOffset;
                byte[] data = bs.ReadBytes((int)(bs.Length - dummyOffset));
                fs.Write(data);
            }
        }
    }
}
