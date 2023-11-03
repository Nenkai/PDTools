using Syroot.BinaryData;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDTools.Files.Models.PS2.ModelSet
{
    public class ModelSet2Model
    {
        public static ModelSet2Model FromStream(BinaryStream bs, long mdlBasePos)
        {
            ModelSet2Model model = new ModelSet2Model();

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
