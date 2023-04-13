using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Diagnostics;
using System.Numerics;

using SixLabors.ImageSharp;
using Syroot.BinaryData;

using PDTools.Utils;
using PDTools.Files.Textures;
using SixLabors.ImageSharp.PixelFormats;

namespace PDTools.Files.Textures.PS2
{
    public class TextureInfo
    {
        public uint TextureDataOffset { get; set; }
        public short BP { get; set; }
        public byte BW { get; set; }
        public SCE_GS_PSM Format { get; set; }
        public short Width { get; set; }
        public short Height { get; set; }

        // Used for serializing
        public bool IsPalette { get; set; }
        public Image<Rgba32> Image { get; set; }
        public IndexedImageFrame<Rgba32> IndexedImage { get; set; }
        public ReadOnlyMemory<Rgba32> Palette { get; set; }
        public Rgba32[] TiledPalette { get; set; }
        public int[] LinearToTiledPaletteIndices { get; set; }

        public void Read(ref BitStream bs)
        {
            TextureDataOffset = bs.ReadUInt32();
            BP = bs.ReadInt16();
            BW = bs.ReadByte();
            Format = (SCE_GS_PSM)bs.ReadByte();
            Width = bs.ReadInt16();
            Height = bs.ReadInt16();
        }

        public void Write(ref BitStream bs)
        {
            bs.WriteUInt32(TextureDataOffset);
            bs.WriteInt16(BP);
            bs.WriteByte(BW);
            bs.WriteByte((byte)Format);
            bs.WriteInt16(Width);
            bs.WriteInt16(Height);
        }
    }
}
