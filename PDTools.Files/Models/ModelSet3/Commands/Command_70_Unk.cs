using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Syroot.BinaryData;

namespace PDTools.Files.Models.ModelSet3.Commands
{
    /// <summary>
    /// Not present in GT PSP
    /// </summary>
    public class Command_70_Unk : ModelCommand
    {
        public override void Read(BinaryStream bs, int commandsBaseOffset)
        {
            
        }

        public override void Write(BinaryStream bs)
        {
            
        }

        public override string ToString()
        {
            return $"{nameof(Command_70_Unk)}";
        }
    }
}
