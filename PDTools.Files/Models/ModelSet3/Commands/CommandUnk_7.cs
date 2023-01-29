using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Syroot.BinaryData;

namespace PDTools.Files.Models.ModelSet3.Commands
{
    public class CommandUnk_7 : ModelCommand
    {
        public ushort Value { get; set; }

        public override void Read(BinaryStream bs, int commandsBaseOffset)
        {
            Value = bs.ReadUInt16();
        }

        public override void Write(BinaryStream bs)
        {
            bs.WriteUInt16(Value);
        }

        public override string ToString()
        {
            return $"{nameof(CommandUnk_7)}: {Value}";
        }
    }
}
