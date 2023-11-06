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
    /// Sets FOGCOL register
    /// </summary>
    public class Command_33_pglSetFogColor : ModelSetupPS2Command
    {
        public override ModelSetupPS2Opcode Opcode => ModelSetupPS2Opcode.pglSetFogColor;

        /// <summary>
        /// Color. RGB only, A is ignored
        /// </summary>
        public uint Color { get; set; }

        public override void Read(BinaryStream bs, int commandsBaseOffset)
        {
            Color = bs.ReadUInt32();
        }

        public override void Write(BinaryStream bs)
        {
            bs.WriteUInt32(Color);
        }

        public override string ToString()
        {
            return $"{nameof(Command_33_pglSetFogColor)}";
        }
    }
}
