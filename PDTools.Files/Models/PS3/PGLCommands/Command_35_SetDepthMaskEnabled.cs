using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Syroot.BinaryData;

namespace PDTools.Files.Models.PS3.PGLCommands
{
    // CELL_GCM_NV4097_SET_POLYGON_OFFSET_SCALE_FACTOR
    public class Command_35_SetDepthMaskEnabled : ModelSetupCommand
    {
        public override void Read(BinaryStream bs, int commandsBaseOffset)
        {

        }

        public override void Write(BinaryStream bs)
        {

        }

        public override string ToString()
        {
            return $"{nameof(Command_35_SetDepthMaskEnabled)}";
        }
    }
}
