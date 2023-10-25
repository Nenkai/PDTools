using Syroot.BinaryData;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;

namespace PDTools.Files.Models.PS3.ModelSet3
{
    public class MDL3Bone
    {
        public Matrix4x4 Matrix { get; set; }
        public string Name { get; set; }
        public short ParentID { get; set; }
        public short ID { get; set; }

        public static MDL3Bone FromStream(BinaryStream bs, long basePos)
        {
            var entry = new MDL3Bone();

            entry.Matrix = new Matrix4x4(
                   bs.ReadSingle(), bs.ReadSingle(), bs.ReadSingle(), bs.ReadSingle(),
                   bs.ReadSingle(), bs.ReadSingle(), bs.ReadSingle(), bs.ReadSingle(),
                   bs.ReadSingle(), bs.ReadSingle(), bs.ReadSingle(), bs.ReadSingle(),
                   bs.ReadSingle(), bs.ReadSingle(), bs.ReadSingle(), bs.ReadSingle()
            );

            int nameOffset = bs.ReadInt32();
            entry.ParentID = bs.ReadInt16();
            entry.ID = bs.ReadInt16();

            bs.Position = basePos + nameOffset;
            entry.Name = bs.ReadString(StringCoding.ZeroTerminated);


            return entry;
        }

        public override string ToString()
        {
            return $"{Name} (Unk: {ParentID}, ID: {ID})";
        }

        public static int GetSize()
        {
            return 0x48;
        }
    }
}
