// Copyright (c) 2026 Nenkai
// SPDX-License-Identifier: MIT

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using PDTools.Files.Models.PS3.ModelSet3.Textures;

using Syroot.BinaryData;

namespace PDTools.Files.Models.PS3.ModelSet3.Materials;

public class MDL3MaterialShaderReference
{
    public string? ShaderReferenceName { get; set; }
    public int ShaderID { get; set; }
    public byte Unk0x01 { get; set; }
    public byte Version { get; set; }
    public MDL3TextureKey? TextureKey { get; set; }
    public static MDL3MaterialShaderReference FromStream(BinaryStream bs, long mdlBasePos, ushort mdl3VersionMajor)
    {
        MDL3MaterialShaderReference entry = new();
        int shaderNameOffset = bs.ReadInt32();
        entry.ShaderID = bs.ReadInt32();
        bs.ReadInt32(); // Empty
        int unkOffset_0x0C = bs.ReadInt32();
        short unkCount = bs.ReadInt16();
        short unkCount2 = bs.ReadInt16();
        int unkOffset_0x10 = bs.ReadInt32();
        int typeOrVersion = bs.ReadInt32();
        int keyOffset = bs.ReadInt32();

        if (shaderNameOffset != 0)
        {
            bs.Position = mdlBasePos + shaderNameOffset;
            entry.ShaderReferenceName = bs.ReadString(StringCoding.ZeroTerminated);
        }

        bs.Position = mdlBasePos + unkOffset_0x0C;
        // TODO - this is something we go through in another master model structure

        bs.Position = mdlBasePos + keyOffset;

        return entry;
    }

    public static int GetSize()
    {
        return 0x28;
    }

    public override string? ToString()
    {
        return ShaderReferenceName;
    }
}
