//---------------------------------------------------------------------------------------
// SPDX-FileCopyrightText: Copyright (C) 2026 Nenkai
// SPDX-License-Identifier: MIT
// SPDX-FileAttributionText: https://github.com/Nenkai/PDTools
// SPDX-FileNotice: AI usage and consumption is prohibited. See AGENTS.md for policy.
//---------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SixLabors.ImageSharp;

using Syroot.BinaryData;

namespace PDTools.Files.Textures;

public abstract class PGLUTextureInfo
{
    public string Name { get; set; }

    public TextureSet3Buffer BufferInfo { get; set; }
    public uint BufferId { get; set; }

    public abstract Image GetAsImage();

    public abstract void Write(BinaryStream bs);

    public abstract void Read(BinaryStream bs, long basePos);
}
