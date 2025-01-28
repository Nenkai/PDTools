using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Syroot.BinaryData;
namespace PDTools.Files.Courses.Minimap;

public class CourseMapFace
{
    public List<CourseMapPoint> Points { get; set; } = [];

    public sbyte ZIndex { get; set; }
    public byte UnkFlag { get; set; }

    public static CourseMapFace FromStream(BinaryStream bs)
    {
        var mapFace = new CourseMapFace();
        mapFace.ZIndex = bs.ReadSByte();
        mapFace.UnkFlag = bs.Read1Byte();

        ushort count = bs.ReadUInt16();

        for (int i = 0; i < count; i++)
        {
            CourseMapPoint point = new CourseMapPoint();
            point.X = bs.ReadInt16();
            point.Y = bs.ReadInt16();
            point.Z = bs.ReadInt16();
            point.Unk = bs.ReadInt16();

            mapFace.Points.Add(point);
        }
        return mapFace;
    }
}
