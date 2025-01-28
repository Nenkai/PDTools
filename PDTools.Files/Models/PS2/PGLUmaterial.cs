using Syroot.BinaryData;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDTools.Files.Models.PS2;

public class PGLUmaterial
{
    public Color4f Ambient { get; set; } = Color4f.One;
    public Color4f Diffuse { get; set; } = Color4f.One;
    public Color4f Specular { get; set; } = Color4f.Zero;
    public Color4f UnkColor { get; set; } = new Color4f(0f, 0f, 0f, 1f);
    public float Unk { get; set; }
    public uint UnkFlags { get; set; }
    public float Unk2 { get; set; } = 127;
    public float Unk3 { get; set; } = 1;

    public void FromStream(BinaryStream bs, long mdlBasePos)
    {
        Ambient = new Color4f(bs.ReadSingle(), bs.ReadSingle(), bs.ReadSingle(), bs.ReadSingle());
        Diffuse = new Color4f(bs.ReadSingle(), bs.ReadSingle(), bs.ReadSingle(), bs.ReadSingle());
        Specular = new Color4f(bs.ReadSingle(), bs.ReadSingle(), bs.ReadSingle(), bs.ReadSingle());
        UnkColor = new Color4f(bs.ReadSingle(), bs.ReadSingle(), bs.ReadSingle(), bs.ReadSingle());

        Unk = bs.ReadSingle();
        UnkFlags = bs.ReadUInt32();
        Unk2 = bs.ReadSingle();
        Unk3 = bs.ReadSingle();
    }

    public void Write(BinaryStream bs)
    {
        bs.WriteSingle(Ambient.R); bs.WriteSingle(Ambient.G); bs.WriteSingle(Ambient.B); bs.WriteSingle(Ambient.A);
        bs.WriteSingle(Diffuse.R); bs.WriteSingle(Diffuse.G); bs.WriteSingle(Diffuse.B); bs.WriteSingle(Diffuse.A);
        bs.WriteSingle(Specular.R); bs.WriteSingle(Specular.G); bs.WriteSingle(Specular.B); bs.WriteSingle(Specular.A);
        bs.WriteSingle(UnkColor.R); bs.WriteSingle(UnkColor.G); bs.WriteSingle(UnkColor.B); bs.WriteSingle(UnkColor.A);

        bs.WriteSingle(Unk);
        bs.WriteUInt32(UnkFlags);
        bs.WriteSingle(Unk2);
        bs.WriteSingle(Unk3);
    }

    public override int GetHashCode()
    {
        unchecked
        {
            int hashcode = 1430287;
            hashcode = hashcode * 7302013 ^ Ambient.GetHashCode();
            hashcode = hashcode * 7302013 ^ Diffuse.GetHashCode();
            hashcode = hashcode * 7302013 ^ Specular.GetHashCode();
            hashcode = hashcode * 7302013 ^ UnkColor.GetHashCode();
            hashcode = hashcode * 7302013 ^ Unk.GetHashCode();
            hashcode = hashcode * 7302013 ^ UnkFlags.GetHashCode();
            hashcode = hashcode * 7302013 ^ Unk2.GetHashCode();
            hashcode = hashcode * 7302013 ^ Unk3.GetHashCode();
            return hashcode;
        }
    }

    public static int GetSize()
    {
        return 0x50;
    }
}
