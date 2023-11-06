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
    /// Maps to GS ALPHA_1 & 2
    /// </summary>
    public class Command_32_pglBlendFunc : ModelSetupPS2Command
    {
        public override ModelSetupPS2Opcode Opcode => ModelSetupPS2Opcode.pglBlendFunc;

        public byte ABCD { get; set; }

        /// <summary>
        /// Fixed Alpha Value
        /// </summary>
        public byte FIX { get; set; }

        public override void Read(BinaryStream bs, int commandsBaseOffset)
        {
            ABCD = bs.Read1Byte();
            FIX = bs.Read1Byte();
        }

        public override void Write(BinaryStream bs)
        {
            bs.WriteByte(ABCD);
            bs.WriteByte(FIX);
        }

        public override string ToString()
        {
            return $"{nameof(Command_32_pglBlendFunc)}";
        }
    }
}
