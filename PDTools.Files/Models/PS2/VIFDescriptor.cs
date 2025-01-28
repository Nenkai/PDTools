using Syroot.BinaryData;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDTools.Files.Models.PS2;

public class VIFDescriptor
{
    public const int NO_TEXTURE = 0;
    public const int EXTERNAL_TEXTURE = 511;

    public const int NO_MATERIAL = 0;

    public uint VIFDataOffset { get; set; }
    public ushort DMATagQuadwordCount { get; set; }

    /// <summary>
    /// 0 means no texture, anything above 0 means index = this value - 1.
    /// 511 is external texture - i.e a car model shape that uses 511 refers to an external texture - eg. track reflection
    /// </summary>
    public ushort pgluTextureIndex { get; set; }

    /// <summary>
    /// 0 means no material, anything above means id is id - 1.
    /// </summary>
    public ushort pgluMaterialIndex { get; set; }

    public static VIFDescriptor FromStream(BinaryStream bs)
    {
        VIFDescriptor VIFDescriptor = new VIFDescriptor();
        VIFDescriptor.VIFDataOffset = bs.ReadUInt32();
        VIFDescriptor.DMATagQuadwordCount = bs.ReadUInt16();
        uint flags = bs.ReadUInt16();
        VIFDescriptor.pgluTextureIndex = (ushort)(flags & 0b111111111);
        VIFDescriptor.pgluMaterialIndex = (ushort)((flags >> 9) & 0b1111111);
        return VIFDescriptor;
    }

    public static uint GetSize()
    {
        return 0x08;
    }
}
