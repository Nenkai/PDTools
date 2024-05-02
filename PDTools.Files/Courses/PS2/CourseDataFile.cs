using Syroot.BinaryData;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDTools.Files.Courses.PS2;

public class CourseDataFile
{
    /// <summary>
    /// Unfinished, not to be used (doesn't read past header)
    /// </summary>
    /// <param name="stream"></param>
    /// <returns></returns>
    /// <exception cref="InvalidDataException"></exception>
    public static CourseDataFile Open(string file)
    {
        using var fs = File.OpenRead(file);
        return FromStream(fs);

    }

    /// <summary>
    /// Unfinished, not to be used (doesn't read past header)
    /// </summary>
    /// <param name="stream"></param>
    /// <returns></returns>
    /// <exception cref="InvalidDataException"></exception>
    public static CourseDataFile FromStream(Stream stream)
    {
        long basePos = stream.Position;
        BinaryStream bs = new BinaryStream(stream);

        bs.ByteConverter = ByteConverter.Little;

        CourseDataFile courseData = new CourseDataFile();

        uint relocPtr = bs.ReadUInt32();

        bs.Position = basePos + 0x04;
        uint world_ = bs.ReadUInt32();                  // World ModelSet - Has explicit assert if missing

        bs.Position = basePos + 0x08;
        uint visionlist_ = bs.ReadUInt32();             // Environment VisionList - Has explicit assert if missing

        bs.Position = basePos + 0x0C;
        uint visionlist_back_mirror_ = bs.ReadUInt32(); // Back Mirror VisionList - Has explicit assert if missing

        bs.Position = basePos + 0x10;
        uint visionlist_2p_ = bs.ReadUInt32();          // 2 Player mode VisionList - Has explicit assert if missing

        bs.Position = basePos + 0x14;
        uint env_ = bs.ReadUInt32();                    // Environment ModelSet - Has explicit assert if missing

        bs.Position = basePos + 0x18;
        uint envvisionlist_ = bs.ReadUInt32();          // Environment VisionList - Has explicit assert if missing

        bs.Position = basePos + 0x1C;
        uint unkVisionList0x1C_ = bs.ReadUInt32();

        bs.Position = basePos + 0x20;
        uint unkVisionList0x20_ = bs.ReadUInt32();

        bs.Position = basePos + 0x24;
        uint unkModelSet0x24_ = bs.ReadUInt32();

        bs.Position = basePos + 0x28;
        uint unkVisionList0x28_ = bs.ReadUInt32();

        bs.Position = basePos + 0x34;
        uint unkModelSet0x34_ = bs.ReadUInt32();

        bs.Position = basePos + 0x38;
        uint unkVisionList0x38_ = bs.ReadUInt32();

        bs.Position = basePos + 0x44;
        uint unkModelSet0x44_ = bs.ReadUInt32();

        bs.Position = basePos + 0x48;
        uint unkVisionList0x48_ = bs.ReadUInt32();

        bs.Position = basePos + 0x4C;
        uint unkVisionList0x4C_ = bs.ReadUInt32();

        bs.Position = basePos + 0x50;
        uint unkVisionList0x50_ = bs.ReadUInt32();

        bs.Position = basePos + 0x54;
        uint sky_ = bs.ReadUInt32();          // ModelSet - Has explicit assert if missing

        bs.Position = basePos + 0x58;
        uint far_ = bs.ReadUInt32();          // ModelSet - Has explicit assert if missing

        bs.Position = basePos + 0x5C;
        uint envsky_ = bs.ReadUInt32();       // ModelSet - Has explicit assert if missing

        bs.Position = basePos + 0x60;
        uint mirror_sky_ = bs.ReadUInt32();   // ModelSet - Has explicit assert if missing

        bs.Position = basePos + 0x64;
        uint unk0x64_ = bs.ReadUInt32();

        bs.Position = basePos + 0x68;
        uint texture_set_0x68_ = bs.ReadUInt32();  // pgluTexSet

        bs.Position = basePos + 0x70;
        uint runway_ = bs.ReadUInt32();       // RunwayGT4 - Has explicit assert if missing

        bs.Position = basePos + 0x74;
        uint course_efx_ = bs.ReadUInt32();   // DCourseEffect Has explicit assert if missing

        bs.Position = basePos + 0x78;
        uint billboard_set_ = bs.ReadUInt32();

        bs.Position = basePos + 0x7C;
        uint unk0x7C_ = bs.ReadUInt32();

        bs.Position = basePos + 0x80;
        uint envptr_ = bs.ReadUInt32();       // CourseEnvPtr - Environment Parameter - Has explicit assert if missing

        bs.Position = basePos + 0x84;
        uint minimapset_ = bs.ReadUInt32();   // MiniMapSet - Has explicit assert if missing

        bs.Position = basePos + 0x88;
        uint texture_set_0x88_ = bs.ReadUInt32();  // pgluTexSet

        bs.Position = basePos + 0x8C;
        uint sound_ = bs.ReadUInt32();        // CourseSound - Has explicit assert if missing

        bs.Position = basePos + 0x94;
        uint shape_0x94_ = bs.ReadUInt32();  // pgluShape

        bs.Position = basePos + 0x98;
        uint texture_set_0x98_ = bs.ReadUInt32();  // pgluTexSet

        bs.Position = basePos + 0x9C;
        uint texture_set_0x9C_ = bs.ReadUInt32();  // pgluTexSet

        bs.Position = basePos + 0xA0;
        uint texture_set_0xA0_ = bs.ReadUInt32();  // pgluTexSet

        bs.Position = basePos + 0xA4;
        uint unkGt4ReplayData0xA4_ = bs.ReadUInt32();  // GT4ReplayData

        bs.Position = basePos + 0xA8;
        uint texture_set_0xA8_ = bs.ReadUInt32();  // pgluTexSet

        bs.Position = basePos + 0xAC;
        uint unkModelSet0xAC_ = bs.ReadUInt32();   // ModelSet

        bs.Position = basePos + 0xB0;
        uint replay_gt4_ = bs.ReadUInt32();  // GT4ReplayData - Has explicit assert if missing

        bs.Position = basePos + 0xB4;
        uint play_start_camera_gt4_ = bs.ReadUInt32();  // GT4ReplayData - Has explicit assert if missing

        bs.Position = basePos + 0xB8;
        uint replay_start_camera_gt4_ = bs.ReadUInt32();  // GT4ReplayData - Has explicit assert if missing

        bs.Position = basePos + 0xBC;
        uint gadgets_ = bs.ReadUInt32();      // GadgetShapeList

        bs.Position = basePos + 0xC0;
        uint course_preview_camera_ = bs.ReadUInt32();  // GT4ReplayData - Has explicit assert if missing

        bs.Position = basePos + 0xC4;
        uint texture_set_0xC4_ = bs.ReadUInt32();  // pgluTexSet

        bs.Position = basePos + 0xC8;
        uint unkModelSet0xC8_ = bs.ReadUInt32();   // ModelSet

        bs.Position = basePos + 0xCC;
        uint unkPhotoMode0xCC_ = bs.ReadUInt32();  // DPhotoMode

        bs.Position = basePos + 0xD0;
        uint start_camera_ = bs.ReadUInt32();  // GT4ReplayData - Has explicit assert if missing

        bs.Position = basePos + 0xD4;
        uint replay_start_camera_ = bs.ReadUInt32();  // GT4ReplayData - Has explicit assert if missing

        bs.Position = basePos + 0xD8;
        uint unkModelSet0xD8_ = bs.ReadUInt32(); // ModelSet

        bs.Position = basePos + 0xDC;
        uint unkRunway0xCC = bs.ReadUInt32(); // RunwayGT4 - Has explicit assert if missing

        return courseData;
    }
}
