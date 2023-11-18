using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;

using Syroot.BinaryData;

namespace PDTools.Files.Models.PS2.Commands
{
    public class Cmd_pglDepthBias : ModelSetupPS2Command
    {
        public override ModelSetupPS2Opcode Opcode => ModelSetupPS2Opcode.pglDepthBias;

        public float Value { get; set; }

        public override void Read(BinaryStream bs, int commandsBaseOffset)
        {
            Value = bs.ReadSingle();
        }

        public override void Write(BinaryStream bs)
        {
            bs.WriteSingle(Value);
        }

        public override string ToString()
        {
            return $"{nameof(Cmd_pglDepthBias)}";
        }
    }
}
