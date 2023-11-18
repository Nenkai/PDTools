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
    /// Calls pglRotateX, same as glRotate except on the Y axis.
    /// </summary>
    public class Cmd_pglRotateY : ModelSetupPS2Command
    {
        public override ModelSetupPS2Opcode Opcode => ModelSetupPS2Opcode.pglRotateY;

        /// <summary>
        /// Rotate value.
        /// </summary>
        public float RotateValue { get; set; }

        public override void Read(BinaryStream bs, int commandsBaseOffset)
        {
            RotateValue = bs.ReadSingle();
        }

        public override void Write(BinaryStream bs)
        {
            bs.WriteSingle(RotateValue);
        }

        public override string ToString()
        {
            return $"{nameof(Cmd_pglRotateY)}";
        }
    }
}
