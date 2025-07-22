using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using PDTools.Files.Models.PS3.ModelSet3.Textures;

using Syroot.BinaryData;

namespace PDTools.Files.Models.PS3.ModelSet3.Materials;

public class MDL3MaterialData
{
    public string Name { get; set; }
    public int UnkIndex { get; set; }
    public byte Unk0x01 { get; set; }
    public byte Version { get; set; }
    public short Unk0x20 { get; set; }

    public int[] Unk0x0CData { get; set; }
    public List<MDL3TextureKey> TextureKeys { get; set; } = new();
    public MDL3MaterialData_0x14 _0x14 { get; set; } = new();
    public List<MDL3MaterialData_0x18> _0x18 { get; set; } = new();
    public MDL3MaterialData_0x1C _0x1C { get; set; }
    public MDL3MaterialShaderReferences ShaderReferences { get; set; }

    public static MDL3MaterialData FromStream(BinaryStream bs, long mdlBasePos, ushort mdl3VersionMajor)
    {
        MDL3MaterialData entry = new();
        int nameOffset = bs.ReadInt32();
        entry.UnkIndex = bs.ReadInt32();
        entry.Unk0x01 = bs.Read1Byte();
        entry.Version = bs.Read1Byte();

        short keyCount = bs.ReadInt16();

        int unkOffset0x0C = bs.ReadInt32();
        int keysOffset = bs.ReadInt32();
        int unkOffset0x14 = bs.ReadInt32();
        int unkOffset0x18 = bs.ReadInt32();
        int unkOffset0x1C = bs.ReadInt32();
        entry.Unk0x20 = bs.ReadInt16();
        int count0x18 = bs.ReadInt16();
        int shaderReferencesOffset = bs.ReadInt32();

        bs.Position = nameOffset;
        entry.Name = bs.ReadString(StringCoding.ZeroTerminated);

        if (unkOffset0x0C != 0)
        {
            bs.Position = unkOffset0x0C;
            entry.Unk0x0CData = bs.ReadInt32s(8);
        }

        for (var i = 0; i < keyCount; i++)
        {
            bs.Position = mdlBasePos + keysOffset + i * MDL3TextureKey.GetSize();

            var key = MDL3TextureKey.FromStream(bs, mdlBasePos);
            entry.TextureKeys.Add(key);
        }

        if (unkOffset0x14 != 0)
        {
            bs.Position = mdlBasePos + unkOffset0x14;
            entry._0x14 = MDL3MaterialData_0x14.FromStream(bs, mdlBasePos, mdl3VersionMajor);
        }

        for (var i = 0; i < count0x18; i++)
        {
            bs.Position = mdlBasePos + unkOffset0x18 + i * 8;
            MDL3MaterialData_0x18 unk0x18 = MDL3MaterialData_0x18.FromStream(bs, mdlBasePos, mdl3VersionMajor);
            entry._0x18.Add(unk0x18);
        }


        if (unkOffset0x1C != 0)
        {
            bs.Position = mdlBasePos + unkOffset0x1C;
            entry._0x1C = MDL3MaterialData_0x1C.FromStream(bs, mdlBasePos, mdl3VersionMajor);
        }

        if (shaderReferencesOffset != 0)
        {
            bs.Position = mdlBasePos + shaderReferencesOffset;
            entry.ShaderReferences = MDL3MaterialShaderReferences.FromStream(bs, mdlBasePos, mdl3VersionMajor);
        }

        return entry;
    }

    public static int GetSize()
    {
        return 0x28;
    }
}
