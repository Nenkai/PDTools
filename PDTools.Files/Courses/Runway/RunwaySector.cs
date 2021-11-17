using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Syroot.BinaryData;
namespace PDTools.Files.Courses.Runway
{
    public class RunwaySector
    {
        public List<float> SectorVCoords { get; set; } = new();

        public static RunwaySector FromStream(BinaryStream bs, ushort rwyVersionMajor, ushort rwyVersionMinor)
        {
            long basePos = bs.Position;
            RunwaySector sector = new();

            int vCoordCount = bs.ReadInt32();
            int offset = bs.ReadInt32();

            bs.Position = offset;
            for (int i = 0; i < vCoordCount; i++)
                sector.SectorVCoords.Add(bs.ReadSingle());

            return sector;
        }

        public void ToStream(BinaryStream bs, ushort rwyVersionMajor, ushort rwyVersionMinor)
        {
            bs.WriteUInt32((uint)SectorVCoords.Count);
            bs.WriteUInt32((uint)bs.Position + 0x04);
            for (int i = 0; i < SectorVCoords.Count; i++)
            {
                bs.WriteSingle(SectorVCoords[i]);
            }
        }
    }
}
