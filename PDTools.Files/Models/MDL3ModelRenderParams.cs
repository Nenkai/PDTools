using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;

using Syroot.BinaryData;

namespace PDTools.Files.Models
{
    /// <summary>
    /// Possibly directly passed to SPUs
    /// </summary>
    public class MDL3ModelRenderParams
    {
        public Vector3 Origin { get; set; }
        public List<Vector3> Bounds = new();

        public static MDL3ModelRenderParams FromStream(BinaryStream bs, long mdlBasePos, ushort mdl3VersionMajor)
        {
            MDL3ModelRenderParams set = new();

            bs.Position += 4;
            float x = bs.ReadSingle();
            float y = bs.ReadSingle();
            float z = bs.ReadSingle();
            set.Origin = new(x, y, z);

            bs.ReadByte(); // Unk
            bs.ReadByte(); // Unk
            int boundsCount = bs.ReadInt16();
            int boundsOffset = bs.ReadInt32();

            uint payloadOffset = bs.ReadUInt32();
            uint payloadLength = bs.ReadUInt32();

            // Possibly indices
            bs.ReadInt16();
            bs.ReadInt16();
            bs.ReadInt16();
            bs.ReadInt16(); // Unk

            for (int i = 0; i < boundsCount; i++)
            {
                bs.Position = mdlBasePos + boundsOffset + (i * 0xC);
                float bx = bs.ReadSingle();
                float by = bs.ReadSingle();
                float bz = bs.ReadSingle();
                set.Bounds.Add(new(bx, by, bz));
            }

            return set;
        }
    }
}
