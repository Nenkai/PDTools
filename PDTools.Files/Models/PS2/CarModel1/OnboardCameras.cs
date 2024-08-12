using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json.Serialization;

using Syroot.BinaryData;

namespace PDTools.Files.Models.PS2.CarModel1;

public class OnboardCameras
{
    [JsonIgnore]
    public List<OnboardCameraData> Cameras { get; set; } = new List<OnboardCameraData>();

    public OnboardCameraData DEFAULT
    {
        get => Cameras.Count >= 1 ? Cameras[0] : null;
        set => SetCamera(0, value);
    }

    public OnboardCameraData CHASE
    {
        get => Cameras.Count >= 2 ? Cameras[1] : null;
        set => SetCamera(1, value);
    }

    public OnboardCameraData UNK_2
    {
        get => Cameras.Count >= 3 ? Cameras[2] : null;
        set => SetCamera(2, value);
    }

    public OnboardCameraData MIRROR_L
    {
        get => Cameras.Count >= 1 ? Cameras[3] : null;
        set => SetCamera(3, value);
    }

    public OnboardCameraData MIRROR_R
    {
        get => Cameras.Count >= 1 ? Cameras[4] : null;
        set => SetCamera(4, value);
    }

    public OnboardCameraData NOSE
    {
        get => Cameras.Count >= 6 ? Cameras[5] : null;
        set => SetCamera(5, value);
    }

    public OnboardCameraData BONNET
    {
        get => Cameras.Count >= 7 ? Cameras[6] : null;
        set => SetCamera(6, value);
    }

    public OnboardCameraData ROOF
    {
        get => Cameras.Count >= 8 ? Cameras[7] : null;
        set => SetCamera(7, value);
    }

    public OnboardCameraData BACK
    {
        get => Cameras.Count >= 9 ? Cameras[8] : null;
        set => SetCamera(8, value);
    }

    public OnboardCameraData TAIL
    {
        get => Cameras.Count >= 10 ? Cameras[9] : null;
        set => SetCamera(9, value);
    }

    public OnboardCameraData SIDE_L
    {
        get => Cameras.Count >= 11 ? Cameras[10] : null;
        set => SetCamera(10, value);
    }

    public OnboardCameraData SIDE_R
    {
        get => Cameras.Count >= 12 ? Cameras[11] : null;
        set => SetCamera(11, value);
    }

    public OnboardCameraData FENDER_L
    {
        get => Cameras.Count >= 13 ? Cameras[12] : null;
        set => SetCamera(12, value);
    }

    public OnboardCameraData FENDER_R
    {
        get => Cameras.Count >= 14 ? Cameras[13] : null;
        set => SetCamera(13, value);
    }

    public OnboardCameraData WHEEL_FL
    {
        get => Cameras.Count >= 15 ? Cameras[14] : null;
        set => SetCamera(14, value);
    }

    public OnboardCameraData WHEEL_FR
    {
        get => Cameras.Count >= 16 ? Cameras[15] : null;
        set => SetCamera(15, value);
    }

    public OnboardCameraData WHEEL_RL
    {
        get => Cameras.Count >= 17 ? Cameras[16] : null;
        set => SetCamera(16, value);
    }

    public OnboardCameraData WHEEL_RR
    {
        get => Cameras.Count >= 18 ? Cameras[17] : null;
        set => SetCamera(17, value);
    }

    public OnboardCameraData OPTION_1
    {
        get => Cameras.Count >= 19 ? Cameras[18] : null;
        set => SetCamera(18, value);
    }

    public OnboardCameraData OPTION_2
    {
        get => Cameras.Count >= 20 ? Cameras[19] : null;
        set => SetCamera(19, value);
    }

    public void SetCamera(int index, OnboardCameraData data)
    {
        while (Cameras.Count < index + 1)
            Cameras.Add(new OnboardCameraData());

        Cameras[index] = data;
    }

    public void Write(BinaryStream bs)
    {
        foreach (var cam in Cameras)
            cam.Write(bs);
    }
}

public class OnboardCameraData
{
    public Vector3 Position { get; set; }
    public float Pitch { get; set; }
    public float Yaw { get; set; }
    public float Roll { get; set; }
    public float FoV { get; set; }
    public uint Unk { get; set; }

    public void FromStream(BinaryStream bs)
    {
        Position = new Vector3(bs.ReadSingle(), bs.ReadSingle(), bs.ReadSingle());
        Pitch = bs.ReadSingle();
        Yaw = bs.ReadSingle();
        Roll = bs.ReadSingle();
        FoV = bs.ReadSingle();
        Unk = bs.ReadUInt32();

        bs.Position += 0x20; // Empty
    }

    public void Write(BinaryStream bs)
    {
        bs.WriteSingle(Position.X); bs.WriteSingle(Position.Y); bs.WriteSingle(Position.Z);
        bs.WriteSingle(Pitch);
        bs.WriteSingle(Yaw);
        bs.WriteSingle(Roll);
        bs.WriteSingle(FoV);
        bs.WriteUInt32(Unk);

        bs.Position += 0x20;
    }

    public static uint GetSize()
    {
        return 0x40;
    }
}
