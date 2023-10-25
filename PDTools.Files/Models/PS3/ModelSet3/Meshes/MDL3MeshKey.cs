﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;

using Syroot.BinaryData;

namespace PDTools.Files.Models.PS3.ModelSet3.Meshes
{
    public class MDL3MeshKey
    {
        public uint MeshID;
        public string Name;

        public static MDL3MeshKey FromStream(BinaryStream bs, long mdlBasePos, ushort mdl3VersionMajor)
        {
            MDL3MeshKey meshInfo = new();
            int strOffset = bs.ReadInt32();
            meshInfo.MeshID = bs.ReadUInt32();

            bs.Position = mdlBasePos + strOffset;

            // first will be empty so skip it
            meshInfo.Name = bs.ReadString(StringCoding.ZeroTerminated);

            return meshInfo;
        }

        public static int GetSize()
        {
            return 0x08;
        }

        public override string ToString()
        {
            return $"{Name} (MeshID: {MeshID})";
        }
    }
}
