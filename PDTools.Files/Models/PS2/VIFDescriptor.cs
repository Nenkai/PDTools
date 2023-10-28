using Syroot.BinaryData;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDTools.Files.Models.PS2
{
    public class VIFDescriptor
    {
        public uint VIFDataOffset { get; set; }
        public ushort DMATagQuadwordCount { get; set; }

        /// <summary>
        /// 0 means no texture, anything above means id is id - 1. 511 is external
        /// </summary>
        public ushort pgluTextureIndex { get; set; }

        /// <summary>
        /// 0 means no material, anything above means id is id - 1.
        /// </summary>
        public ushort pgluMaterialIndex { get; set; }

        public static VIFDescriptor FromStream(BinaryStream bs, long mdlBasePos)
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
}
