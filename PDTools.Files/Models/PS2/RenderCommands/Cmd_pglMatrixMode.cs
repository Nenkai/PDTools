using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;

using Syroot.BinaryData;

namespace PDTools.Files.Models.PS2.Commands
{
    public class Cmd_pglMatrixMode : ModelSetupPS2Command
    {
        public override ModelSetupPS2Opcode Opcode => ModelSetupPS2Opcode.pglMatrixMode;

        /// <summary>
        /// 0 = MODEL_VIEW
        /// 1 = PROJECTION
        /// 2 = TEXTURE
        /// </summary>
        public byte Mode { get; set; }

        public override void Read(BinaryStream bs, int commandsBaseOffset)
        {
            
        }

        public override void Write(BinaryStream bs)
        {
            
        }

        public override string ToString()
        {
            return $"{nameof(Cmd_pglMatrixMode)}";
        }
    }
}
