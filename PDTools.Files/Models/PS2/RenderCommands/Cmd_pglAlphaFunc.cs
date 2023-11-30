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
    /// Sets the alpha function to use by setting GS register TEST's ATST & AREF fields
    /// </summary>
    public class Cmd_pglAlphaFunc : ModelSetupPS2Command
    {
        /// <summary>
        /// Alpha Test Method
        /// </summary>
        public AlphaTestFunc TST { get; set; }

        /// <summary>
        /// Alpha value to be compared and referred to
        /// </summary>
        public byte REF { get; set; }

        public override ModelSetupPS2Opcode Opcode => ModelSetupPS2Opcode.pglAlphaFunc;

        public Cmd_pglAlphaFunc()
        {

        }

        public Cmd_pglAlphaFunc(AlphaTestFunc tst, byte @ref)
        {
            TST = tst;
            REF = @ref;
        }

        public override void Read(BinaryStream bs, int commandsBaseOffset)
        {
            TST = (AlphaTestFunc)bs.Read1Byte();
            REF = bs.Read1Byte();
        }

        public override void Write(BinaryStream bs)
        {
            bs.WriteByte((byte)TST);
            bs.WriteByte(REF);
        }

        public override string ToString()
        {
            return $"{nameof(Cmd_pglAlphaFunc)}";
        }
    }

    public enum AlphaTestFunc
    {
        NEVER = 0,
        ALWAYS = 1,
        LESS = 2,
        LEQUAL = 3,
        EQUAL = 4,
        GEQUAL = 5,
        GREATER = 6,
        NOTEQUAL = 7
    }
}
