using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Syroot.BinaryData;

namespace PDTools.Files.Models.PS3.PGLCommands
{
    public class Command_43_SetVMUnk : ModelSetupCommand
    {
        public int Unk { get; set; }
        public override void Read(BinaryStream bs, int commandsBaseOffset)
        {
            Unk = bs.ReadInt32();
        }

        public override void Write(BinaryStream bs)
        {
            bs.WriteInt32(Unk);
        }

        public override string ToString()
        {
            return $"{nameof(Command_43_SetVMUnk)} - {Unk}";
        }
    }
}
