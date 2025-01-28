using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Syroot.BinaryData;

namespace PDTools.Files.Models.PS2;

public class GIFTag
{
    public ushort NLoop { get; set; }
    public bool EndOfPacket { get; set; }
    public ushort Id { get; set; }
    public bool Pre { get; set; }
    public SCE_GS_PRIM Prim { get; set; }
    public byte Flag { get; set; }
    public List<byte> Regs { get; set; } = [];

    public void FromStream(BinaryStream bs)
    {
        ushort flags = bs.ReadUInt16();
        NLoop = (ushort)(flags & 0b111_1111_1111_1111);
        EndOfPacket = ((flags >> 15) & 1) != 0;

        bs.ReadUInt16(); // pad

        uint flags2 = bs.ReadUInt32();
        Id = (ushort)(flags2 & 0b11_1111_1111_1111);
        Pre = ((flags2 >> 14) & 1) != 0;
        Prim = (SCE_GS_PRIM)((flags2 >> 15) & 0b111_1111_1111);
        Flag = (byte)((flags2 >> 26) & 0b11);

        byte nreg = (byte)((flags2 >> 28) & 0b1111);

        uint regs = bs.ReadUInt32();
        int bitIndex = 0;
        for (int i = 0; i < 4; i++)
        {
            Regs.Add((byte)((regs >> bitIndex) & 0b1111));
            bitIndex += 4;
        }
    }

    public void Write(BinaryStream bs)
    {
        bs.WriteUInt16((ushort)(((EndOfPacket ? 1 : 0) << 15) | (NLoop & 0b111_1111_1111_1111)));
        bs.WriteUInt16(0); // pad
        bs.WriteUInt32((uint)(((3 & 0b1111) << 28) |
            ((Flag & 0b11) << 26) |
            (((ushort)Prim & 0b111_1111_1111) << 15) |
            ((Pre ? 1 : 0) << 14) |
            (Id & 0b11_1111_1111_1111)));

        uint regs = 0;
        int bitIndex = 0;
        for (int i = 0; i < 4; i++)
        {
            regs |= (uint)((Regs[i] & 0b1111) << bitIndex);
            bitIndex += 4;
        }
        bs.WriteUInt32(regs);
    }
}

[Flags]
public enum SCE_GS_PRIM : ushort
{
    /// <summary>
    /// Point
    /// </summary>
    SCE_GS_PRIM_POINT	  = 0x00,

    /// <summary>
    /// Line
    /// </summary>
    SCE_GS_PRIM_LINE	  = 0x01,

    /// <summary>
    /// Line strips
    /// </summary>
    SCE_GS_PRIM_LINESTRIP = 0x02,

    /// <summary>
    /// Regular triangles
    /// </summary>
    SCE_GS_PRIM_TRI	      = 0x03,

    /// <summary>
    /// Triangle strips
    /// </summary>
    SCE_GS_PRIM_TRISTRIP  = 0x04,

    /// <summary>
    /// Triangle fan
    /// </summary>
    SCE_GS_PRIM_TRIFAN	  = 0x05,

    /// <summary>
    /// Sprite
    /// </summary>
    SCE_GS_PRIM_SPRITE	  = 0x06,

    ////////////////////////////////

    /// <summary>
    /// Gouraud
    /// </summary>
    SCE_GS_PRIM_IIP  = 1 << 3,

    /// <summary>
    /// Textured
    /// </summary>
    SCE_GS_PRIM_TME  = 1 << 4,

    /// <summary>
    /// Fogging
    /// </summary>
    SCE_GS_PRIM_FGE  = 1 << 5,

    /// <summary>
    /// Alpha Blending
    /// </summary>
    SCE_GS_PRIM_ABE  = 1 << 6,

    /// <summary>
    /// Anti Aliasing
    /// </summary>
    SCE_GS_PRIM_AA1  = 1 << 7, // Anti-Aliasing

    /// <summary>
    /// Use ST for texture coords
    /// </summary>
    SCE_GS_PRIM_FST = 1 << 8,

    /// <summary>
    /// Context
    /// </summary>
    SCE_GS_PRIM_CTX1 = 0,

    SCE_GS_PRIM_CTX2 = 1 << 9,

    /// <summary>
    /// Fragment Control
    /// </summary>
    SCE_GS_PRIM_FIX  = 1 << 10,
}
