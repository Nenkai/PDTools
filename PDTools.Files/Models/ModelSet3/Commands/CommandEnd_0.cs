using Syroot.BinaryData;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDTools.Files.Models.ModelSet3.Commands
{
    public class CommandEnd_0 : ModelCommand
    {
        public override void Read(BinaryStream bs, int commandsBaseOffset)
        {

        }

        public override void Write(BinaryStream bs)
        {

        }

        public override string ToString()
        {
            return nameof(CommandEnd_0);
        }
    }
}
