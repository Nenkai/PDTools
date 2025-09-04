using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Syroot.BinaryData.Core;
using Syroot.BinaryData;
using System.IO;

using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.Formats;

using Pfim;
using Pfim.dds;
using System.Buffers.Binary;
using System.Runtime.InteropServices;
using System.Diagnostics;

namespace PDTools.Files.Textures.PS3;

public class CellTextureBuffer : TextureSet3Buffer
{
    public int LastMipmapLevel { get; set; }
    public CELL_GCM_TEXTURE_FORMAT FormatBits { get; set; }

    public short Depth { get; set; } = 1;

    public CellTextureBuffer()
    {
        
    }

    public override void Read(BinaryStream bs)
    {
        ImageOffset = bs.ReadUInt32();
        ImageSize = bs.ReadUInt32();
        bs.ReadByte(); // 2
        FormatBits = (CELL_GCM_TEXTURE_FORMAT)bs.ReadByte();
        LastMipmapLevel = bs.ReadByte();
        bs.ReadByte();

        Width = bs.ReadUInt16();
        Height = bs.ReadUInt16();
        bs.ReadUInt16();
        bs.ReadUInt16();
        bs.ReadInt32(); // Stream Set Offset
        bs.ReadInt32(); // Pad
    }
}