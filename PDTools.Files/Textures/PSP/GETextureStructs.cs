using PDTools.Utils;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDTools.Files.Textures.PSP;

public abstract class SCE_GE_CMD_BASE
{
    public SCE_GE_CMD cmd;

    public abstract void Read(ref BitStream bs);
    public abstract void Write(ref BitStream bs);
}

public class SCE_GE_TMAP : SCE_GE_CMD_BASE
{
    public SCE_GE_TMAP_TMN tmn;
    public SCE_GE_TMAP_TMI tmi;

    public override void Read(ref BitStream bs)
    {
        tmn = (SCE_GE_TMAP_TMN)bs.ReadBits(2);
        bs.ReadBits(6);
        tmi = (SCE_GE_TMAP_TMI)bs.ReadBits(2);
        bs.ReadBits(14);
        cmd = (SCE_GE_CMD)bs.ReadBits(8);
    }

    public override void Write(ref BitStream bs)
    {
        bs.WriteBits((byte)tmn, 2);
        bs.WriteBits(0, 6);
        bs.WriteBits((byte)tmi, 2);
        bs.WriteBits(0, 14);
        bs.WriteBits((byte)SCE_GE_CMD.SCE_GE_CMD_TMAP, 8);
    }
}

public class SCE_GE_TSHADE : SCE_GE_CMD_BASE
{
    public byte u;
    public byte v;

    public override void Read(ref BitStream bs)
    {
        u = (byte)bs.ReadBits(2);
        bs.ReadBits(6);
        v = (byte)bs.ReadBits(2);
        bs.ReadBits(14);
        cmd = (SCE_GE_CMD)bs.ReadBits(8);
    }

    public override void Write(ref BitStream bs)
    {
        bs.WriteBits(u, 2);
        bs.WriteBits(0, 6);
        bs.WriteBits(v, 2);
        bs.WriteBits(0, 14);
        bs.WriteBits((byte)SCE_GE_CMD.SCE_GE_CMD_TSHADE, 8);
    }
}

public class SCE_GE_TMODE : SCE_GE_CMD_BASE
{
    public SCE_GE_TMODE_HSM hsm;
    public bool mc;
    public byte mxl;

    public override void Read(ref BitStream bs)
    {
        hsm = (SCE_GE_TMODE_HSM)bs.ReadBits(1);
        bs.ReadBits(7);
        mc = bs.ReadBits(1) == 1;
        bs.ReadBits(7);
        mxl = (byte)bs.ReadBits(3);
        bs.ReadBits(5);
        cmd = (SCE_GE_CMD)bs.ReadBits(8);
    }

    public override void Write(ref BitStream bs)
    {
        bs.WriteBits((byte)hsm, 1);
        bs.WriteBits(0, 7);
        bs.WriteBits(mc ? 1ul : 0ul, 1);
        bs.WriteBits(0, 7);
        bs.WriteBits(mxl, 3);
        bs.WriteBits(0, 5);
        bs.WriteBits((byte)SCE_GE_CMD.SCE_GE_CMD_TMODE, 8);
    }
}

public class SCE_GE_TPF : SCE_GE_CMD_BASE
{
    public eSCE_GE_TPF tpf;
    public ushort ext;

    public override void Read(ref BitStream bs)
    {
        tpf = (eSCE_GE_TPF)bs.ReadBits(8);
        ext = (ushort)bs.ReadBits(16);
        cmd = (SCE_GE_CMD)bs.ReadBits(8);
    }

    public override void Write(ref BitStream bs)
    {
        bs.WriteBits((byte)tpf, 8);
        bs.WriteBits(ext, 16);
        bs.WriteBits((byte)SCE_GE_CMD.SCE_GE_CMD_TPF, 8);
    }
}

public class SCE_GE_CLOAD : SCE_GE_CMD_BASE
{
    public byte np;

    public override void Read(ref BitStream bs)
    {
        np = (byte)bs.ReadBits(8);
        bs.ReadBits(16);
        cmd = (SCE_GE_CMD)bs.ReadBits(8);
    }

    public override void Write(ref BitStream bs)
    {
        bs.WriteBits(np, 8);
        bs.WriteBits(0, 16);
        bs.WriteBits((byte)SCE_GE_CMD.SCE_GE_CMD_CLOAD, 8);
    }
}

public class SCE_GE_CLUT : SCE_GE_CMD_BASE
{
    public SCE_GE_CLUT_CPF cpf;
    public byte sft;
    public byte msk;
    public byte csa;

    public override void Read(ref BitStream bs)
    {
        cpf = (SCE_GE_CLUT_CPF)bs.ReadBits(2);
        sft = (byte)bs.ReadBits(6);
        msk = (byte)bs.ReadBits(8);
        csa = (byte)bs.ReadBits(8);
        cmd = (SCE_GE_CMD)bs.ReadBits(8);
    }

    public override void Write(ref BitStream bs)
    {
        bs.WriteBits((byte)cpf, 2);
        bs.WriteBits(sft, 6);
        bs.WriteBits(msk, 8);
        bs.WriteBits(csa, 8);
        bs.WriteBits((byte)SCE_GE_CMD.SCE_GE_CMD_CLUT, 8);
    }
}

public class SCE_GE_TFILTER : SCE_GE_CMD_BASE
{
    public eSCE_GE_TFILTER min;
    public eSCE_GE_TFILTER mag;

    public override void Read(ref BitStream bs)
    {
        min = (eSCE_GE_TFILTER)bs.ReadBits(3);
        bs.ReadBits(5);
        mag = (eSCE_GE_TFILTER)bs.ReadBits(1);
        bs.ReadBits(15);
        cmd = (SCE_GE_CMD)bs.ReadBits(8);
    }

    public override void Write(ref BitStream bs)
    {
        bs.WriteBits((byte)min, 3);
        bs.WriteBits(0, 5);
        bs.WriteBits((byte)mag, 1);
        bs.WriteBits(0, 15);
        bs.WriteBits((byte)SCE_GE_CMD.SCE_GE_CMD_TFILTER, 8);
    }
}

public class SCE_GE_TWRAP : SCE_GE_CMD_BASE
{
    public eSCE_GE_TWRAP wms;
    public eSCE_GE_TWRAP wmt;

    public override void Read(ref BitStream bs)
    {
        wms = (eSCE_GE_TWRAP)bs.ReadBits(1);
        bs.ReadBits(7);
        wmt = (eSCE_GE_TWRAP)bs.ReadBits(1);
        bs.ReadBits(15);
        cmd = (SCE_GE_CMD)bs.ReadBits(8);
    }

    public override void Write(ref BitStream bs)
    {
        bs.WriteBits((byte)wms, 1);
        bs.WriteBits(0, 7);
        bs.WriteBits((byte)wmt, 1);
        bs.WriteBits(0, 15);
        bs.WriteBits((byte)SCE_GE_CMD.SCE_GE_CMD_TWRAP, 8);
    }
}

public class SCE_GE_TLEVEL : SCE_GE_CMD_BASE
{
    public eSCE_GE_TLEVEL lcm;
    public byte offl;

    public override void Read(ref BitStream bs)
    {
        lcm = (eSCE_GE_TLEVEL)bs.ReadBits(2);
        bs.ReadBits(14);
        offl = (byte)bs.ReadBits(8);
        cmd = (SCE_GE_CMD)bs.ReadBits(8);
    }

    public override void Write(ref BitStream bs)
    {
        bs.WriteBits((byte)lcm, 2);
        bs.WriteBits(0, 14);
        bs.WriteBits(offl, 8);
        bs.WriteBits((byte)SCE_GE_CMD.SCE_GE_CMD_TLEVEL, 8);
    }
}

public class SCE_GE_TFUNC : SCE_GE_CMD_BASE
{
    public SCE_GE_TFUNC_TXF txf;
    public SCE_GE_TFUNC_TCC tcc;
    public byte cd;

    public override void Read(ref BitStream bs)
    {
        txf = (SCE_GE_TFUNC_TXF)bs.ReadBits(3);
        bs.ReadBits(5);
        tcc = (SCE_GE_TFUNC_TCC)bs.ReadBits(1);
        bs.ReadBits(7);
        cd = (byte)bs.ReadBits(1);
        bs.ReadBits(7);
        cmd = (SCE_GE_CMD)bs.ReadBits(8);
    }

    public override void Write(ref BitStream bs)
    {
        bs.WriteBits((byte)txf, 3);
        bs.WriteBits(0, 5);
        bs.WriteBits((byte)tcc, 1);
        bs.WriteBits(0, 7);
        bs.WriteBits(cd, 1);
        bs.WriteBits(0, 7);
        bs.WriteBits((byte)SCE_GE_CMD.SCE_GE_CMD_TFUNC, 8);
    }
}

public class SCE_GE_TEC : SCE_GE_CMD_BASE
{
    public byte r;
    public byte g;
    public byte b;

    public override void Read(ref BitStream bs)
    {
        r = (byte)bs.ReadBits(8);
        g = (byte)bs.ReadBits(8);
        b = (byte)bs.ReadBits(8);
        cmd = (SCE_GE_CMD)bs.ReadBits(8);
    }

    public override void Write(ref BitStream bs)
    {
        bs.WriteBits((byte)r, 8);
        bs.WriteBits((byte)g, 8);
        bs.WriteBits((byte)b, 8);
        bs.WriteBits((byte)SCE_GE_CMD.SCE_GE_CMD_TEC, 8);
    }
}

public class SCE_GE_CBP : SCE_GE_CMD_BASE
{
    public uint cbp;

    public override void Read(ref BitStream bs)
    {
        cbp = (uint)bs.ReadBits(24);
        cmd = (SCE_GE_CMD)bs.ReadBits(8);
    }

    public override void Write(ref BitStream bs)
    {
        bs.WriteBits(cbp, 24);
        bs.WriteBits((byte)SCE_GE_CMD.SCE_GE_CMD_CBP, 8);
    }
}

public class SCE_GE_CBW : SCE_GE_CMD_BASE
{
    public uint cbw;

    public override void Read(ref BitStream bs)
    {
        cbw = (uint)bs.ReadBits(24);
        cmd = (SCE_GE_CMD)bs.ReadBits(8);
    }

    public override void Write(ref BitStream bs)
    {
        bs.WriteBits(cbw, 24);
        bs.WriteBits((byte)SCE_GE_CMD.SCE_GE_CMD_CBW, 8);
    }
}

public class SCE_GE_TBP0 : SCE_GE_CMD_BASE
{
    public uint tbp;

    public override void Read(ref BitStream bs)
    {
        tbp = (uint)bs.ReadBits(24);
        cmd = (SCE_GE_CMD)bs.ReadBits(8);
    }

    public override void Write(ref BitStream bs)
    {
        bs.WriteBits(tbp, 24);
        bs.WriteBits((byte)SCE_GE_CMD.SCE_GE_CMD_TBP0, 8);
    }
}

public class SCE_GE_TBW0 : SCE_GE_CMD_BASE
{
    public uint tbw;

    public override void Read(ref BitStream bs)
    {
        tbw = (uint)bs.ReadBits(24);
        cmd = (SCE_GE_CMD)bs.ReadBits(8);
    }

    public override void Write(ref BitStream bs)
    {
        bs.WriteBits(tbw, 24);
        bs.WriteBits((byte)SCE_GE_CMD.SCE_GE_CMD_TBW0, 8);
    }
}

public class SCE_GE_TSIZE0 : SCE_GE_CMD_BASE
{
    public byte tw;
    public byte th;

    public override void Read(ref BitStream bs)
    {
        tw = (byte)bs.ReadBits(8);
        th = (byte)bs.ReadBits(8);
        bs.ReadBits(8);
        cmd = (SCE_GE_CMD)bs.ReadBits(8);
    }

    public override void Write(ref BitStream bs)
    {
        bs.WriteBits(tw, 8);
        bs.WriteBits(th, 8);
        bs.WriteBits(0, 8);
        bs.WriteBits((byte)SCE_GE_CMD.SCE_GE_CMD_TSIZE0, 8);
    }
}

public class SCE_GE_SU : SCE_GE_CMD_BASE
{
    public uint su;

    public override void Read(ref BitStream bs)
    {
        su = (uint)bs.ReadBits(24);
        cmd = (SCE_GE_CMD)bs.ReadBits(8);
    }

    public override void Write(ref BitStream bs)
    {
        bs.WriteBits(su, 24);
        bs.WriteBits((byte)SCE_GE_CMD.SCE_GE_CMD_SU, 8);
    }
}

public class SCE_GE_SV : SCE_GE_CMD_BASE
{
    public uint sv;

    public override void Read(ref BitStream bs)
    {
        sv = (uint)bs.ReadBits(24);
        cmd = (SCE_GE_CMD)bs.ReadBits(8);
    }

    public override void Write(ref BitStream bs)
    {
        bs.WriteBits(sv, 24);
        bs.WriteBits((byte)SCE_GE_CMD.SCE_GE_CMD_SV, 8);
    }
}

public class SCE_GE_TU : SCE_GE_CMD_BASE
{
    public uint tu;

    public override void Read(ref BitStream bs)
    {
        tu = (uint)bs.ReadBits(24);
        cmd = (SCE_GE_CMD)bs.ReadBits(8);
    }

    public override void Write(ref BitStream bs)
    {
        bs.WriteBits(tu, 24);
        bs.WriteBits((byte)SCE_GE_CMD.SCE_GE_CMD_TU, 8);
    }
}

public class SCE_GE_TV : SCE_GE_CMD_BASE
{
    public uint tv;

    public override void Read(ref BitStream bs)
    {
        tv = (uint)bs.ReadBits(24);
        cmd = (SCE_GE_CMD)bs.ReadBits(8);
    }

    public override void Write(ref BitStream bs)
    {
        bs.WriteBits(tv, 24);
        bs.WriteBits((byte)SCE_GE_CMD.SCE_GE_CMD_TV, 8);
    }
}

public class SCE_GE_TFLUSH : SCE_GE_CMD_BASE
{
    public override void Read(ref BitStream bs)
    {
        bs.ReadBits(24);
        cmd = (SCE_GE_CMD)bs.ReadBits(8);
    }

    public override void Write(ref BitStream bs)
    {
        bs.WriteBits(0, 24);
        bs.WriteBits((byte)SCE_GE_CMD.SCE_GE_CMD_TFLUSH, 8);
    }
}