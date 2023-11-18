using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;

using Syroot.BinaryData;

namespace PDTools.Files.Models.PS2.Commands
{
    /// <summary>
    /// Calls glEnable(15). Enables face culling
    /// </summary>
    public class Cmd_pglEnableCullFace : ModelSetupPS2Command
    {
        public override ModelSetupPS2Opcode Opcode => ModelSetupPS2Opcode.pglEnableCullFace;

        public override void Read(BinaryStream bs, int commandsBaseOffset)
        {
            
        }

        public override void Write(BinaryStream bs)
        {
            
        }

        public override string ToString()
        {
            return $"{nameof(Cmd_pglEnableCullFace)}";
        }
    }
}
