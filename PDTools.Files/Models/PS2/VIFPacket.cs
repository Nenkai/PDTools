﻿using Syroot.BinaryData;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDTools.Files.Models.PS2;

public class VIFPacket
{
    public List<VIFCommand> Commands = [];

    public static VIFPacket FromStream(BinaryStream bs, int quadwordSize)
    {
        VIFPacket packet = new VIFPacket();
        long basePos = bs.Position;

        while (bs.Position < basePos + quadwordSize * 0x10)
        {
            var cmd = new VIFCommand();
            cmd.FromStream(bs);
            packet.Commands.Add(cmd);
        }

        return packet;
    }

    public void Write(BinaryStream bs)
    {
        for (int i = 0; i < Commands.Count; i++)
        {
            VIFCommand command = Commands[i];
            command.Write(bs);
        }

        bs.Align(0x10, grow: true);
    }
}
