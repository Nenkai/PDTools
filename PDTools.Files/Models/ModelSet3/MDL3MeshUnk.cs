﻿using System;
using System.Collections.Generic;

using System.Numerics;
using System.Windows.Markup;

using Syroot.BinaryData;


namespace PDTools.Files.Models.ModelSet3
{
    public class MDL3MeshUnk
    {
        public float[] Values { get; set; } = new float[12];
        public int PMSHEntryIndex { get; set; }
        public static MDL3MeshUnk FromStream(BinaryStream bs, long mdlBasePos, ushort mdl3VersionMajor)
        {
            long unkBasePos = bs.Position;

            var unk = new MDL3MeshUnk();
            unk.Values = bs.ReadSingles(12);
            int unkOffset = bs.ReadInt32();
            unk.PMSHEntryIndex = bs.ReadInt32();

            return unk;
        }
    }
}
