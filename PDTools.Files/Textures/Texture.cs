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

namespace PDTools.Files.Textures;

public abstract class Texture
{
    public string Name { get; set; }
    public long ImageOffset { get; set; }
    public long ImageSize { get; set; }

    public PGLUTextureInfo TextureRenderInfo { get; set; }

    public Memory<byte> ImageData { get; set; }
    
    public abstract void ConvertTextureToStandardFormat(string outputFile);

    public abstract void ReadTextureDetails(BinaryStream bs);
}