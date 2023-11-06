using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;

using Syroot.BinaryData;

namespace PDTools.Files.Models.PS2.Commands
{
    public class Command_30_pglDisableDestinationAlphaTest : ModelSetupPS2Command
    {
        public override ModelSetupPS2Opcode Opcode => ModelSetupPS2Opcode.pglDisableDestinationAlphaTest;

        public override void Read(BinaryStream bs, int commandsBaseOffset)
        {
            
        }

        public override void Write(BinaryStream bs)
        {
            
        }

        public override string ToString()
        {
            return $"{nameof(Command_30_pglDisableDestinationAlphaTest)}";
        }
    }
}
