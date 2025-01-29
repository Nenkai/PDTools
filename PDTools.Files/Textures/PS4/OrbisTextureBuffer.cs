using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Buffers;
using System.Runtime.InteropServices;
using System.Numerics;

using Syroot.BinaryData.Core;
using Syroot.BinaryData;


namespace PDTools.Files.Textures.PS4;

public class OrbisTextureBuffer : TextureSet3Buffer
{
    public int Mipmap { get; set; }

    public ushort Width { get; set; }
    public ushort Height { get; set; }

    public OrbisTextureBuffer()
    {

    }

    public override void Read(BinaryStream bs)
    {
        ImageOffset = bs.ReadUInt32();
        bs.ReadUInt32();
        ImageSize = bs.ReadUInt32();
        bs.ReadUInt32();

        Width = bs.ReadUInt16();
        Height = bs.ReadUInt16();

        bs.ReadInt32(); // Unknown 1
    }

    public void InitFromDDSImage(Pfim.IImage image)
    {
        Width = (ushort)image.Width;
        Height = (ushort)image.Height;
        Mipmap = image.MipMaps.Length;
    }
}