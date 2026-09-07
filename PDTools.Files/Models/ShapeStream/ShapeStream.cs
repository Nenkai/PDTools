// Copyright (c) 2026 Nenkai
// SPDX-License-Identifier: MIT

using System.Collections.Generic;
using System.IO;

using Syroot.BinaryData;

using PDTools.Files.Models.PS3.ModelSet3.ShapeStream;
using PDTools.Files.Models.PS3.ModelSet3;
using System;

namespace PDTools.Files.Models.ShapeStream;

public class ShapeStream
{
    public List<ShapeStreamChunk> Chunks = [];

    static public ShapeStream FromStream(Stream stream, ModelSet3 mdl)
    {
        if (mdl.StreamingInfo is null)
            throw new InvalidOperationException("ModelSet3 has no shape streaming information.");

        ShapeStream ss = new();

        ushort i = 0;
        foreach (MDL3ShapeStreamingChunkInfo ssInfo in mdl.StreamingInfo.ChunkInfos)
        {
            var chunk = ShapeStreamChunk.FromStream(stream, ssInfo);
            ss.Chunks.Add(chunk);
            i++;
        }

        return ss;
    }

    public ShapeStreamShape? GetShapeByIndex(ushort meshIndex)
    {
        foreach (var chunk in Chunks)
        {
            if (chunk.Meshes.TryGetValue(meshIndex, out ShapeStreamShape? mesh))
                return mesh;
        }

        return null;
    }
}
