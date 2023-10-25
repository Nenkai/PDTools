using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Syroot.BinaryData;

namespace PDTools.Files.Models.PS3.PGLCommands
{
    public class Command_44_CallVM_Ptr2 : ModelSetupCommand
    {

        public override void Read(BinaryStream bs, int commandsBaseOffset)
        {

        }

        public override void Write(BinaryStream bs)
        {

        }

        public override string ToString()
        {
            return $"{nameof(Command_44_CallVM_Ptr2)}";
        }
    }
}
