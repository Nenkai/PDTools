using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Syroot.BinaryData;

namespace PDTools.Files.Models.ModelSet3.Commands
{
    /// <summary>
    /// Some graphics command - matrix related? Start render?
    /// </summary>
    public class Command_13_PSP_Unk : ModelSetupCommand
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
            return $"{nameof(Command_13_PSP_Unk)} - {Unk}";
        }
    }
}
