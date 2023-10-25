using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;

using Syroot.BinaryData;

namespace PDTools.Files.Models.PS2.Commands
{
    public class Command_43_pgluTexTable1ub : ModelSetupPS2Command
    {
        public override ModelSetupPS2Opcode Opcode => ModelSetupPS2Opcode.pgluTexTable_1ub;

        public byte TexSetIndex { get; set; }

        public override void Read(BinaryStream bs, int commandsBaseOffset)
        {
            TexSetIndex = bs.Read1Byte();
        }

        public override void Write(BinaryStream bs)
        {
            bs.WriteByte(TexSetIndex);
        }

        public override string ToString()
        {
            return $"{nameof(Command_43_pgluTexTable1ub)} - TexSet: {TexSetIndex}";
        }
    }
}
