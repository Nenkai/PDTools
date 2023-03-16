using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Syroot.BinaryData;

namespace PDTools.Files.Models.ModelSet3.Commands
{
    // CELL_GCM_NV4097_SET_POLYGON_OFFSET_SCALE_FACTOR
    public class Command_36_SetDepthMaskDisabled : ModelSetupCommand
    {
        public byte MaskBoolBits { get; set; }

        public override void Read(BinaryStream bs, int commandsBaseOffset)
        {
            MaskBoolBits = bs.Read1Byte();
        }

        public override void Write(BinaryStream bs)
        {

        }

        public override string ToString()
        {
            return $"{nameof(Command_36_SetDepthMaskDisabled)} - Mask:{MaskBoolBits}";
        }
    }
}
