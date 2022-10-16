using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using PDTools.Utils;

namespace PDTools.Structures.PS3
{
    public class MCarThin
    {
        public int? CarCode { get; set; }
        public string CarLabel { get; set; }

        public short Paint { get; set; }
        public MCarThin(int code)
        {
            CarCode = code;
        }

        public MCarThin(string label)
        {
            CarLabel = label;
        }

        public void Read(ref BitStream bs)
        {
            CarCode = bs.ReadInt32(); // Code
            Paint = bs.ReadInt16(); // Paint
            bs.ReadInt16(); // Is Tuned Car
            bs.ReadInt32();
        }

        public void Serialize(ref BitStream bs)
        {
            bs.WriteInt32(CarCode ?? -1); // Code
            bs.WriteInt16(Paint); // Paint
            bs.WriteInt16(0); // Is Tuned Car
            bs.WriteInt32(-1);
        }
    }
}
