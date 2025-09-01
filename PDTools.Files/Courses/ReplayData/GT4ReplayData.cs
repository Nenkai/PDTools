using PDTools.Files.Courses.Runway;

using Syroot.BinaryData;

using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDTools.Files.Courses.ReplayData;

/// <summary>
/// Camera data (REP4 header). Also used in later games!
/// </summary>
public class GT4ReplayData
{
    /// <summary>
    /// 'REP4' (ReplayGT4)
    /// </summary>
    public static readonly uint MAGIC = BinaryPrimitives.ReadUInt32LittleEndian("REP4"u8);

    public uint Version { get; set; }

    public float VStart { get; set; }
    public float VEnd { get; set; }
    public uint UnkField_0x1C { get; set; }

    public List<CameraGT4> Cameras { get; set; } = [];

    /// <summary>
    /// Note: Unfinished, doesn't read past cameras
    /// </summary>
    /// <param name="stream"></param>
    /// <returns></returns>
    /// <exception cref="InvalidDataException"></exception>
    public static GT4ReplayData FromStream(Stream stream)
    {
        BinaryStream bs = new BinaryStream(stream);
        long basePos = bs.Position;

        GT4ReplayData cam = new GT4ReplayData();
        cam.Read(bs, basePos);
        return cam;
    }

    private void Read(BinaryStream bs, long basePos)
    {
        uint magic = bs.ReadUInt32();
        if (magic != MAGIC)
            throw new InvalidDataException("Invalid magic.");

        bs.ReadUInt32(); // Reloc Ptr
        Version = bs.ReadUInt32();
        uint relocSize = bs.ReadUInt32();

        if (bs.Length < basePos + relocSize)
            throw new InvalidDataException("Stream is too short to fit REP4 file.");

        bs.ReadUInt32();
        VStart = bs.ReadSingle();
        VEnd = bs.ReadSingle();
        UnkField_0x1C = bs.ReadUInt32();
        uint camerasCount = bs.ReadUInt32();
        uint camerasOffset = bs.ReadUInt32();
        uint unkField_0x28 = bs.ReadUInt32();
        uint distributeCameraCount = bs.ReadUInt32();
        uint distributeCameraOffset = bs.ReadUInt32();
        uint unkVCoordsCount = bs.ReadUInt32();
        uint unkVCoordsOffset = bs.ReadUInt32();
        uint sectionLapCount = bs.ReadUInt32();
        uint sectionLapOffset = bs.ReadUInt32();

        for (int i = 0; i < camerasCount; i++)
        {
            bs.Position = basePos + camerasOffset + (i * CameraGT4.GetSize(Version));
            var camera = new CameraGT4();
            camera.Read(bs, basePos, Version);
            Cameras.Add(camera);
        }

        // TODO: More
    }
}