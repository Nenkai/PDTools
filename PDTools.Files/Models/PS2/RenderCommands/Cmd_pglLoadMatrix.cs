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
    /// Calls pglLoadMatrix, same as glLoadMatrix. Replaces the current matrix with the one whose elements are specified by m. 
    /// </summary>
    public class Cmd_pglLoadMatrix : ModelSetupPS2Command
    {
        public override ModelSetupPS2Opcode Opcode => ModelSetupPS2Opcode.pglLoadMatrix;

        public Matrix4x4 Matrix { get; set; }

        public override void Read(BinaryStream bs, int commandsBaseOffset)
        {
            Matrix = new Matrix4x4(
                   bs.ReadSingle(), bs.ReadSingle(), bs.ReadSingle(), bs.ReadSingle(),
                   bs.ReadSingle(), bs.ReadSingle(), bs.ReadSingle(), bs.ReadSingle(),
                   bs.ReadSingle(), bs.ReadSingle(), bs.ReadSingle(), bs.ReadSingle(),
                   bs.ReadSingle(), bs.ReadSingle(), bs.ReadSingle(), bs.ReadSingle()
            );
        }

        public override void Write(BinaryStream bs)
        {
            bs.WriteSingle(Matrix.M11); bs.WriteSingle(Matrix.M12); bs.WriteSingle(Matrix.M13); bs.WriteSingle(Matrix.M14);
            bs.WriteSingle(Matrix.M21); bs.WriteSingle(Matrix.M22); bs.WriteSingle(Matrix.M23); bs.WriteSingle(Matrix.M24);
            bs.WriteSingle(Matrix.M31); bs.WriteSingle(Matrix.M32); bs.WriteSingle(Matrix.M33); bs.WriteSingle(Matrix.M34);
            bs.WriteSingle(Matrix.M41); bs.WriteSingle(Matrix.M42); bs.WriteSingle(Matrix.M43); bs.WriteSingle(Matrix.M44);
        }

        public override string ToString()
        {
            return $"{nameof(Cmd_pglLoadMatrix)}";
        }
    }
}
