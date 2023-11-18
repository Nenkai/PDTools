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
    /// Whether to render this model based on bbox, seeks past commands if not
    /// </summary>
    public class Cmd_BBoxRender : ModelSetupPS2Command
    {
        public override ModelSetupPS2Opcode Opcode => ModelSetupPS2Opcode.BBoxRender;

        public Vector3[] BBox { get; set; }
        public List<ModelSetupPS2Command> CommandsOnRender { get; set; } = new();

        public override void Read(BinaryStream bs, int commandsBaseOffset)
        {
            byte count = bs.Read1Byte();
            BBox = new Vector3[count];
            for (int i = 0; i < count; i++)
                BBox[i] = new Vector3(bs.ReadSingle(), bs.ReadSingle(), bs.ReadSingle());

            long startOffset = bs.Position;
            ushort jumpOffsetOnNoRender = bs.ReadUInt16();

            while (bs.Position < startOffset + jumpOffsetOnNoRender)
            {
                ModelSetupPS2Opcode opcode = (ModelSetupPS2Opcode)bs.Read1Byte();
                if (opcode == ModelSetupPS2Opcode.End)
                    break;

 
                var cmd = ModelSetupPS2Command.GetByOpcode(opcode);

                cmd.Read(bs, 0);
                CommandsOnRender.Add(cmd);

                if (opcode == ModelSetupPS2Opcode.pglEnableRendering)
                    break;
            }
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

            long jumpOffsetOffset = bs.Position;
            bs.WriteInt16(0); // Write later

            foreach (var cmd in CommandsOnRender)
            {
                bs.WriteByte((byte)cmd.Opcode);
                cmd.Write(bs);
            }

            long endOffset = bs.Position;

            bs.Position = jumpOffsetOffset;
            bs.WriteUInt16((ushort)(endOffset - jumpOffsetOffset));
            bs.Position = endOffset;
        }

        public void SetBBox(Vector3[] bbox)
        {
            BBox = bbox;
        }

        public override string ToString()
        {
            return $"{nameof(Cmd_BBoxRender)} - BBox={string.Join(", ", BBox)}";
        }
    }
}
