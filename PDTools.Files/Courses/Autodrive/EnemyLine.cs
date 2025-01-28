using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.IO;

using Syroot.BinaryData;
using Syroot.BinaryData.Core;

namespace PDTools.Files.Courses.AutoDrive;

public class EnemyLine
{
    public const string Magic = "ADLN";

    public List<AutoDriveInfo> AutoDriveInfos { get; set; } = [];

    public static EnemyLine FromStream(BinaryStream bs)
    {
        long basePos = bs.Position;

        if (bs.ReadString(4) != Magic)
            throw new InvalidDataException("Not a valid ADLN header.");

        EnemyLine line = new EnemyLine();

        int relocPtr = bs.ReadInt32();
        bs.ReadInt32();
        int adlnFullSize = bs.ReadInt32();
        int unkCount = bs.ReadInt32();
        int autoDriveInfoCount = bs.ReadInt32();
        int autoDriveOffset = bs.ReadInt32();

        // List
        for (int i = 0; i < autoDriveInfoCount; i++)
        {
            bs.Position = (basePos + autoDriveOffset) + (sizeof(int) * i);
            int autoDriveInfoOffset = bs.ReadInt32();

            bs.Position = basePos + autoDriveInfoOffset;
            AutoDriveInfo driveInfo = AutoDriveInfo.FromStream(bs);
            line.AutoDriveInfos.Add(driveInfo);
        }

        return line;
    }
}
