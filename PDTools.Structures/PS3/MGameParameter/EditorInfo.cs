using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using PDTools.Utils;

namespace PDTools.Structures.PS3.MGameParameter
{
    public class EditorInfo
    {
        public int PspMode { get; set; }

        public void WriteToCache(ref BitStream bs)
        {
            bs.WriteUInt32(0xE6_E6_A0_7D);
            bs.WriteUInt32(1_00); // Version

            bs.WriteInt32(PspMode);
        }
    }
}
