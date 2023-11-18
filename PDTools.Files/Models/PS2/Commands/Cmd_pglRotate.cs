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
    /// Calls pglRotate, same as glRotate. Multiplies the current matrix by the specified rotation vector.
    /// </summary>
    public class Cmd_pglRotate : ModelSetupPS2Command
    {
        public override ModelSetupPS2Opcode Opcode => ModelSetupPS2Opcode.pglRotate;

        /// <summary>
        /// Rotate vector.
        /// </summary>
        public Vector3 Vector { get; set; }

        public override void Read(BinaryStream bs, int commandsBaseOffset)
        {
            Vector = new Vector3(bs.ReadSingle(), bs.ReadSingle(), bs.ReadSingle());
        }

        public override void Write(BinaryStream bs)
        {
            bs.WriteSingle(Vector.X); bs.WriteSingle(Vector.Y); bs.WriteSingle(Vector.Z);
        }

        public override string ToString()
        {
            return $"{nameof(Cmd_pglRotate)}";
        }
    }
}
