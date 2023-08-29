using Syroot.BinaryData;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDTools.Files.Models.ModelSet2
{
    public class Model
    {
        public static Model FromStream(BinaryStream bs, long mdlBasePos)
        {
            Model model = new Model();

            bs.Read1Byte();
            byte boundCount = bs.Read1Byte();

            return model;
        }

        public static uint GetSize()
        {
            return 0x24;
        }
    }
}
