using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Numerics;
using Syroot.BinaryData;

namespace PDTools.Files.Models.ModelSet3.Commands
{
    public class Command_68_Unk : ModelSetupCommand
    {
        public Vector4 Unk { get; set; }
        public Vector4 Unk2 { get; set; }

        public override void Read(BinaryStream bs, int commandsBaseOffset)
        {
            Unk = new Vector4(bs.ReadSingle(), bs.ReadSingle(), bs.ReadSingle(), bs.ReadSingle());
            Unk2 = new Vector4(bs.ReadSingle(), bs.ReadSingle(), bs.ReadSingle(), bs.ReadSingle());
        }

        public override void Write(BinaryStream bs)
        {
            bs.WriteSingle(Unk.X); bs.WriteSingle(Unk.Y); bs.WriteSingle(Unk.Z); bs.WriteSingle(Unk.W);
            bs.WriteSingle(Unk2.X); bs.WriteSingle(Unk2.Y); bs.WriteSingle(Unk2.Z); bs.WriteSingle(Unk2.W);
        }

        public override string ToString()
        {
            return $"{nameof(Command_68_Unk)} - {Unk}, {Unk2}";
        }
    }
}
