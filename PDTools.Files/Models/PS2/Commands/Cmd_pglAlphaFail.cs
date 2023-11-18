using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;

using Syroot.BinaryData;

namespace PDTools.Files.Models.PS2.Commands
{
    public class Cmd_pglAlphaFail : ModelSetupPS2Command
    {
        public override ModelSetupPS2Opcode Opcode => ModelSetupPS2Opcode.pglAlphaFail;

        /// <summary>
        /// 0 = KEEP, 1 = FB_ONLY, 2 = ZB_ONLY, 3 = RGB_ONLY
        /// </summary>
        public byte GS_TEST_AFAIL { get; set; }

        public override void Read(BinaryStream bs, int commandsBaseOffset)
        {
            GS_TEST_AFAIL = bs.Read1Byte();
        }

        public override void Write(BinaryStream bs)
        {
            bs.WriteByte(GS_TEST_AFAIL);
        }

        public override string ToString()
        {
            return $"{nameof(Cmd_pglAlphaFail)}";
        }
    }
}
