using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;

using Syroot.BinaryData;

namespace PDTools.Files.Models.PS2.Commands
{
    public class Cmd_Jump1us : ModelSetupPS2Command
    {
        public override ModelSetupPS2Opcode Opcode => ModelSetupPS2Opcode.JumpShort;

        public ushort JumpOffset { get; set; }

        public override void Read(BinaryStream bs, int commandsBaseOffset)
        {
            JumpOffset = bs.ReadUInt16();           
        }

        public override void Write(BinaryStream bs)
        {
            bs.WriteUInt16(JumpOffset);
        }

        public override string ToString()
        {
            return $"{nameof(Cmd_Jump1us)}";
        }
    }
}
