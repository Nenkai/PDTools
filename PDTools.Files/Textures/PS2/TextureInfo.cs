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

namespace PDTools.Files.Textures.PS2; 

/// <summary>
/// Represents a data transfer being made to the GS memory.
/// Dimensions may be different to texture dimensions - buffers may be swizzled for faster uploading to GS.
/// </summary>
public class GSTransfer
{
    public uint DataOffset { get; set; }
    public ushort BP { get; set; }

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
    public ushort Width { get; set; }

    /// <summary>
    /// "Image" height of the transfer
    /// </summary>
    public ushort Height { get; set; }

    // Used for serializing
    public byte[] Data { get; set; }

    /// <summary>
    /// Read old transfer data (from GT2K)
    /// </summary>
    /// <param name="bs"></param>
    public void ReadOld(BinaryStream bs)
    {
        BP = bs.ReadUInt16();
        BW = bs.Read1Byte();
        Format = (SCE_GS_PSM)bs.ReadByte();
        bs.ReadUInt32(); // Removed in later games
        Width = bs.ReadUInt16();
        Height = bs.ReadUInt16();

        Data = bs.ReadBytes(Tex1Utils.GetDataSize(Width, Height, Format));
        if (Format == SCE_GS_PSM.SCE_GS_PSMCT24)
        {
            // PSMCT24 ACTUALLY only contains 24 bpp in the transfer data, alter it
            // Used by GT2K opening/logo.img
            byte[] newData = new byte[Width * Height * 4];
            for (int i = 0; i < Data.Length / 3; i++)
            {
                newData[i*4] = Data[(i*3)];
                newData[i*4+1] = Data[(i*3)+1];
                newData[i*4+2] = Data[(i*3)+2];
                newData[i*4+3] = 0x80; // As per GS documentation, Alpha is always 0x80 in PSMCT24
            }

            Data = newData;
        }
    }

    public void Read(BinaryStream bs, long texSetBasePos)
    {
        DataOffset = bs.ReadUInt32();
        BP = bs.ReadUInt16();
        BW = bs.Read1Byte();
        Format = (SCE_GS_PSM)bs.ReadByte();
        Width = bs.ReadUInt16();
        Height = bs.ReadUInt16();

        bs.Position = texSetBasePos + DataOffset;
        Data = bs.ReadBytes(Tex1Utils.GetDataSize(Width, Height, Format));
    }

    public void Write(BinaryStream bs)
    {
        bs.WriteUInt32(DataOffset);
        bs.WriteUInt16(BP);
        bs.WriteByte(BW);
        bs.WriteByte((byte)Format);
        bs.WriteUInt16(Width);
        bs.WriteUInt16(Height);
    }

    public static int GetSize()
    {
        return 0x0C;
    }

    public override int GetHashCode()
    {
        const int p = 16777619;
        int hashcode = 1430287;
        hashcode = hashcode * 7302013 ^ BP.GetHashCode();
        hashcode = hashcode * 7302013 ^ BW.GetHashCode();
        hashcode = hashcode * 7302013 ^ Format.GetHashCode();
        hashcode = hashcode * 7302013 ^ Width.GetHashCode();
        hashcode = hashcode * 7302013 ^ Height.GetHashCode();

        for (int i = 0; i < Data.Length; i++)
            hashcode = (hashcode ^ Data[i]) * p;

        return hashcode;
    }
}
