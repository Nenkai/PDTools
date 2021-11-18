using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Syroot.BinaryData;

namespace PDTools.Files.Courses.Runway
{
    public class RunwayRayCastData
    {
        public uint? MainRoadTriIndex { get; set; }

        public List<uint> RoadTriIndices { get; set; } = new();

        public void ToStream(BinaryStream bs)
        {
            bs.WriteUInt32(MainRoadTriIndex.Value);

            for (int j = 0; j < RoadTriIndices.Count; j++)
            {
                // Encode indices
                uint index = RoadTriIndices[j];
                if (index < 0x80)
                {
                    bs.WriteByte((byte)index);
                }
                else if (index <= 0x3FFF)
                {
                    byte[] val = BitConverter.GetBytes(index);

                    bs.WriteByte((byte)(0x80 + val[1]));
                    bs.WriteByte(val[0]);
                }
                else
                {
                    byte[] val = BitConverter.GetBytes(index);
                    bs.WriteByte((byte)(0xC0 + val[3]));
                    bs.WriteByte(val[2]);
                    bs.WriteByte(val[1]);
                    bs.WriteByte(val[0]);
                }

                // Data entries are not aligned, all in a row
            }

            // End terminator
            bs.WriteByte(0);
        }
    }
}
