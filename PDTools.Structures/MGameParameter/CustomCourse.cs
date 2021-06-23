using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.IO;
using System.Buffers.Binary;
using Syroot.BinaryData;

using PDTools.Utils;
using PDTools.Compression;

namespace PDTools.Structures.MGameParameter
{
    public class CustomCourse
    {
        public SceneryType Scenery { get; set; }
        public float RoadWidth { get; set; }
        public float StartPoint { get; set; }
        public DateTime Time { get; set; }
        public bool IsCircuit { get; set; }
        public float HomeStraightLength { get; set; }
        public float ElevationDifference { get; set; }
        public int CornerCount { get; set; }
        public float FinishLine { get; set; }
        public float StartLine { get; set; }

        public byte[] Data { get; set; }

        public static CustomCourse FromBase64File(string path)
        {
            string bytes = File.ReadAllText(path);
            return FromBase64(bytes);
        }

        public static CustomCourse FromBase64(string b64)
        {
            /*
            int magicIsh = BinaryPrimitives.ReadInt32LittleEndian(b64);
            if (magicIsh != 0x33376578)
                throw new FileFormatException("Not a valid compressed Base64 file.");
            */
            byte[] decoded = Convert.FromBase64String(b64);

            return Read(decoded);
        }

        public static CustomCourse FromTED(string path)
        {
            byte[] ted = File.ReadAllBytes(path);
            return Read(ted);
        }


        public static CustomCourse Read(byte[] bytes)
        {
            var course = new CustomCourse();

            if (BinaryPrimitives.ReadUInt32LittleEndian(bytes) == 0xFFF7EEC5)
                bytes = Deflater.Deflate(bytes);

            course.Data = bytes;
            using (var bs = new BinaryStream(new MemoryStream(bytes), ByteConverter.Big))
            {
                var magic = bs.ReadString(6);
                if (magic != "GT6TED")
                    throw new InvalidDataException($"Not a valid Custom Track File. (Magic needed is GT6TED, got {magic})");
                bs.Position += 2;

                bs.Position += 4;

                course.Scenery = (SceneryType)bs.ReadInt32();
                course.RoadWidth = bs.ReadSingle();
                bs.Position += 8;
                course.StartPoint = bs.ReadSingle();

                var time = new PDIDATETIME32();
                time.SetRawData(bs.ReadUInt32());
                course.Time = time.GetDateTime();
                course.IsCircuit = bs.ReadBoolean(BooleanCoding.Dword);
                bs.Position += 8;

                course.HomeStraightLength = bs.ReadSingle();
                course.ElevationDifference = bs.ReadSingle();
                course.CornerCount = bs.ReadInt32();
                course.FinishLine = bs.ReadSingle();
                course.StartLine = bs.ReadSingle();

                return course;
            }

        }
    }

    public enum SceneryType
    {
        Death_Valley = 1,
        Eifel = 2,
        Andalusia = 3,
        Eifel_Flat = 5,
    }
}
