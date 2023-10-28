using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;

using Syroot.BinaryData;

namespace PDTools.Files.Models.PS2.Commands
{
    public class Command_35_pglFogColor1ui_default : ModelSetupPS2Command
    {
        public override ModelSetupPS2Opcode Opcode => ModelSetupPS2Opcode.pglFogColor1ui_default;

        public override void Read(BinaryStream bs, int commandsBaseOffset)
        {

        }

        public override void Write(BinaryStream bs)
        {

        }

        public override string ToString()
        {
            return $"{nameof(Command_35_pglFogColor1ui_default)}";
        }
    }
}
