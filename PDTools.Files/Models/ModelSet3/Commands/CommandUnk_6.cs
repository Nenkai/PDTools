using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Syroot.BinaryData;

namespace PDTools.Files.Models.ModelSet3.Commands
{
    public class CommandUnk_6 : ModelCommand
    {
        public byte Value { get; set; }

        public override void Read(BinaryStream bs, int commandsBaseOffset)
        {
            Value = bs.Read1Byte();
        }

        public override void Write(BinaryStream bs)
        {
            bs.WriteByte(Value);
        }

        public override string ToString()
        {
            return $"{nameof(CommandUnk_6)}: {Value}";
        }
    }
}
