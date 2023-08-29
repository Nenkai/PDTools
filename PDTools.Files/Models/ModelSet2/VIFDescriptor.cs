using Syroot.BinaryData;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDTools.Files.Models.ModelSet2
{
    public class VIFDescriptor
    {
        public uint VIFDataOffset { get; set; }
        public ushort DMATagQuadwordCount { get; set; }
        public ushort pgluMaterialFuncParams { get; set; }

        public static VIFDescriptor FromStream(BinaryStream bs, long mdlBasePos)
        {
            VIFDescriptor VIFDescriptor = new VIFDescriptor();
            VIFDescriptor.VIFDataOffset = bs.ReadUInt32();
            VIFDescriptor.DMATagQuadwordCount = bs.ReadUInt16();
            VIFDescriptor.pgluMaterialFuncParams = bs.ReadUInt16();
            return VIFDescriptor;
        }

        public static uint GetSize()
        {
            return 0x08;
        }
    }
}
