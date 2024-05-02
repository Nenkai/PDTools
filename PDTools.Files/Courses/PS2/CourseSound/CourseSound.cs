using Syroot.BinaryData;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDTools.Files.Courses.PS2.CourseSound;

public class CourseSound
{
    /// <summary>
    /// 'CRSS'
    /// </summary>
    public const uint MAGIC = 0x53535243;
    public const uint VERSION_LATEST = 1;

    /// <summary>
    /// Unfinished, not to be used (doesn't read past header)
    /// </summary>
    /// <param name="stream"></param>
    /// <returns></returns>
    /// <exception cref="InvalidDataException"></exception>
    public static CourseSound FromStream(Stream stream)
    {
        BinaryStream bs = new BinaryStream(stream);
        long basePos = bs.Position;

        CourseSound crss = new CourseSound();
        uint magic = bs.ReadUInt32();

        if (magic != MAGIC)
            throw new InvalidDataException("Invalid CourseSound magic.");

        bs.ReadUInt32(); // Reloc ptr

        bs.Position = basePos + 0x10;
        uint version = bs.ReadUInt32(); // Should be 1 always.
        if (version != VERSION_LATEST)
            throw new InvalidDataException($"Invalid DCourseEffect version - should be {VERSION_LATEST}, got {version}.");

        bs.Position = basePos + 0x14;
        uint speakerListOffset = bs.ReadUInt32(); // CourseSound::SpeakerList (array of 5)
        return crss;
    }
}
