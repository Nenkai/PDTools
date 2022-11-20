using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;
using System.Buffers.Binary;

using Syroot.BinaryData;
using System.Runtime.Intrinsics;
using System.Runtime.InteropServices;
using System.Runtime.Intrinsics.X86;
using System.Diagnostics;

namespace PDTools.Files.Courses.Runway
{
    public class RunwayRoadVertMap
    {
        public List<RunwayRoadVert> Vertices { get; set; } = new();

        public static RunwayRoadVertMap FromStream(BinaryStream bs, long count, RunwayFile rwy)
        {
            var map = new RunwayRoadVertMap();

            long basePos = bs.Position;
            for (int i = 0; i < count; i++)
            {
                var vert = new RunwayRoadVert();

                if (rwy.VersionMajor >= 6)
                {
                    bs.Position = basePos + (i * 0x0C);

                    if (rwy.BBoxAndRoadVertScaleX > 0 || rwy.BBoxAndRoadVertScaleY > 0 || rwy.BBoxAndRoadVertScaleZ > 0)
                    {
                        // Encoded
                        float X = (float)RunwayFile.DecodeAVXFloat(bs.ReadInt32(), rwy.BBoxAndRoadVertScaleX);
                        float Y = (float)RunwayFile.DecodeAVXFloat(bs.ReadInt32(), rwy.BBoxAndRoadVertScaleY);
                        float Z = (float)RunwayFile.DecodeAVXFloat(bs.ReadInt32(), rwy.BBoxAndRoadVertScaleZ);
                        vert.Vertex = new Vector3(X, Y, Z);
                    }
                    else
                    {
                        vert.Vertex = new Vector3(bs.ReadSingle(), bs.ReadSingle(), bs.ReadSingle());
                    }
                }
                else if (rwy.VersionMajor >= 4) 
                {
                    bs.Position = basePos + (i * 0x0C);
                    vert.Vertex = new Vector3(bs.ReadSingle(), bs.ReadSingle(), bs.ReadSingle());

                    bs.Position = basePos + (count * 0x0C) + (i * sizeof(ushort));
                    vert.Unk1 = bs.ReadUInt16();
                }
                else // V2 Confirmed
                {
                    bs.Position = basePos + (i * 0x10);
                    vert.Vertex = new Vector3(bs.ReadSingle(), bs.ReadSingle(), bs.ReadSingle());
                    vert.Unk1 = bs.ReadUInt16();
                    vert.Unk2 = bs.ReadUInt16();
                }

                map.Vertices.Add(vert);
            }

            return map;
        }

        public void ToStream(BinaryStream bs, ushort rwyVersionMajor, ushort rwyVersionMinor)
        {
            if (rwyVersionMajor >= 6)
            {
                throw new NotImplementedException("Implement RunwayRoadVert serializing for version >= 6");
            }
            else if (rwyVersionMajor >= 4)
            {
                for (int i = 0; i < Vertices.Count; i++)
                {
                    bs.WriteSingle(Vertices[i].Vertex.X);
                    bs.WriteSingle(Vertices[i].Vertex.Y);
                    bs.WriteSingle(Vertices[i].Vertex.Z);
                }

                for (int i = 0; i < Vertices.Count; i++)
                {
                    bs.WriteUInt16(Vertices[i].Unk1);
                }
            }
            else
            {
                for (int i = 0; i < Vertices.Count; i++)
                {
                    bs.WriteSingle(Vertices[i].Vertex.X);
                    bs.WriteSingle(Vertices[i].Vertex.Y);
                    bs.WriteSingle(Vertices[i].Vertex.Z);
                    bs.WriteUInt16(Vertices[i].Unk1);
                    bs.WriteUInt16(Vertices[i].Unk2);
                }
            }
        }
    }
}
