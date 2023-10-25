﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Syroot.BinaryData;

namespace PDTools.Files.Models.PS3.PGLCommands
{
    // CELL_GCM_NV4097_SET_POLYGON_OFFSET_SCALE_FACTOR
    public class Command_34_SetColorMask : ModelSetupCommand
    {
        public byte MaskBoolBits { get; set; }

        public override void Read(BinaryStream bs, int commandsBaseOffset)
        {
            MaskBoolBits = bs.Read1Byte();
        }

        public override void Write(BinaryStream bs)
        {
            bs.WriteByte(MaskBoolBits);
        }

        public override string ToString()
        {
            return $"{nameof(Command_34_SetColorMask)} - Mask:{MaskBoolBits}";
        }
    }
}
