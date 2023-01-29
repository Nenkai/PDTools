using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Syroot.BinaryData;
using PDTools.Files.Textures;

namespace PDTools.Files.Models.ModelSet3
{
    public class MDL3Materials
    {
        public List<MDL3Material> Materials { get; set; } = new();
        public List<PGLUCellTextureInfo> TextureInfos { get; set; } = new();

        public static MDL3Materials FromStream(BinaryStream bs, long mdlBasePos, ushort mdl3VersionMajor)
        {
            MDL3Materials materialMap = new();
            ushort materialCount = bs.ReadUInt16();
            ushort count_0x0c = bs.ReadUInt16();
            ushort count_0x10 = bs.ReadUInt16();
            ushort textureParameterCount = bs.ReadUInt16();

            uint materialsOffset = bs.ReadUInt32();
            uint offset_0x0C = bs.ReadUInt32();
            uint offset_0x10 = bs.ReadUInt32();
            uint textureInfoParamOffset = bs.ReadUInt32();

            for (int i = 0; i < materialCount; i++)
            {
                bs.Position = mdlBasePos + materialsOffset + i * 0x34;
                MDL3Material entry = MDL3Material.FromStream(bs, mdlBasePos, mdl3VersionMajor);
                materialMap.Materials.Add(entry);
            }

            for (int i = 0; i < textureParameterCount; i++)
            {
                bs.Position = mdlBasePos + textureInfoParamOffset + i * 0x44;

                var info = new PGLUCellTextureInfo();
                info.Read(bs);
                materialMap.TextureInfos.Add(info);
            }

            return materialMap;
        }
    }
}
