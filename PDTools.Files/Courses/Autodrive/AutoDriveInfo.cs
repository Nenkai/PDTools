using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.IO;

using Syroot.BinaryData;
using Syroot.BinaryData.Core;

namespace PDTools.Files.Courses.AutoDrive
{
    public class AutoDriveInfo
    {
        public List<AttackInfo> AttackInfos { get; set; } = new();

        public static AutoDriveInfo FromStream(BinaryStream bs)
        {
            AutoDriveInfo adInfo = new AutoDriveInfo();

            long basePos = bs.Position;
            int attackInfoCount = bs.ReadInt32();

            long dataOffset = basePos + 0x40;
            for (int i = 0; i < attackInfoCount; i++)
            {
                bs.Position = dataOffset + (i * 0x40);
                AttackInfo attackInfo = AttackInfo.FromStream(bs);
                adInfo.AttackInfos.Add(attackInfo);
            }

            return adInfo;
        }
    }
}
