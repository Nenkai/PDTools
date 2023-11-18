using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;

using Syroot.BinaryData;

namespace PDTools.Files.Models.PS2.Commands
{
    public class Cmd_pgluSetTexTable_Byte : ModelSetupPS2Command
    {
        public override ModelSetupPS2Opcode Opcode => ModelSetupPS2Opcode.pgluSetTexTable_Byte;

        /// <summary>
        /// Texture set table index to use. Mostly for LODs
        /// </summary>
        public byte TexSetTableIndex { get; set; }

        public override void Read(BinaryStream bs, int commandsBaseOffset)
        {
            TexSetTableIndex = bs.Read1Byte();
        }

        public override void Write(BinaryStream bs)
        {
            bs.WriteByte(TexSetTableIndex);
        }

        public override string ToString()
        {
            return $"{nameof(Cmd_pgluSetTexTable_UShort)} - TexSetTableIndex: {TexSetTableIndex}";
        }
    }
}
