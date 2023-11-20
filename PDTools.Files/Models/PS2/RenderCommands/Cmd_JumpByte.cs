using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;

using Syroot.BinaryData;

namespace PDTools.Files.Models.PS2.Commands
{
    public class Cmd_JumpByte : ModelSetupPS2Command
    {
        public override ModelSetupPS2Opcode Opcode => ModelSetupPS2Opcode.Jump_Byte;

        public byte JumpOffset { get; set; }

        public override void Read(BinaryStream bs, int commandsBaseOffset)
        {
            JumpOffset = bs.Read1Byte();           
        }

        public override void Write(BinaryStream bs)
        {
            bs.WriteByte(JumpOffset);
        }

        public override string ToString()
        {
            return $"{nameof(Cmd_JumpByte)}";
        }
    }
}
