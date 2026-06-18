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

    /// <summary>
    /// Short token for this texture's pixel format (e.g. "A8R8G8B8", "DXT45", "IDTEX8"),
    /// used to tag dumped filenames so the format can be reproduced on rebuild.
    /// </summary>
    public virtual string GetPixelFormatName() => "UNKNOWN";
}
