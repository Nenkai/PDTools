using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Syroot.BinaryData;

namespace PDTools.Files.Models.ModelSet3.Commands
{
    public class Command_72_Unk : ModelSetupCommand
    {
        public override void Read(BinaryStream bs, int commandsBaseOffset)
        {
            throw new NotSupportedException("Implement command 72 if you see it");
        }

        public override void Write(BinaryStream bs)
        {
            
        }

        public override string ToString()
        {
            return $"{nameof(Command_72_Unk)}";
        }
    }
}
