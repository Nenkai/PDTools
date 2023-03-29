using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Syroot.BinaryData;

namespace PDTools.Files.Models.ModelSet3.Commands
{
    public class Command_10_JumpToShort : ModelSetupCommand
    {
        public ushort RelativeJumpOffset { get; set; }
        public int AbsoluteJumpOffset { get; set; }

        public int JumpToIndex { get; set; }

        public override void Read(BinaryStream bs, int commandsBaseOffset)
        {
            RelativeJumpOffset = bs.ReadUInt16();

            // Translate relative to absolute
            long currentOffset = bs.Position - commandsBaseOffset;
            AbsoluteJumpOffset = RelativeJumpOffset + (int)currentOffset;
        }

        public override void Write(BinaryStream bs)
        {
            bs.WriteUInt16(RelativeJumpOffset);
        }

        public override string ToString()
        {
            return $"{nameof(Command_10_JumpToShort)}: {AbsoluteJumpOffset:X2}";
        }
    }
}
