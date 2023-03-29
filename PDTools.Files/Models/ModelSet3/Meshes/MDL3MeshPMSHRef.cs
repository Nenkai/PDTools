using System;
using System.Collections.Generic;

using System.Numerics;
using System.Windows.Markup;

using Syroot.BinaryData;


namespace PDTools.Files.Models.ModelSet3.Meshes
{
    public class MDL3MeshPMSHRef
    {
        public float[] Values { get; set; } = new float[12];
        public int PackedMeshEntryIndex { get; set; }
        public static MDL3MeshPMSHRef FromStream(BinaryStream bs, long mdlBasePos, ushort mdl3VersionMajor)
        {
            long unkBasePos = bs.Position;

            var unk = new MDL3MeshPMSHRef();
            unk.Values = bs.ReadSingles(12);
            int unkOffset = bs.ReadInt32();
            unk.PackedMeshEntryIndex = bs.ReadInt32();

            return unk;
        }
    }
}
