using PDTools.Files.Courses.Runway;
using Syroot.BinaryData;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDTools.Files.Courses;

/// <summary>
/// Camera data. Also used in later games!
/// </summary>
public class GT4ReplayData
{
    /// <summary>
    /// 'REP4' (ReplayGT4)
    /// </summary>
    public const uint MAGIC = 0x34504552;

    public uint Version { get; set; }

    /// <summary>
    /// Unfinished, not to be used (doesn't read past version)
    /// </summary>
    /// <param name="stream"></param>
    /// <returns></returns>
    /// <exception cref="InvalidDataException"></exception>
    public static GT4ReplayData FromStream(Stream stream)
    {
        BinaryStream bs = new BinaryStream(stream);
        long basePos = bs.Position;

        GT4ReplayData cam = new GT4ReplayData();
        uint magic = bs.ReadUInt32();

        if (magic == MAGIC)
            throw new InvalidDataException("Invalid magic.");
        cam.Version = bs.ReadUInt32();

        // TODO
        return cam;
    }
}