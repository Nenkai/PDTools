using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Syroot.BinaryData;

namespace PDTools.Files.Courses.ReplayData;

public class CameraGT4
{
    public uint CameraFlags { get; set; }
    public float VStart { get; set; }
    public float VEnd { get; set; }
    public uint UnkField_0x0C { get; set; }
    public float UnkField_0x10 { get; set; }
    public uint UnkField_0x14 { get; set; }
    public uint UnkField_0x18 { get; set; }
    public float UnkField_0x1C { get; set; }
    public float GammaBrightness { get; set; }
    public float UnkField_0x24 { get; set; }
    public float UnkField_0x28 { get; set; }
    public float UnkField_0x2C { get; set; }
    public float UnkField_0x30 { get; set; }
    public CurveGT4 PositionCurve { get; set; }
    public CurveGT4 HeightCurve { get; set; }
    public CurveGT4 Curve0x3C { get; set; }
    public CurveGT4 Curve0x40 { get; set; }
    public CurveGT4 Curve0x44 { get; set; }
    public CurveGT4 Curve0x48 { get; set; }
    public CurveGT4 CurvePermRotationMaybeLeftRight { get; set; }
    public CurveGT4 FOVCurve { get; set; }
    public CurveGT4 Curve0x50 { get; set; }
    public CurveGT4 Curve0x54 { get; set; }
    public CurveGT4 Curve0x58 { get; set; }
    public float UnkField_0x5C { get; set; }
    public float UnkField_0x60 { get; set; }
    public CurveGT4 Curve0x64 { get; set; }
    public CurveGT4 Curve0x68 { get; set; }
    public CurveGT4 Curve0x6C { get; set; }
    public CurveGT4 Curve0x70 { get; set; }

    public void Read(BinaryStream bs, long basePos, uint version)
    {
        CameraFlags = bs.ReadUInt32();
        VStart = bs.ReadSingle();
        VEnd = bs.ReadSingle();
        UnkField_0x0C = bs.ReadUInt32();
        UnkField_0x10 = bs.ReadSingle();
        UnkField_0x14 = bs.ReadUInt32();
        UnkField_0x18 = bs.ReadUInt32();
        UnkField_0x1C = bs.ReadUInt32();
        GammaBrightness = bs.ReadSingle();
        UnkField_0x24 = bs.ReadSingle();
        UnkField_0x28 = bs.ReadSingle();
        UnkField_0x2C = bs.ReadSingle();
        UnkField_0x30 = bs.ReadSingle();
        uint positionCurveOffset = bs.ReadUInt32();
        uint heightCurveOffset = bs.ReadUInt32();
        uint curve0x3COffset = bs.ReadUInt32();
        uint curve0x40Offset = bs.ReadUInt32();
        uint curve0x44Offset = bs.ReadUInt32();
        uint curve0x48Offset = bs.ReadUInt32();
        uint fovCurveOffset = bs.ReadUInt32();
        uint curvePermrotationMaybeLeftRightOffset = bs.ReadUInt32();
        uint curve0x54Offset = bs.ReadUInt32();
        uint curve0x58Offset = bs.ReadUInt32();
        UnkField_0x5C = bs.ReadSingle();
        UnkField_0x60 = bs.ReadSingle();
        uint curve0x64Offset = bs.ReadUInt32();
        uint curve0x68Offset = bs.ReadUInt32();
        uint curve0x6COffset = bs.ReadUInt32();
        uint curve0x70Offset = bs.ReadUInt32();

        if (positionCurveOffset != 0)
        {
            bs.Position = basePos + positionCurveOffset;
            PositionCurve = new CurveGT4();
            PositionCurve.Read(bs, version);
        }

        if (heightCurveOffset != 0)
        {
            bs.Position = basePos + heightCurveOffset;
            HeightCurve = new CurveGT4();
            HeightCurve.Read(bs, version);
        }

        if (curve0x3COffset != 0)
        {
            bs.Position = basePos + curve0x3COffset;
            Curve0x3C = new CurveGT4();
            Curve0x3C.Read(bs, version);
        }

        if (curve0x40Offset != 0)
        {
            bs.Position = basePos + curve0x40Offset;
            Curve0x40 = new CurveGT4();
            Curve0x40.Read(bs, version);
        }

        if (curve0x44Offset != 0)
        {
            bs.Position = basePos + curve0x44Offset;
            Curve0x44 = new CurveGT4();
            Curve0x44.Read(bs, version);
        }

        if (curve0x48Offset != 0)
        {
            bs.Position = basePos + curve0x48Offset;
            Curve0x48 = new CurveGT4();
            Curve0x48.Read(bs, version);
        }

        if (fovCurveOffset != 0)
        {
            bs.Position = basePos + fovCurveOffset;
            FOVCurve = new CurveGT4();
            FOVCurve.Read(bs, version);
        }

        if (curvePermrotationMaybeLeftRightOffset != 0)
        {
            bs.Position = basePos + curvePermrotationMaybeLeftRightOffset;
            CurvePermRotationMaybeLeftRight = new CurveGT4();
            CurvePermRotationMaybeLeftRight.Read(bs, version);
        }

        if (curve0x54Offset != 0)
        {
            bs.Position = basePos + curve0x54Offset;
            Curve0x54 = new CurveGT4();
            Curve0x54.Read(bs, version);
        }

        if (curve0x58Offset != 0)
        {
            bs.Position = basePos + curve0x58Offset;
            Curve0x58 = new CurveGT4();
            Curve0x58.Read(bs, version);
        }

        if (curve0x64Offset != 0)
        {
            bs.Position = basePos + curve0x64Offset;
            Curve0x64 = new CurveGT4();
            Curve0x64.Read(bs, version);
        }

        if (curve0x68Offset != 0)
        {
            bs.Position = basePos + curve0x68Offset;
            Curve0x68 = new CurveGT4();
            Curve0x68.Read(bs, version);
        }

        if (curve0x6COffset != 0)
        {
            bs.Position = basePos + curve0x6COffset;
            Curve0x6C = new CurveGT4();
            Curve0x6C.Read(bs, version);
        }

        if (curve0x70Offset != 0)
        {
            bs.Position = basePos + curve0x70Offset;
            Curve0x70 = new CurveGT4();
            Curve0x70.Read(bs, version);
        }
    }

    public static uint GetSize(uint version)
    {
        if (version >= 3)
            return 0xD4;
        else
            return 0x7C;
    }
}
