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
    /// Maps to GS ALPHA_1
    /// </summary>
    public class Cmd_pglBlendFunc : ModelSetupPS2Command
    {
        public override ModelSetupPS2Opcode Opcode => ModelSetupPS2Opcode.pglBlendFunc;

        // For each field,
        // - 0 = Cs - RGB value of the source is used
        // - 1 = Cd - RGB value in frame buffer is used
        // - 2 = 0

        /// <summary>
        /// 0 = Cs - RGB value of the source is used
        /// 1 = Cd - RGB value in frame buffer is used
        /// 2 = 0
        /// </summary>
        public byte A { get; set; }

        /// <summary>
        /// 0 = Cs - RGB value of the source is used
        /// 1 = Cd - RGB value in frame buffer is used
        /// 2 = 0
        /// </summary>
        public byte B { get; set; }

        /// <summary>
        /// 0 = As - Alpha of the source is used
        /// 1 = Ad - Alpha in the frame buffer is used.
        /// 2 = FIX - FIX-field value is used as Alpha.
        /// </summary>
        public byte C { get; set; }

        /// <summary>
        /// 0 = Cs - RGB value of the source is used
        /// 1 = Cd - RGB value in frame buffer is used
        /// 2 = 0
        /// </summary>
        public byte D { get; set; }

        /// <summary>
        /// Fixed Alpha Value
        /// </summary>
        public byte FIX { get; set; }

        public Cmd_pglBlendFunc()
        {

        }

        public Cmd_pglBlendFunc(byte a, byte b, byte c, byte d, byte fix)
        {
            A = a;
            B = b;
            C = c;
            D = d;
            FIX = fix;
        }

        public override void Read(BinaryStream bs, int commandsBaseOffset)
        {
            byte bits = bs.Read1Byte();
            A = (byte)((bits >> 0) & 0b11);
            B = (byte)((bits >> 2) & 0b11);
            C = (byte)((bits >> 4) & 0b11);
            D = (byte)((bits >> 6) & 0b11);
            FIX = bs.Read1Byte();
        }

        public override void Write(BinaryStream bs)
        {
            bs.WriteByte( (byte)(((D & 0b11) << 6) | ((C & 0b11) << 4) | ((B & 0b11) << 2) | (A & 0b11) ));
            bs.WriteByte(FIX);
        }

        public override string ToString()
        {
            return $"{nameof(Cmd_pglBlendFunc)}";
        }
    }
}
