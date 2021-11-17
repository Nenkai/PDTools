using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.IO;

using System.Numerics;
using Syroot.BinaryData;
using Syroot.BinaryData.Core;

namespace PDTools.Files.Courses.AutoDrive
{
    public class AttackInfo
    {
        public bool UnkBool { get; set; }
        public Vector3 Position { get; set; }

        public short UnkIndex { get; set; }
        public short UnkIndex2 { get; set; }
        public float Unk1 { get; set; }
        public float Unk2 { get; set; }
        public float X2 { get; set; }
        public float Y2 { get; set; }
        public float Angle { get; set; }

        public const int StrideSize = 0x40;

        public static AttackInfo FromStream(BinaryStream bs)
        {
            long basePos = bs.Position;

            AttackInfo atkInfo = new AttackInfo();
            atkInfo.UnkBool = bs.ReadBoolean(BooleanCoding.Dword);
            atkInfo.Position = new Vector3(bs.ReadSingle(), bs.ReadSingle(), bs.ReadSingle());
            atkInfo.UnkIndex = bs.ReadInt16();
            atkInfo.UnkIndex2 = bs.ReadInt16();
            atkInfo.Unk1 = bs.ReadSingle();
            atkInfo.Unk2 = bs.ReadSingle();
            atkInfo.X2 = bs.ReadSingle();
            atkInfo.Y2 = bs.ReadSingle();
            atkInfo.Angle = bs.ReadSingle();

            return atkInfo;
        }
    }
}
