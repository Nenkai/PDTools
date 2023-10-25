using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Syroot.BinaryData;

namespace PDTools.Files.Models.PS3.PGLCommands
{
    // CELL_GCM_NV4097_SET_POLYGON_OFFSET_SCALE_FACTOR
    /// <summary>
    /// Stubbed in GT PSP
    /// </summary>
    public class Command_37_SetPolyOffsetScaleFactor : ModelSetupCommand
    {
        public float Factor { get; set; }
        public float Bias { get; set; }

        public override void Read(BinaryStream bs, int commandsBaseOffset)
        {
            Factor = bs.ReadSingle();
            Bias = bs.ReadSingle();

            // if Unk & Unk2 is 0.0
            // CELL_GCM_NV4097_SET_POLY_OFFSET_FILL_ENABLE(0) is called
            // otherwise CELL_GCM_NV4097_SET_POLY_OFFSET_FILL_ENABLE(1)

        }

        public override void Write(BinaryStream bs)
        {
            bs.WriteSingle(Factor);
            bs.WriteSingle(Bias);
        }

        public override string ToString()
        {
            return $"{nameof(Command_37_SetPolyOffsetScaleFactor)} - Factor:{Factor} Bias:{Bias}";
        }
    }
}
