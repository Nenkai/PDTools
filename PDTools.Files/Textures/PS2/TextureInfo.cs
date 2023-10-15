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
    /// <summary>
    /// Represents a data transfer being made to the GS memory.
    /// Dimensions may be different to texture dimensions - buffers may be swizzled for faster uploading to GS.
    /// </summary>
    public class GSTransfers
    {
        public uint DataOffset { get; set; }
        public short BP { get; set; }

        /// <summary>
        /// Buffer Width
        /// </summary>
        public byte BW { get; set; }

        /// <summary>
        /// "Image" format of the transfer
        /// </summary>
        public SCE_GS_PSM Format { get; set; }

        /// <summary>
        /// "Image" width of the transfer
        /// </summary>
        public short Width { get; set; }

        /// <summary>
        /// "Image" height of the transfer
        /// </summary>
        public short Height { get; set; }

        // Used for serializing
        public bool IsPalette { get; set; }
        public Image<Rgba32> Image { get; set; }
        public IndexedImageFrame<Rgba32> IndexedImage { get; set; }
        public ReadOnlyMemory<Rgba32> Palette { get; set; }
        public Rgba32[] TiledPalette { get; set; }
        public int[] LinearToTiledPaletteIndices { get; set; }

        public void Read(BinaryStream bs)
        {
            DataOffset = bs.ReadUInt32();
            BP = bs.ReadInt16();
            BW = bs.Read1Byte();
            Format = (SCE_GS_PSM)bs.ReadByte();
            Width = bs.ReadInt16();
            Height = bs.ReadInt16();
        }

        public void Write(BinaryStream bs)
        {
            bs.WriteUInt32(DataOffset);
            bs.WriteInt16(BP);
            bs.WriteByte(BW);
            bs.WriteByte((byte)Format);
            bs.WriteInt16(Width);
            bs.WriteInt16(Height);
        }
    }
}
