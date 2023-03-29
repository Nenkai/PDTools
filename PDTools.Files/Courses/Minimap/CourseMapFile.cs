using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

using Syroot.BinaryData;

namespace PDTools.Files.Courses.Minimap
{
    public class CourseMapFile
    {
        public ushort VersionMajor { get; set; }
        public ushort VersionMinor { get; set; }

        public const uint MMAP_LE = 0x4D4D5150;
        public const uint MMAP_BE = 0x50414D4D;

        public float BoundMinX { get; set; }
        public float BoundMinY { get; set; }
        public float BoundMaxX { get; set; }
        public float BoundMaxY { get; set; }

        public List<CourseMapFace> OffCourseFaces { get; set; } = new();
        public List<CourseMapFace> RoadFaces { get; set; } = new();
        public List<CourseMapFace> PitLaneFaces { get; set; } = new();
        public List<CourseMapFace> SectionFaces { get; set; } = new();
        public List<CourseMapFace> FullRoadLine { get; set; } = new();

        /// <summary>
        /// VCoord
        /// </summary>
        public float RoadLength { get; set; }

        public static CourseMapFile FromStream(Stream stream)
        {
            BinaryStream bs = new BinaryStream(stream);
            long basePos = bs.Position;

            var map = new CourseMapFile();
            uint magic = bs.ReadUInt32();
            if (magic == MMAP_BE)
                bs.ByteConverter = ByteConverter.Big;
            else
                throw new InvalidDataException("Unexpected magic for course map stream.");

            map.VersionMajor = bs.ReadUInt16();
            map.VersionMinor = bs.ReadUInt16();

            uint unk = bs.ReadUInt32();
            uint fileSize = bs.ReadUInt32();

            uint offCourse = bs.ReadUInt32(); // 1 byte count, 3 bytes offset
            uint offCourseMeshCount = (byte)(offCourse >> 24);
            uint offCourseMeshesOffset = (byte)(offCourse & 0xFFFFFF); // 24 bits

            uint road = bs.ReadUInt32();
            uint roadMeshCount = (byte)(road >> 24);
            uint roadMeshesOffset = (road & 0xFFFFFF);

            uint pitlane = bs.ReadUInt32();
            uint pitlaneMeshCount = (byte)(pitlane >> 24);
            uint pitlaneMeshesOffset = (pitlane & 0xFFFFFF);

            uint section = bs.ReadUInt32();
            uint sectionMeshCount = (byte)(section >> 24);
            uint sectionMeshesOffset = (section & 0xFFFFFF);

            map.BoundMinX = (float)bs.ReadInt16();
            map.BoundMinY = (float)bs.ReadInt16();
            map.BoundMaxX = (float)bs.ReadInt16();
            map.BoundMaxY = (float)bs.ReadInt16();

            uint unks = bs.ReadUInt32(); // 1 byte count, 3 bytes offset
            uint unkMeshCount = (byte)(unks >> 24);
            uint unkMeshesOffset = (unks & 0xFFFFFF); // 24 bits

            bs.Position += 4;
            map.RoadLength = bs.ReadSingle();

            if (offCourseMeshCount > 0 && offCourseMeshesOffset != 0)
            {
                bs.Position = basePos + offCourseMeshesOffset;
                for (int i = 0; i < offCourseMeshCount; i++)
                {
                    CourseMapFace face = CourseMapFace.FromStream(bs);
                    map.OffCourseFaces.Add(face);
                }
            }

            if (roadMeshCount > 0 && roadMeshesOffset != 0)
            {
                bs.Position = basePos + roadMeshesOffset;
                for (int i = 0; i < roadMeshCount; i++)
                {
                    CourseMapFace face = CourseMapFace.FromStream(bs);
                    map.RoadFaces.Add(face);
                }
            }

            if (pitlaneMeshCount > 0 && pitlaneMeshesOffset != 0)
            {
                bs.Position = basePos + pitlaneMeshesOffset;
                for (int i = 0; i < pitlaneMeshCount; i++)
                {
                    CourseMapFace face = CourseMapFace.FromStream(bs);
                    map.PitLaneFaces.Add(face);
                }
            }

            if (sectionMeshCount > 0 && sectionMeshesOffset != 0)
            {
                bs.Position = basePos + sectionMeshesOffset;
                for (int i = 0; i < 1; i++)
                {
                    CourseMapFace face = CourseMapFace.FromStream(bs);
                    map.SectionFaces.Add(face);
                }
            }

            if (unkMeshCount > 0 && unkMeshesOffset != 0)
            {
                bs.Position = basePos + unkMeshesOffset;
                for (int i = 0; i < unkMeshCount; i++)
                {
                    CourseMapFace face = CourseMapFace.FromStream(bs);
                    map.FullRoadLine.Add(face);
                }
            }

            return map;
        }
    }
}
