using System;
using System.Collections.Generic;
using System.Linq;
using Syroot.BinaryData;

namespace PDTools.Files.Models
{
    public class MDL3MeshInfo
    {
        public uint MeshIndex;
        public List<string> MeshParams;

        public static MDL3MeshInfo FromStream(BinaryStream bs, long mdlBasePos, ushort mdl3VersionMajor)
        {
            MDL3MeshInfo meshInfo = new();
            int strOffset = bs.ReadInt32();
            meshInfo.MeshIndex = bs.ReadUInt32();

            bs.Position = mdlBasePos + strOffset;

            string meshParamString = bs.ReadString(StringCoding.ZeroTerminated);
            // first will be empty so skip it
            meshInfo.MeshParams = new(meshParamString.Split("|").Skip(1));

            return meshInfo;
        }
    }
}
