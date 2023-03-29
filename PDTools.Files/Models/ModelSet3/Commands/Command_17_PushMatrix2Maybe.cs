using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Syroot.BinaryData;

namespace PDTools.Files.Models.ModelSet3.Commands
{
    /// <summary>
    /// BBox related?
    /// </summary>
    public class Command_17_PushMatrix2Maybe : ModelSetupCommand
    {
        public float[] Unk { get; set; }
        public override void Read(BinaryStream bs, int commandsBaseOffset)
        {
            Unk = bs.ReadSingles(16);
        }

        public override void Write(BinaryStream bs)
        {
            for (var i = 0; i < 16; i++)
                bs.WriteSingle(Unk[i]);
        }

        public override string ToString()
        {
            return $"{nameof(Command_17_PushMatrix2Maybe)} - Unk={string.Join(", ", Unk)}";
        }
    }
}
