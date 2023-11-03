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
    /// <summary>
    /// GT3 Wheel File (GTTW)
    /// </summary>
    public class WheelFile
    {
        /// <summary>
        /// "GTTW" GT ? Wheel
        /// </summary>
        public const uint MAGIC = 0x57545447;

        // Header size is 0x20

        public ModelSet1 ModelSet { get; set; } = new ModelSet1();

        public void FromStream(Stream stream)
        {
            long basePos = stream.Position;

            var bs = new BinaryStream(stream, ByteConverter.Little);

            uint magic = bs.ReadUInt32();
            if (magic != MAGIC)
                throw new InvalidDataException("Not a wheel file (GTTW).");

            bs.ReadUInt32(); // Reloc ptr
            bs.ReadUInt32(); // Empty
            uint fileSize = bs.ReadUInt32();

            bs.Position = basePos + 0x1C;
            uint texSetOffset = bs.ReadUInt32();
            bs.Position = basePos + texSetOffset;

            ModelSet.FromStream(stream);
        }

        public void Write(Stream stream)
        {
            long basePos = stream.Position;

            var bs = new BinaryStream(stream, ByteConverter.Little);
            stream.Position = basePos + 0x20;

            var modelSet1Serializer = new ModelSet1Serializer(ModelSet);
            modelSet1Serializer.Write(stream);

            long lastPos = bs.Position;

            bs.Position = basePos;
            bs.WriteUInt32(MAGIC);
            bs.WriteUInt32(0);
            bs.WriteUInt32(0);
            bs.WriteUInt32((uint)(lastPos - basePos));

            bs.Position = basePos + 0x1C;
            bs.WriteUInt32(0x20); // Offset of model set

            bs.Position = lastPos;
        }
    }
}
