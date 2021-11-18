using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Syroot.BinaryData;

namespace PDTools.Files.Courses.Runway
{
    public class RunwayCarLightSet
    {
        public List<ushort> LightDefIndices { get; set; } = new();

        public static RunwayCarLightSet FromStream(BinaryStream bs, ushort rwyVersionMajor, ushort rwyVersionMinor)
        {
            long basePos = bs.Position;
            RunwayCarLightSet lightSet = new();

            bs.Position -= 2;
            ushort count = bs.ReadUInt16();
            for (int i = 0; i < count; i++)
                lightSet.LightDefIndices.Add(bs.ReadUInt16());

            return lightSet;
        }

        public void ToStream(BinaryStream bs, ushort rwyVersionMajor, ushort rwyVersionMinor)
        {
            bs.WriteUInt16((ushort)LightDefIndices.Count);
            for (int i = 0; i < LightDefIndices.Count; i++)
                bs.WriteUInt16(LightDefIndices[i]);
        }
    }
}
