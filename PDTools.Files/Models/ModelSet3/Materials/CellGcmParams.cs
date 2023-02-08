using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Syroot.BinaryData;

namespace PDTools.Files.Models.ModelSet3.Materials
{
    public class CellGcmParams
    {
        public int[] Params { get; set; } = new int[40];

        public static CellGcmParams FromStream(BinaryStream bs, long mdlBasePos, ushort mdl3VersionMajor)
        {
            CellGcmParams entry = new();
            entry.Params = bs.ReadInt32s(40);

            return entry;
        }

        public static int GetSize()
        {
            return 0xA0;
        }
    }
}
