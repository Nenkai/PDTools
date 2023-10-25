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
    /// BBox related?
    /// </summary>
    public class Command_10_pglUnk_Enable17_WithMatrix : ModelSetupPS2Command
    {
        public override ModelSetupPS2Opcode Opcode => ModelSetupPS2Opcode.pglUnk_Enable17_WithMatrix;

        public Vector3[] BBox { get; set; }
        public short UnkOffset { get; set; }

        public override void Read(BinaryStream bs, int commandsBaseOffset)
        {
            byte count = bs.Read1Byte();
            BBox = new Vector3[count];
            for (int i = 0; i < count; i++)
                BBox[i] = new Vector3(bs.ReadSingle(), bs.ReadSingle(), bs.ReadSingle());
            UnkOffset = bs.ReadInt16();
        }

        public override void Write(BinaryStream bs)
        {
            bs.WriteByte((byte)BBox.Length);
            for (var i = 0; i < BBox.Length; i++)
            {
                bs.WriteSingle(BBox[i].X);
                bs.WriteSingle(BBox[i].Y);
                bs.WriteSingle(BBox[i].Z);
            }

            bs.WriteInt16(UnkOffset);
        }

        public override string ToString()
        {
            return $"{nameof(Command_10_pglUnk_Enable17_WithMatrix)} - Unk={string.Join(", ", BBox)}";
        }
    }
}
