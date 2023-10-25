using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Syroot.BinaryData;

namespace PDTools.Files.Models.PS3.PGLCommands
{
    /// <summary>
    /// Doesn't do anything?
    /// </summary>
    public class Command_69_Unk : ModelSetupCommand
    {
        public byte Unk { get; set; }

        public override void Read(BinaryStream bs, int commandsBaseOffset)
        {
            Unk = bs.Read1Byte();
        }

        public override void Write(BinaryStream bs)
        {
            bs.WriteByte(Unk);
        }

        public override string ToString()
        {
            return $"{nameof(Command_69_Unk)} - {Unk}";
        }
    }
}
