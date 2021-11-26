using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Syroot.BinaryData;
using PDTools.Files.Textures;

namespace PDTools.Files.Models
{
    public class MDL3MaterialMap
    {
        public List<MDL3MaterialEntryUnk1> Entries1 { get; set; } = new();
        public List<PGLUTextureInfo> TextureInfos { get; set; } = new();

        public static MDL3MaterialMap FromStream(BinaryStream bs, long mdlBasePos, ushort mdl3VersionMajor)
        {
            MDL3MaterialMap materialMap = new();
            ushort count_0x08 = bs.ReadUInt16();
            ushort count_0x0c = bs.ReadUInt16();
            ushort count_0x10 = bs.ReadUInt16();
            ushort textureParameterCount = bs.ReadUInt16();

            uint offset_0x08 = bs.ReadUInt32();
            uint offset_0x0C = bs.ReadUInt32();
            uint offset_0x10 = bs.ReadUInt32();
            uint textureInfoParamOffset = bs.ReadUInt32();

            for (int i = 0; i < count_0x08; i++)
            {
                bs.Position = mdlBasePos + offset_0x08 + (i * 0x34);
                MDL3MaterialEntryUnk1 entry = MDL3MaterialEntryUnk1.FromStream(bs, mdlBasePos, mdl3VersionMajor);
                materialMap.Entries1.Add(entry);
            }

            for (int i = 0; i < textureParameterCount; i++)
            {
                bs.Position = mdlBasePos + textureInfoParamOffset + (i * 0x44);
                PGLUTextureInfo info = PGLUTextureInfo.FromStream(bs);
                materialMap.TextureInfos.Add(info);
            }

            return materialMap;
        }
    }
}
