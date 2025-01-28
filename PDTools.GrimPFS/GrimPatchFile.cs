﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDTools.GrimPFS;

public class GrimPatchFile
{
    public GrimPatchFileType FileType { get; set; }

    public uint FileIndex { get; set; }

    public string GamePath { get; set; }

    public string PFSPath { get; set; }

    public uint ChunkId { get; set; }

    public string DownloadPath { get; set; }
}

public enum GrimPatchFileType
{
    PatchSequence,
    UpdateNodeInfo,
    Header,
    TOC,
    GameFile,
}
