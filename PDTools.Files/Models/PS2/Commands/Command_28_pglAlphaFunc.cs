using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;

using Syroot.BinaryData;

namespace PDTools.Files.Models.PS2.Commands
{
    public class Command_28_pglAlphaFunc : ModelSetupPS2Command
    {
        public byte Func { get; set; }
        public byte Ref { get; set; }

        public override ModelSetupPS2Opcode Opcode => ModelSetupPS2Opcode.pglAlphaFunc;

        public override void Read(BinaryStream bs, int commandsBaseOffset)
        {
            Func = bs.Read1Byte();
            Ref = bs.Read1Byte();
        }

        public override void Write(BinaryStream bs)
        {
            Func = bs.Read1Byte();
            Ref = bs.Read1Byte();
        }

        public override string ToString()
        {
            return $"{nameof(Command_28_pglAlphaFunc)}";
        }
    }
}
