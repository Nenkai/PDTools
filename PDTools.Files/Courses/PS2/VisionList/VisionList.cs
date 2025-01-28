using Syroot.BinaryData;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDTools.Files.Courses.PS2.CourseSound;

public class VisionList
{
    /// <summary>
    /// 'Vls0'
    /// </summary>
    public const uint VISION_LIST_MAGIC = 0x30736C56;

    /// <summary>
    /// Unfinished, not to be used (doesn't read past header)
    /// </summary>
    /// <param name="stream"></param>
    /// <returns></returns>
    /// <exception cref="InvalidDataException"></exception>
    public VisionList FromStream(Stream stream)
    {
        BinaryStream bs = new BinaryStream(stream);
        long basePos = bs.Position;

        VisionList vision = new VisionList();
        uint magic = bs.ReadUInt32();

        if (magic != VISION_LIST_MAGIC)
            throw new InvalidDataException("Invalid VisionList magic.");

        bs.ReadUInt32(); // Reloc ptr

        bs.Position = basePos + 0x10;

        // %d x %d
        ushort blockX = bs.ReadUInt16(); 
        ushort blockY = bs.ReadUInt16();
        uint mesh = bs.ReadUInt32();

        bs.Position = basePos + 0x30;
        uint voronoiNum = bs.ReadUInt32();
        uint voronois = bs.ReadUInt32();
        return vision;
    }
}
