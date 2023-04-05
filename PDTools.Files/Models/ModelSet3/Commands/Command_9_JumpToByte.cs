using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Syroot.BinaryData;

namespace PDTools.Files.Models.ModelSet3.Commands
{
    public class Command_9_JumpToByte : ModelSetupCommand
    {
        public byte RelativeJumpToOffset { get; set; }
        public int AbsoluteJumpToOffset;

        public int JumpToIndex { get; set; }

        public override void Read(BinaryStream bs, int commandsBaseOffset)
        {
            RelativeJumpToOffset = bs.Read1Byte();

            // Translate relative to absolute
            long currentOffset = bs.Position - commandsBaseOffset;
            AbsoluteJumpToOffset = RelativeJumpToOffset + (int)currentOffset;
        }

        public override void Write(BinaryStream bs)
        {
            bs.WriteByte(RelativeJumpToOffset);
        }

        public override string ToString()
        {
            return $"{nameof(Command_9_JumpToByte)}: {AbsoluteJumpToOffset.ToString("X2")}";
        }
    }
}
