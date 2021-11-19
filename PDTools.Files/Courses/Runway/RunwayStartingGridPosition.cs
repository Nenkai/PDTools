using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Syroot.BinaryData;

namespace PDTools.Files.Courses.Runway
{
    public class RunwayStartingGridPosition
    {
        public Vec3R Position { get; set; }

        public static RunwayStartingGridPosition FromStream(BinaryStream bs, ushort rwyVersionMajor, ushort rwyVersionMinor)
        {
            var pos = new RunwayStartingGridPosition();
            pos.Position = Vec3R.FromStream(bs);
            return pos;
        }

        public void ToStream(BinaryStream bs, ushort rwyVersionMajor, ushort rwyVersionMinor)
        {
            Position.ToStream(bs);
        }
    }
}
