using System;
using System.Collections.Generic;

using System.Numerics;
using Syroot.BinaryData;


namespace PDTools.Files.Models
{
    public class MDL3Mesh
    {
        public ushort Flags { get; set; }
        
        /// <summary>
        /// FVF Index to refer to for resolving the flexible vertex data.
        /// </summary>
        public ushort FVFIndex { get; set; }

        public ushort MaterialIndex;

        /// <summary>
        /// Vertex Count for this mesh.
        /// </summary>
        public uint VertexCount { get; set; }

        /// <summary>
        /// Vertices offset within the model, if not in a shapestream.
        /// </summary>
        public uint VerticesOffset { get; set; }

        /// <summary>
        /// Length of 1 tri.
        /// </summary>
        public uint TriLength { get; set; }

        /// <summary>
        /// Count of tris for this mesh.
        /// </summary>
        public uint TriCount { get; set; }

        /// <summary>
        /// Tri offset within the model, if not in a shapestream.
        /// </summary>
        public uint TriOffset { get; set; }

        /// <summary>
        /// Bounds for this mesh.
        /// </summary>
        public Vector3[] BBox;

        public bool Tristrip = false;

        public static MDL3Mesh FromStream(BinaryStream bs, long mdlBasePos, ushort mdl3VersionMajor)
        {
            long meshBasePos = bs.Position;

            MDL3Mesh mesh = new();

            mesh.Flags = bs.ReadUInt16();
            mesh.FVFIndex = bs.ReadUInt16();
            mesh.MaterialIndex = bs.ReadUInt16();
            bs.ReadUInt16(); // Unk
            mesh.VertexCount = bs.ReadUInt32();
            mesh.VerticesOffset = bs.ReadUInt32();
            bs.ReadUInt32(); // Unk
            mesh.TriLength = bs.ReadUInt32();
            mesh.TriOffset = bs.ReadUInt32();
            bs.Position += 0x04;
            if (mdl3VersionMajor < 10)
            {
                bs.ReadInt16();
                bs.ReadInt16(); // Index
            }
            else
            {
                bs.ReadUInt32(); // Unk Offset
            }

            bs.ReadInt16();
            mesh.TriCount = bs.ReadUInt16();
            int bboxOffset = bs.ReadInt32();
            bs.ReadUInt32();

            return mesh;
        }
    }
}
