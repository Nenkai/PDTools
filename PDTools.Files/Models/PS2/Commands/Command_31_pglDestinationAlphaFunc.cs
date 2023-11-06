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
    /// Sets GS TEST register bit 15 - DATM (Destination Alpha Test Mode)
    /// </summary>
    public class Command_31_pglSetDestinationAlphaFunc : ModelSetupPS2Command
    {
        public override ModelSetupPS2Opcode Opcode => ModelSetupPS2Opcode.pglSetDestinationAlphaFunc;

        /// <summary>
        /// 2 = Set GS_TEST DATM bit to 0 - Pixels with destination equal to 0 pass
        /// 5 = Set GS_TEST DATM bit to 1 - Pixels with destination equal to 1 pass
        /// </summary>
        public byte Mode { get; set; }

        public override void Read(BinaryStream bs, int commandsBaseOffset)
        {
            Mode = bs.Read1Byte();
        }

        public override void Write(BinaryStream bs)
        {
            bs.WriteByte(Mode);
        }

        public override string ToString()
        {
            return $"{nameof(Command_31_pglSetDestinationAlphaFunc)}";
        }
    }
}
