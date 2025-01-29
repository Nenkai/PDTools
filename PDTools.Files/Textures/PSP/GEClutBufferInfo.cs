using Syroot.BinaryData;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDTools.Files.Textures.PSP;

public class GEClutBufferInfo : TextureSet3ClutInfoBase
{
    public SCE_GE_CLUT_CPF ClutType { get; set; }
    public ushort NumColors { get; set; }
    public uint ClutBufferOffset { get; set; }

    public void Read(BinaryStream bs)
    {
        bs.ReadByte();
        ClutType = (SCE_GE_CLUT_CPF)bs.ReadByte();
        NumColors = bs.ReadUInt16();
        ClutBufferOffset = bs.ReadUInt32();
        bs.ReadUInt32();
    }

    public void Write(BinaryStream bs)
    {
        bs.WriteByte(0);
        bs.WriteByte((byte)ClutType);
        bs.WriteUInt16(NumColors);
        bs.WriteUInt32(ClutBufferOffset);
        bs.WriteUInt32(0);
    }
}
