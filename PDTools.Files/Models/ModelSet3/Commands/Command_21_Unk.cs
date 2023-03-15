using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;

using Syroot.BinaryData;

namespace PDTools.Files.Models.ModelSet3.Commands
{
    public class Command_21_Unk : ModelCommand
    {
        public float Value { get; set; }
        public override void Read(BinaryStream bs, int commandsBaseOffset)
        {
            Value = bs.ReadSingle();
        }

        public override void Write(BinaryStream bs)
        {
            bs.WriteSingle(Value);
        }

        public override string ToString()
        {
            return $"{nameof(Command_21_Unk)} - {Value}";
        }
    }
}
