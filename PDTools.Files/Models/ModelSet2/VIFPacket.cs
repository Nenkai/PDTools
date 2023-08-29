using Syroot.BinaryData;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDTools.Files.Models.ModelSet2
{
    public class VIFPacket
    {
        public List<VIFCommand> Commands = new List<VIFCommand>();

        public static VIFPacket FromStream(BinaryStream bs, int quadwordSize, long mdlBasePos)
        {
            VIFPacket packet = new VIFPacket();
            long basePos = bs.Position;

            while (bs.Position < basePos + (quadwordSize * 0x10))
            {
                var cmd = VIFCommand.FromStream(bs, mdlBasePos);
                packet.Commands.Add(cmd);
            }

            return packet;
        }
    }
}
