using PDTools.Utils;
using Syroot.BinaryData;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDTools.Files.Textures.PSP;

public class GECommandList
{
    public Dictionary<SCE_GE_CMD, SCE_GE_CMD_BASE> Commands = [];

    public void Read(BinaryStream bs, PGLUGETextureInfo textureInfo)
    {
        while (true)
        {
            uint val = bs.ReadUInt32();

            var cmdType = (SCE_GE_CMD)(val >> 24);
            if (cmdType == SCE_GE_CMD.SCE_GE_CMD_RET)
                break;

            bs.Position -= 4;

            byte[] buffer = bs.ReadBytes(0x04);
            BitStream bitStream = new BitStream(BitStreamMode.Read, buffer, BitStreamSignificantBitOrder.MSB);

            // Should be the accurate order
            SCE_GE_CMD_BASE cmd = cmdType switch
            {
                SCE_GE_CMD.SCE_GE_CMD_CBP => new SCE_GE_CBP(),
                SCE_GE_CMD.SCE_GE_CMD_CBW => new SCE_GE_CBW(),
                SCE_GE_CMD.SCE_GE_CMD_TBP0 => new SCE_GE_TBP0(),
                SCE_GE_CMD.SCE_GE_CMD_TBW0 => new SCE_GE_TBW0(),
                SCE_GE_CMD.SCE_GE_CMD_TSIZE0 => new SCE_GE_TSIZE0(),
                SCE_GE_CMD.SCE_GE_CMD_SU => new SCE_GE_SU(),
                SCE_GE_CMD.SCE_GE_CMD_SV => new SCE_GE_SV(),
                SCE_GE_CMD.SCE_GE_CMD_TU => new SCE_GE_TU(),
                SCE_GE_CMD.SCE_GE_CMD_TV => new SCE_GE_TV(),
                SCE_GE_CMD.SCE_GE_CMD_TMAP => new SCE_GE_TMAP(),
                SCE_GE_CMD.SCE_GE_CMD_TMODE => new SCE_GE_TMODE(),
                SCE_GE_CMD.SCE_GE_CMD_TPF => new SCE_GE_TPF(),
                SCE_GE_CMD.SCE_GE_CMD_TFILTER => new SCE_GE_TFILTER(),
                SCE_GE_CMD.SCE_GE_CMD_TWRAP => new SCE_GE_TWRAP(),
                SCE_GE_CMD.SCE_GE_CMD_TLEVEL => new SCE_GE_TLEVEL(),
                SCE_GE_CMD.SCE_GE_CMD_TFUNC => new SCE_GE_TFUNC(),
                SCE_GE_CMD.SCE_GE_CMD_TEC => new SCE_GE_TEC(),
                SCE_GE_CMD.SCE_GE_CMD_CLUT => new SCE_GE_CLUT(),
                SCE_GE_CMD.SCE_GE_CMD_CLOAD => new SCE_GE_CLOAD(),
                SCE_GE_CMD.SCE_GE_CMD_TFLUSH => new SCE_GE_TFLUSH(),
                _ => throw new NotImplementedException($"GE CMD Type {cmdType} not yet implemented for texture info"),
            };

            cmd.Read(ref bitStream);
            Commands.Add(cmdType, cmd);
        }
    }
}
