using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;

using Syroot.BinaryData;

namespace PDTools.Files.Models.PS2.CarModel1;

public class Light
{
    public float[] UnkData { get; set; } = new float[8];
    public Vector4 Intensities { get; set; }
    public Vector4 Position { get; set; }
    public float UnkMin { get; set; }
    public float UnkMax { get; set; }
    public Vector4 UnkVec { get; set; }
    public Color4f FlareColor { get; set; }
    public uint Unk { get; set; }

    public void FromStream(BinaryStream bs)
    {
        UnkData = bs.ReadSingles(8);
        Intensities = new Vector4(bs.ReadSingle(), bs.ReadSingle(), bs.ReadSingle(), bs.ReadSingle());
        Position = new Vector4(bs.ReadSingle(), bs.ReadSingle(), bs.ReadSingle(), bs.ReadSingle());
        UnkMin = bs.ReadSingle();
        UnkMax = bs.ReadSingle();
        UnkVec = new Vector4(bs.ReadSingle(), bs.ReadSingle(), bs.ReadSingle(), bs.ReadSingle());
        FlareColor = new Color4f(bs.ReadSingle(), bs.ReadSingle(), bs.ReadSingle(), bs.ReadSingle());
        Unk = bs.ReadUInt32();
        bs.Position += 0x14; // Empty
    }

    public void Write(BinaryStream bs)
    {
        bs.WriteSingles(UnkData);
        bs.WriteSingle(Intensities.X); bs.WriteSingle(Intensities.Y); bs.WriteSingle(Intensities.Z); bs.WriteSingle(Intensities.W);
        bs.WriteSingle(Position.X); bs.WriteSingle(Position.Y); bs.WriteSingle(Position.Z); bs.WriteSingle(Position.W);
        bs.WriteSingle(UnkMin);
        bs.WriteSingle(UnkMax);
        bs.WriteSingle(UnkVec.X); bs.WriteSingle(UnkVec.Y); bs.WriteSingle(UnkVec.Z); bs.WriteSingle(UnkVec.W);
        bs.WriteSingle(FlareColor.R); bs.WriteSingle(FlareColor.G); bs.WriteSingle(FlareColor.B); bs.WriteSingle(FlareColor.A);
        bs.WriteUInt32(Unk);
        bs.Position += 0x14;
    }

    public static uint GetSize()
    {
        return 0x80;
    }
}
