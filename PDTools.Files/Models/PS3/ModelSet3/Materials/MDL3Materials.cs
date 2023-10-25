using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Syroot.BinaryData;
using PDTools.Files.Textures.PS3;

namespace PDTools.Files.Models.PS3.ModelSet3.Materials
{
    public class MDL3Materials
    {
        public List<MDL3Material> Definitions { get; set; } = new();
        public List<MDL3MaterialData> MaterialDatas { get; set; } = new();
        public List<CellGcmParams> GcmParams { get; set; } = new();
        public List<PGLUCellTextureInfo> TextureInfos { get; set; } = new();

        public static MDL3Materials FromStream(BinaryStream bs, long mdlBasePos, ushort mdl3VersionMajor)
        {
            MDL3Materials materialMap = new();
            ushort materialCount = bs.ReadUInt16();
            ushort materialDataCount = bs.ReadUInt16();
            byte unk = bs.Read1Byte();
            byte cellGcmParamCount = bs.Read1Byte();
            ushort textureParameterCount = bs.ReadUInt16();

            uint materialsOffset = bs.ReadUInt32();
            uint materialDataOffset = bs.ReadUInt32();
            uint cellGcmParamsOffset = bs.ReadUInt32();
            uint textureInfoParamOffset = bs.ReadUInt32();

            for (int i = 0; i < materialCount; i++)
            {
                bs.Position = mdlBasePos + materialsOffset + i * 0x34;
                MDL3Material entry = MDL3Material.FromStream(bs, mdlBasePos, mdl3VersionMajor);
                materialMap.Definitions.Add(entry);
            }

            for (int i = 0; i < materialDataCount; i++)
            {
                bs.Position = mdlBasePos + materialDataOffset + i * 0x28;
                MDL3MaterialData entry = MDL3MaterialData.FromStream(bs, mdlBasePos, mdl3VersionMajor);
                materialMap.MaterialDatas.Add(entry);
            }

            for (int i = 0; i < cellGcmParamCount; i++)
            {
                bs.Position = mdlBasePos + cellGcmParamsOffset + i * 0xA0;
                CellGcmParams entry = CellGcmParams.FromStream(bs, mdlBasePos, mdl3VersionMajor);
                materialMap.GcmParams.Add(entry);
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
