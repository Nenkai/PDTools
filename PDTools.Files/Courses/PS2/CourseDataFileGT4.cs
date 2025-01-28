using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using PDTools.Files.Models.PS2;
using PDTools.Files.Models.PS2.ModelSet;
using PDTools.Files.Textures.PS2;

using Syroot.BinaryData;

namespace PDTools.Files.Courses.PS2;

/// <summary>
/// Represents a course data file (GT4).
/// </summary>
public class CourseDataFileGT4
{
    /// <summary>
    /// Main model
    /// </summary>
    public ModelSet2 World { get; set; }

    /// <summary>
    /// Scenery Model (lights, banners, signs, buildings)
    /// </summary>
    public ModelSet2 Environment { get; set; }

    public ModelSet2 Reflection { get; set; }

    /// <summary>
    /// Normally used for road reflection
    /// </summary>
    public ModelSet2 ReflectionMask { get; set; }

    public ModelSet2 After { get; set; }

    /// <summary>
    /// Sky Model
    /// </summary>
    public ModelSet2 Sky { get; set; }

    /// <summary>
    /// Scenery/Sky Model
    /// </summary>
    public ModelSet2 EnvSky { get; set; }

    /// <summary>
    /// Distant/Far Model
    /// </summary>
    public ModelSet2 Far { get; set; }

    /// <summary>
    /// Sky model (mirror)
    /// </summary>
    public ModelSet2 MirrorSky { get; set; }

    /// <summary>
    /// Smoke texture set.
    /// </summary>
    public TextureSet1 RaceSmoke { get; set; }

    /// <summary>
    /// Minimap.
    /// </summary>
    public MiniMapSet MiniMap { get; set; }

    public TextureSet1 UnusedSphereReflectionTexture { get; set; }

    /// <summary>
    /// Shape of the flare.
    /// </summary>
    public PGLUshape FlareShape { get; set; }

    /// <summary>
    /// Texture of the flare.
    /// </summary>
    public TextureSet1 FlareTexture { get; set; }

    /// <summary>
    /// Particles texture set.
    /// </summary>
    public TextureSet1 ParticleTexture { get; set; }

    /// <summary>
    /// Texture of the flare reflection.
    /// </summary>
    public TextureSet1 FlareReflection { get; set; }

    public ModelSet2 ReflectionUnk0xC8 { get; set; }

    public ModelSet2 FgSky { get; set; }


    /// <summary>
    /// Reads a course data file.
    /// </summary>
    /// <param name="stream"></param>
    /// <returns></returns>
    /// <exception cref="InvalidDataException"></exception>
    public CourseDataFileGT4 Open(string file)
    {
        using var fs = File.OpenRead(file);
        return FromStream(fs);

    }

    /// <summary>
    /// Reads a course data from stream.
    /// </summary>
    /// <param name="stream"></param>
    /// <returns></returns>
    /// <exception cref="InvalidDataException"></exception>
    public CourseDataFileGT4 FromStream(Stream stream)
    {
        long basePos = stream.Position;
        BinaryStream bs = new BinaryStream(stream);

        bs.ByteConverter = ByteConverter.Little;

        CourseDataFileGT4 courseData = new CourseDataFileGT4();

        uint relocPtr = bs.ReadUInt32();

        bs.Position = basePos + 0x04;
        uint world_ = bs.ReadUInt32();                  // World ModelSet - Has explicit assert if missing

        bs.Position = basePos + 0x08;
        uint visionlist_ = bs.ReadUInt32();             // Environment VisionList - Has explicit assert if missing

        bs.Position = basePos + 0x0C;
        uint visionlist_back_mirror_ = bs.ReadUInt32(); // Back Mirror VisionList - Has explicit assert if missing

        bs.Position = basePos + 0x10;
        uint visionlist_2p_ = bs.ReadUInt32();          // 2 Player mode VisionList - Has explicit assert if missing

        bs.Position = basePos + 0x14;
        uint env_ = bs.ReadUInt32();                    // Environment ModelSet - Has explicit assert if missing

        bs.Position = basePos + 0x18;
        uint envvisionlist_ = bs.ReadUInt32();          // Environment VisionList - Has explicit assert if missing

        bs.Position = basePos + 0x1C;
        uint unkVisionList0x1C_ = bs.ReadUInt32();

        bs.Position = basePos + 0x20;
        uint unkVisionList0x20_ = bs.ReadUInt32();

        bs.Position = basePos + 0x24;
        uint reflection_ = bs.ReadUInt32();

        bs.Position = basePos + 0x28;
        uint visionlist_reflection_ = bs.ReadUInt32();

        bs.Position = basePos + 0x34;
        uint reflection_mask_ = bs.ReadUInt32();

        bs.Position = basePos + 0x38;
        uint visionlist_reflection_mask = bs.ReadUInt32();

        bs.Position = basePos + 0x44;
        uint after_ = bs.ReadUInt32();

        bs.Position = basePos + 0x48;
        uint visionlist_after_ = bs.ReadUInt32();

        bs.Position = basePos + 0x4C;
        uint unkVisionList0x4C_ = bs.ReadUInt32();

        bs.Position = basePos + 0x50;
        uint unkVisionList0x50_ = bs.ReadUInt32();

        bs.Position = basePos + 0x54;
        uint sky_ = bs.ReadUInt32();          // ModelSet - Has explicit assert if missing

        bs.Position = basePos + 0x58;
        uint far_ = bs.ReadUInt32();          // ModelSet - Has explicit assert if missing

        bs.Position = basePos + 0x5C;
        uint envsky_ = bs.ReadUInt32();       // ModelSet - Has explicit assert if missing

        bs.Position = basePos + 0x60;
        uint mirror_sky_ = bs.ReadUInt32();   // ModelSet - Has explicit assert if missing

        bs.Position = basePos + 0x64;
        uint unk0x64_ = bs.ReadUInt32();

        bs.Position = basePos + 0x68;
        uint race_smoke_ = bs.ReadUInt32();  // pgluTexSet

        bs.Position = basePos + 0x6C;
        uint unkModelSet_0x6C = bs.ReadUInt32();  // ModelSet2

        bs.Position = basePos + 0x70;
        uint runway_ = bs.ReadUInt32();       // RunwayGT4 - Has explicit assert if missing

        bs.Position = basePos + 0x74;
        uint course_efx_ = bs.ReadUInt32();   // DCourseEffect Has explicit assert if missing

        bs.Position = basePos + 0x78;
        uint billboard_set_ = bs.ReadUInt32();

        bs.Position = basePos + 0x7C;
        uint unk0x7C_ = bs.ReadUInt32();

        bs.Position = basePos + 0x80;
        uint envptr_ = bs.ReadUInt32();       // CourseEnvPtr - Environment Parameter - Has explicit assert if missing

        bs.Position = basePos + 0x84;
        uint minimapset_ = bs.ReadUInt32();   // MiniMapSet - Has explicit assert if missing

        bs.Position = basePos + 0x88;
        uint unused_sphere_reflection_texture_ = bs.ReadUInt32();  // pgluTexSet

        bs.Position = basePos + 0x8C;
        uint sound_ = bs.ReadUInt32();        // CourseSound - Has explicit assert if missing

        bs.Position = basePos + 0x94;
        uint flare_shape_ = bs.ReadUInt32();  // pgluShape

        bs.Position = basePos + 0x98;
        uint flare_texture_ = bs.ReadUInt32();  // pgluTexSet

        bs.Position = basePos + 0x9C;
        uint particle_texture_ = bs.ReadUInt32();  // pgluTexSet

        bs.Position = basePos + 0xA0;
        uint flare_reflection_texture_ = bs.ReadUInt32();  // pgluTexSet

        bs.Position = basePos + 0xA4;
        uint unkGt4ReplayData0xA4_ = bs.ReadUInt32();  // GT4ReplayData

        bs.Position = basePos + 0xA8;
        uint texture_set_0xA8_ = bs.ReadUInt32();  // pgluTexSet

        bs.Position = basePos + 0xAC;
        uint projection_mask_ = bs.ReadUInt32();   // ModelSet

        bs.Position = basePos + 0xB0;
        uint replay_gt4_ = bs.ReadUInt32();  // GT4ReplayData - Has explicit assert if missing

        bs.Position = basePos + 0xB4;
        uint play_start_camera_gt4_ = bs.ReadUInt32();  // GT4ReplayData - Has explicit assert if missing

        bs.Position = basePos + 0xB8;
        uint replay_start_camera_gt4_ = bs.ReadUInt32();  // GT4ReplayData - Has explicit assert if missing

        bs.Position = basePos + 0xBC;
        uint gadget_shape_list_ = bs.ReadUInt32();      // GadgetShapeList

        bs.Position = basePos + 0xC0;
        uint course_preview_camera_ = bs.ReadUInt32();  // GT4ReplayData - Has explicit assert if missing

        bs.Position = basePos + 0xC4;
        uint texture_set_0xC4_ = bs.ReadUInt32();  // pgluTexSet

        bs.Position = basePos + 0xC8;
        uint unkModelSet0xC8_ = bs.ReadUInt32();   // ModelSet

        bs.Position = basePos + 0xCC;
        uint unkPhotoMode0xCC_ = bs.ReadUInt32();  // DPhotoMode

        bs.Position = basePos + 0xD0;
        uint start_camera_ = bs.ReadUInt32();  // GT4ReplayData - Has explicit assert if missing

        bs.Position = basePos + 0xD4;
        uint replay_start_camera_ = bs.ReadUInt32();  // GT4ReplayData - Has explicit assert if missing

        bs.Position = basePos + 0xD8;
        uint fg_sky_ = bs.ReadUInt32(); // ModelSet

        bs.Position = basePos + 0xDC;
        uint unkRunway0xCC = bs.ReadUInt32(); // RunwayGT4 - Has explicit assert if missing

        if (world_ != 0)
        {
            bs.Position = basePos + world_;
            World = new ModelSet2();
            World.FromStream(bs);
        }

        if (env_ != 0)
        {
            bs.Position = basePos + env_;
            Environment = new ModelSet2();
            Environment.FromStream(bs);
        }

        if (reflection_ != 0)
        {
            bs.Position = basePos + reflection_;
            Reflection = new ModelSet2();
            Reflection.FromStream(bs);
        }

        if (reflection_mask_ != 0)
        {
            bs.Position = basePos + reflection_mask_;
            ReflectionMask = new ModelSet2();
            ReflectionMask.FromStream(bs);
        }

        if (after_ != 0)
        {
            bs.Position = basePos + after_;
            After = new ModelSet2();
            After.FromStream(bs);
        }

        if (sky_ != 0)
        {
            bs.Position = basePos + sky_;
            Sky = new ModelSet2();
            Sky.FromStream(bs);
        }

        if (far_ != 0)
        {
            bs.Position = basePos + far_;
            Far = new ModelSet2();
            Far.FromStream(bs);
        }

        if (envsky_ != 0)
        {
            bs.Position = basePos + envsky_;
            EnvSky = new ModelSet2();
            EnvSky.FromStream(bs);
        }

        if (mirror_sky_ != 0)
        {
            bs.Position = basePos + mirror_sky_;
            MirrorSky = new ModelSet2();
            MirrorSky.FromStream(bs);
        }

        if (race_smoke_ != 0)
        {
            bs.Position = basePos + race_smoke_;
            RaceSmoke = new TextureSet1();
            RaceSmoke.FromStream(bs);
        }

        if (minimapset_ != 0)
        {
            bs.Position = basePos + minimapset_;
            MiniMap = new MiniMapSet();
            MiniMap.FromStream(bs);
        }
        
        if (unused_sphere_reflection_texture_ != 0)
        {
            bs.Position = basePos + unused_sphere_reflection_texture_;
            UnusedSphereReflectionTexture = new TextureSet1();
            UnusedSphereReflectionTexture.FromStream(bs);
        }

        if (flare_shape_ != 0)
        {
            bs.Position = basePos + flare_shape_;
            FlareShape = new PGLUshape();
            FlareShape.FromStream(bs);
        }

        if (flare_texture_ != 0)
        {
            bs.Position = basePos + flare_texture_;
            FlareTexture = new TextureSet1();
            FlareTexture.FromStream(bs);
        }

        if (particle_texture_ != 0)
        {
            bs.Position = basePos + particle_texture_;
            ParticleTexture = new TextureSet1();
            ParticleTexture.FromStream(bs);
        }

        if (flare_reflection_texture_ != 0)
        {
            bs.Position = basePos + flare_reflection_texture_;
            FlareReflection = new TextureSet1();
            FlareReflection.FromStream(bs);
        }

        if (unkModelSet0xC8_ != 0)
        {
            bs.Position = basePos + unkModelSet0xC8_;
            ReflectionUnk0xC8 = new ModelSet2();
            ReflectionUnk0xC8.FromStream(bs);
        }

        if (fg_sky_ != 0)
        {
            bs.Position = basePos + fg_sky_;
            FgSky = new ModelSet2();
            FgSky.FromStream(bs);
        }

        return courseData;
    }
}
