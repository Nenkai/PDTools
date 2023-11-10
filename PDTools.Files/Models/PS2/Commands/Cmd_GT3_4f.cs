using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;

using Syroot.BinaryData;

namespace PDTools.Files.Models.PS2.Commands
{
    /// <summary>
    /// Same as command 50 but sets all 4 values to specified.
    /// </summary>
    public class Cmd_GT3_4f : ModelSetupPS2Command
    {
        public override ModelSetupPS2Opcode Opcode => ModelSetupPS2Opcode.pglGT3_4f;

        /// <summary>
        /// Color. RGB only, A is ignored
        /// </summary>
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
            return $"{nameof(Cmd_GT3_4f)}";
        }
    }
}
