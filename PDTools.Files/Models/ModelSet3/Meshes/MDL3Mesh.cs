using System;
using System.Collections.Generic;

using System.Numerics;
using PDTools.Files.Models.ModelSet3.FVF;
using PDTools.Files.Models.ModelSet3.Materials;
using Syroot.BinaryData;


namespace PDTools.Files.Models.ModelSet3.Meshes
{
    public class MDL3Mesh
    {
        public string Name { get; set; }

        public ushort Flags { get; set; }

        /// <summary>
        /// FVF Index to refer to for resolving the flexible vertex data.
        /// </summary>
        public short FVFIndex { get; set; }

        public short MaterialIndex;

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
        /// Bounds for this mesh. Must be 8 vectors
        /// </summary>
        public Vector3[] BBox { get; set; }

        public bool Tristrip = false;

        public MDL3FlexibleVertexDefinition FVF { get; set; }
        public MDL3Material Material { get; set; }
        public MDL3MeshUnk Unk { get; set; }

        public static MDL3Mesh FromStream(BinaryStream bs, long mdlBasePos, ushort mdl3VersionMajor)
        {
            long meshBasePos = bs.Position;

            MDL3Mesh mesh = new();

            mesh.Flags = bs.ReadUInt16();
            mesh.FVFIndex = bs.ReadInt16();
            mesh.MaterialIndex = bs.ReadInt16();
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
            int unkOffset = bs.ReadInt32();

            if (bboxOffset != 0)
            {
                bs.Position = mdlBasePos + bboxOffset;
                mesh.BBox = new Vector3[8];
                for (var i = 0; i < 8; i++)
                    mesh.BBox[i] = new Vector3(bs.ReadSingle(), bs.ReadSingle(), bs.ReadSingle());
            }

            if (unkOffset != 0)
            {
                bs.Position = mdlBasePos + unkOffset;
                mesh.Unk = MDL3MeshUnk.FromStream(bs, mdlBasePos, mdl3VersionMajor);
            }

            return mesh;
        }

        public static int GetSize()
        {
            return 0x30;
        }

        public override string ToString()
        {
            return $"{Name}";
        }
    }
}
