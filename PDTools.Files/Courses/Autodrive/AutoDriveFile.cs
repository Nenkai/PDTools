using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

using Syroot.BinaryData;
using Syroot.BinaryData.Core;

namespace PDTools.Files.Courses.AutoDrive;

public class AutoDriveFile
{
    public const int HeaderAlignment = 0x80;

    public EnemyLine EnemyLine { get; set; }

    public static AutoDriveFile FromStream(BinaryStream bs)
    {
        AutoDriveFile ad = new AutoDriveFile();
        uint relocPtr = bs.ReadUInt32(); // If not 0, memory file most likely
        int enemyLineHeaderOffset = bs.ReadInt32();

        uint unkOffset1 = bs.ReadUInt32();
        uint unkOffset2 = bs.ReadUInt32();
        uint unkOffset3 = bs.ReadUInt32();
        uint unkOffset4 = bs.ReadUInt32();

        bs.Align(HeaderAlignment);

        if (bs.Position != enemyLineHeaderOffset)
        {
            Debug.WriteLine($"Abnormal padding for AutoDrive file header (ADLN expected at 0x40, got {enemyLineHeaderOffset})");
            bs.Position = enemyLineHeaderOffset;
            ad.EnemyLine = EnemyLine.FromStream(bs);
        }

        ad.EnemyLine = EnemyLine.FromStream(bs);
        return ad;
    }
}
