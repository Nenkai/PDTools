using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Syroot.BinaryData;

namespace PDTools.Files.Courses.Runway;

public class RunwayCarLightSetCollection
{
    public List<RunwayCarLightSet> Sets { get; set; } = [];

    public static RunwayCarLightSetCollection FromStream(BinaryStream bs, long count, ushort rwyVersionMajor, ushort rwyVersionMinor)
    {
        RunwayCarLightSetCollection collection = new RunwayCarLightSetCollection();

        long basePos = bs.Position;
        for (int i = 0; i < count; i++)
        {
            bs.Position = basePos + (i * sizeof(uint));
            int offset = bs.ReadInt32();
            if (offset == 0)
                continue;

            bs.Position = offset;
            RunwayCarLightSet lightSet = RunwayCarLightSet.FromStream(bs, rwyVersionMajor, rwyVersionMinor);
            collection.Sets.Add(lightSet);
        }

        return collection;
    }

    public void ToStream(BinaryStream bs, ushort rwyVersionMajor, ushort rwyVersionMinor)
    {
        if (Sets.Count == 0)
            return;

        long basePos = bs.Position;
        long lastDataPos = bs.Position + 
                           sizeof(int) + (Sets.Count * sizeof(int)) // First 0 offset + entire offset list
                           + 4; // first 4 byte of data is 0

        bs.WriteUInt32(0);

        for (int i = 0; i < Sets.Count; i++)
        {
            RunwayCarLightSet lightSet = Sets[i];

            bs.Position = basePos + (i * sizeof(int)) + sizeof(int);
            bs.WriteUInt32((uint)lastDataPos + 2);

            bs.Position = lastDataPos;
            lightSet.ToStream(bs, rwyVersionMajor, rwyVersionMinor);

            lastDataPos = bs.Position;
        }
    }
}
