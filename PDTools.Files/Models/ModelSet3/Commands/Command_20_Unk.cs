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
    public class Command_20_Unk : ModelCommand
    {
        public float[] Values { get; set; }
        public override void Read(BinaryStream bs, int commandsBaseOffset)
        {
            Values = bs.ReadSingles(4);
        }

        public override void Write(BinaryStream bs)
        {
            bs.WriteSingles(Values);
        }

        public override string ToString()
        {
            return $"{nameof(Command_20_Unk)} - {string.Join(", ", Values)}";
        }
    }
}
