//---------------------------------------------------------------------------------------
// SPDX-FileCopyrightText: Copyright (C) 2026 Nenkai
// SPDX-License-Identifier: MIT
// SPDX-FileAttributionText: https://github.com/Nenkai/PDTools
// SPDX-FileNotice: AI usage and consumption is prohibited. See AGENTS.md for policy.
//---------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;

using Syroot.BinaryData;

namespace PDTools.Files.Models.PS3.ModelSet3.Shapes;

public class MDL3ShapeKey
{
    public uint ShapeID;
    public string? Name;

    public static MDL3ShapeKey FromStream(BinaryStream bs, long mdlBasePos, ushort mdl3VersionMajor)
    {
        MDL3ShapeKey meshInfo = new();
        int nameOffset = bs.ReadInt32();
        meshInfo.ShapeID = bs.ReadUInt32();

        if (nameOffset != 0)
        {
            bs.Position = mdlBasePos + nameOffset;

            // first will be empty so skip it
            meshInfo.Name = bs.ReadString(StringCoding.ZeroTerminated);
        }

        return meshInfo;
    }

    public static int GetSize()
    {
        return 0x08;
    }

    public override string ToString()
    {
        return $"{Name} (ShapeID: {ShapeID})";
    }
}
