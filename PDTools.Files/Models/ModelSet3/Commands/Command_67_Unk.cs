using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Syroot.BinaryData;

namespace PDTools.Files.Models.ModelSet3.Commands
{
    /// <summary>
    /// Same as 66
    /// </summary>
    public class Command_67_Unk : ModelSetupCommand
    {
        public uint Unk { get; set; }

        public override void Read(BinaryStream bs, int commandsBaseOffset)
        {
            Unk = bs.ReadUInt32();
        }

        public override void Write(BinaryStream bs)
        {
            bs.WriteUInt32(Unk);
        }

        public override string ToString()
        {
            return $"{nameof(Command_67_Unk)}";
        }
    }
}
