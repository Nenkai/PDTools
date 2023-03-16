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
        public byte JumpOffset { get; set; }
        private int _absoluteOffset;

        public override void Read(BinaryStream bs, int commandsBaseOffset)
        {
            JumpOffset = bs.Read1Byte();

            // Translate relative to absolute
            long currentOffset = bs.Position - commandsBaseOffset;
            _absoluteOffset = JumpOffset + (int)currentOffset;
        }

        public override void Write(BinaryStream bs)
        {
            bs.WriteByte(JumpOffset);
        }

        public override string ToString()
        {
            return $"{nameof(Command_9_JumpToByte)}: {_absoluteOffset.ToString("X2")}";
        }
    }
}
