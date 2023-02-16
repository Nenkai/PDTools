using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;

using Syroot.BinaryData;

namespace PDTools.Files.Models.ModelSet3.Meshes
{
    public class MDL3PMSHKey
    {
        public uint PMSHID;
        public string Name;

        public static MDL3PMSHKey FromStream(BinaryStream bs, long mdlBasePos, ushort mdl3VersionMajor)
        {
            MDL3PMSHKey meshInfo = new();
            int strOffset = bs.ReadInt32();
            meshInfo.PMSHID = bs.ReadUInt32();

            bs.Position = mdlBasePos + strOffset;
            meshInfo.Name = bs.ReadString(StringCoding.ZeroTerminated);

            return meshInfo;
        }

        public static int GetSize()
        {
            return 0x08;
        }

        public override string ToString()
        {
            return $"{Name} (PMSHID: {PMSHID})";
        }
    }
}
