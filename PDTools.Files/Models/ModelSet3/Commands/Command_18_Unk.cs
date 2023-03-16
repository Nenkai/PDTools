using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;

using Syroot.BinaryData;

namespace PDTools.Files.Models.ModelSet3.Commands
{
    /// <summary>
    /// Matrix related?
    /// </summary>
    public class Command_18_Unk : ModelSetupCommand
    {
        public Vector3 Vec { get; set; }
        public override void Read(BinaryStream bs, int commandsBaseOffset)
        {
            Vec = new Vector3(bs.ReadSingle(), bs.ReadSingle(), bs.ReadSingle());

        }

        public override void Write(BinaryStream bs)
        {

        }

        public override string ToString()
        {
            return $"{nameof(Command_18_Unk)} - {Vec}";
        }
    }
}
