using System;
using System.Collections.Generic;
using System.Linq;
using Syroot.BinaryData;

namespace PDTools.Files.Models.PS3.ModelSet3
{
    public class MDL3TextureKey
    {
        public uint TextureID { get; set; }
        public string Name { get; set; }

        public static MDL3TextureKey FromStream(BinaryStream bs, long mdlBasePos)
        {
            MDL3TextureKey modelKey = new();
            int strOffset = bs.ReadInt32();
            modelKey.TextureID = bs.ReadUInt32();

            bs.Position = mdlBasePos + strOffset;

            // first will be empty so skip it
            modelKey.Name = bs.ReadString(StringCoding.ZeroTerminated);

            return modelKey;
        }

        public static int GetSize()
        {
            return 0x08;
        }

        public override string ToString()
        {
            return $"{Name} (TextureID: {TextureID})";
        }
    }
}
