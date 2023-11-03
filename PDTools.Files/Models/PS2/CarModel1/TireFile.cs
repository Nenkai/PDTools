using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

using Syroot.BinaryData;

using PDTools.Files.Textures.PS2;

namespace PDTools.Files.Models.PS2.CarModel1
{
    /// <summary>
    /// GT3 Tire File (GTTR)
    /// </summary>
    public class TireFile
    {
        /// <summary>
        /// "GTTR" GT Tire
        /// </summary>
        public const uint MAGIC = 0x52545447;

        // Header size is 0x20

        public TextureSet1 TextureSet { get; set; } = new TextureSet1();

        public void FromStream(Stream stream)
        {
            long basePos = stream.Position;

            var bs = new BinaryStream(stream, ByteConverter.Little);

            uint magic = bs.ReadUInt32();
            if (magic != MAGIC)
                throw new InvalidDataException("Not a tire file (GTTR).");

            bs.ReadUInt32(); // Reloc ptr
            bs.ReadUInt32(); // Empty
            uint fileSize = bs.ReadUInt32();

            bs.Position = basePos + 0x1C;
            uint texSetOffset = bs.ReadUInt32();
            bs.Position = basePos + texSetOffset;

            TextureSet.FromStream(stream);
        }

        public void Write(Stream stream)
        {
            long basePos = stream.Position;

            var bs = new BinaryStream(stream, ByteConverter.Little);
            stream.Position = basePos + 0x20;
            TextureSet.Serialize(stream);
            long lastPos = bs.Position;

            bs.Position = basePos;
            bs.WriteUInt32(MAGIC);
            bs.WriteUInt32(0);
            bs.WriteUInt32(0);
            bs.WriteUInt32((uint)(lastPos - basePos));

            bs.Position = basePos + 0x1C;
            bs.WriteUInt32(0x20); // Offset of texture set

            bs.Position = lastPos;
        }
    }
}
