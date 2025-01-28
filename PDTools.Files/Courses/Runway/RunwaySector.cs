using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Syroot.BinaryData;

namespace PDTools.Files.Courses.Runway;

public class RunwaySector
{
    public List<float> SectorVCoords { get; set; } = [];

    public static RunwaySector FromStream(BinaryStream bs, ushort rwyVersionMajor, ushort rwyVersionMinor)
    {
        long basePos = bs.Position;
        RunwaySector sector = new();

        long vCoordCount = RunwayFile.Read32Or64(bs, rwyVersionMajor);
        long offset = RunwayFile.Read32Or64(bs, rwyVersionMajor);

        bs.Position = offset;
        for (int i = 0; i < vCoordCount; i++)
            sector.SectorVCoords.Add(bs.ReadSingle());

        return sector;
    }

    public void ToStream(BinaryStream bs, ushort rwyVersionMajor, ushort rwyVersionMinor)
    {
        for (int i = 0; i < SectorVCoords.Count; i++)
        {
            bs.WriteSingle(SectorVCoords[i]);
        }
    }
}
