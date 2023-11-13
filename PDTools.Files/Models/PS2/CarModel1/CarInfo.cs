using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Text.Json;

using Syroot.BinaryData;
using System.Numerics;

namespace PDTools.Files.Models.PS2.CarModel1
{
    public class CarInfo
    {
        /// <summary>
        /// Magic, "GTCI".
        /// </summary>
        public const uint MAGIC = 0x49435447;
        public const uint HeaderSize = 0x94;

        public BrakeParameter[] BrakeParameters { get; set; } = new BrakeParameter[2]
        {
            new BrakeParameter(),
            new BrakeParameter(),
        };

        public uint DefaultTireIndex { get; set; }
        public List<Light> FrontLights { get; set; } = new();
        public List<Light> NightBrakeLights { get; set; } = new();
        public List<Light> NightBrakeLightFlares { get; set; } = new();
        public List<CollisionParticle> CollisionParticles { get; set; } = new();
        public List<ExhaustData> Exhausts { get; set; } = new List<ExhaustData>();
        public OnboardCameras OnboardCameras { get; set; } = new();
        public TireIndices Tires { get; set; } = new TireIndices();

        public void FromStream(Stream stream)
        {
            long basePos = stream.Position;

            var bs = new BinaryStream(stream, ByteConverter.Little);
            uint magic = bs.ReadUInt32();
            if (magic != MAGIC)
                throw new InvalidDataException("Not a car info header.");

            bs.ReadUInt32(); // Reloc ptr
            bs.ReadUInt32(); // Empty;
            uint size = bs.ReadUInt32();

            if (bs.Length - basePos < size)
                throw new InvalidDataException("Car info size is smaller than remaining stream length.");

            uint versionMaybe = bs.ReadUInt32(); // Always 1
            for (int i = 0; i < 2; i++)
                BrakeParameters[i].FromStream(bs);
            DefaultTireIndex = bs.ReadUInt32();

            uint frontLightCount = bs.ReadUInt32();
            uint frontLightsTableOffset = bs.ReadUInt32();
            uint nightBrakeLightsCount = bs.ReadUInt32();
            uint nightBrakeLightsTableOffset = bs.ReadUInt32();
            uint nightBrakeLightsFlareCount = bs.ReadUInt32();
            uint nightBrakeLightsFlareTableOffset = bs.ReadUInt32();
            uint collisionParticlesCount = bs.ReadUInt32();
            uint collisionParticleTableOffset = bs.ReadUInt32();
            uint unusedExhaustCount = bs.ReadUInt32();
            uint unusedExhaustDataTableOffset = bs.ReadUInt32();
            uint onboardCameraCount = bs.ReadUInt32();
            uint onboardCamerasOffset = bs.ReadUInt32();
            uint tireIndexTableOfset = bs.ReadUInt32();

            for (int i = 0; i < frontLightCount; i++)
            {
                bs.Position = basePos + frontLightsTableOffset + (i * Light.GetSize());
                var light = new Light();
                light.FromStream(bs);
                FrontLights.Add(light);
            }

            for (int i = 0; i < nightBrakeLightsCount; i++)
            {
                bs.Position = basePos + nightBrakeLightsTableOffset + (i * Light.GetSize());
                var light = new Light();
                light.FromStream(bs);
                NightBrakeLights.Add(light);
            }

            for (int i = 0; i < nightBrakeLightsFlareCount; i++)
            {
                bs.Position = basePos + nightBrakeLightsFlareTableOffset + (i * Light.GetSize());
                var light = new Light();
                light.FromStream(bs);
                NightBrakeLightFlares.Add(light);
            }

            for (int i = 0; i < collisionParticlesCount; i++)
            {
                bs.Position = basePos + collisionParticleTableOffset + (i * CollisionParticle.GetSize());
                var collisionParticle = new CollisionParticle();
                collisionParticle.FromStream(bs);
                CollisionParticles.Add(collisionParticle);
            }

            for (int i = 0; i < unusedExhaustCount; i++)
            {
                bs.Position = basePos + unusedExhaustDataTableOffset + (i * ExhaustData.GetSize());
                var exhaust = new ExhaustData();
                exhaust.FromStream(bs);
                Exhausts.Add(exhaust);
            }

            for (int i = 0; i < onboardCameraCount; i++)
            {
                bs.Position = basePos + onboardCamerasOffset + (i * OnboardCameraData.GetSize());
                var data = new OnboardCameraData();
                data.FromStream(bs);
                OnboardCameras.Cameras.Add(data);
            }

            if (tireIndexTableOfset != 0)
            {
                bs.Position = basePos + tireIndexTableOfset;
                Tires.FromStream(bs);
            }
        }

        public void Write(Stream stream)
        {
            long basePos = stream.Position;

            var bs = new BinaryStream(stream, ByteConverter.Little);
            bs.Position = basePos + HeaderSize;

            long frontLightsOffset = bs.Position;
            for (int i = 0; i < FrontLights.Count; i++)
                FrontLights[i].Write(bs);

            long nightLightsOffset = bs.Position;
            for (int i = 0; i < NightBrakeLights.Count; i++)
                NightBrakeLights[i].Write(bs);

            long nightBrakeLightFlaresOffset = bs.Position;
            for (int i = 0; i < NightBrakeLightFlares.Count; i++)
                NightBrakeLightFlares[i].Write(bs);

            long collisionParticlesOffset = bs.Position;
            for (int i = 0; i < CollisionParticles.Count; i++)
                CollisionParticles[i].Write(bs);

            long exhaustDataOffset = bs.Position;
            for (int i = 0; i < Exhausts.Count; i++)
                Exhausts[i].Write(bs);

            long onboardCameraDataOffset = bs.Position;
            OnboardCameras.Write(bs);

            long tireFileIndicesOffset = bs.Position;
            Tires.Write(bs);

            long lastPos = bs.Position;
            long fileSize = lastPos - basePos;

            // Write header
            bs.Position = basePos;
            bs.WriteUInt32(MAGIC);
            bs.WriteUInt32(0);
            bs.WriteUInt32(0);
            bs.WriteUInt32((uint)fileSize + 0x40);
            bs.WriteUInt32(1);
            for (int i = 0; i < 2; i++)
                BrakeParameters[i].Write(bs);
            bs.WriteUInt32(DefaultTireIndex);
            bs.WriteUInt32((uint)FrontLights.Count);
            bs.WriteUInt32((uint)(frontLightsOffset - basePos));
            bs.WriteUInt32((uint)NightBrakeLights.Count);
            bs.WriteUInt32((uint)(nightLightsOffset - basePos));
            bs.WriteUInt32((uint)NightBrakeLightFlares.Count);
            bs.WriteUInt32((uint)(nightBrakeLightFlaresOffset - basePos));
            bs.WriteUInt32((uint)CollisionParticles.Count);
            bs.WriteUInt32((uint)(collisionParticlesOffset - basePos));
            bs.WriteUInt32((uint)Exhausts.Count);
            bs.WriteUInt32((uint)(exhaustDataOffset - basePos));
            bs.WriteUInt32((uint)OnboardCameras.Cameras.Count);
            bs.WriteUInt32((uint)(onboardCameraDataOffset - basePos));
            bs.WriteUInt32((uint)(tireFileIndicesOffset - basePos));

            bs.Position = lastPos;
        }

        public string AsJson()
        {
            return JsonSerializer.Serialize(this, typeof(CarInfo), new JsonSerializerOptions() { WriteIndented = true, IncludeFields = true });
        }

        public static CarInfo FromJson(string json)
        {
            return JsonSerializer.Deserialize<CarInfo>(json, new JsonSerializerOptions() { WriteIndented = true, IncludeFields = true });
        }
    }

    public class BrakeParameter
    {
        public uint BrakeCaliperTextureIndex { get; set; }
        public uint BrakeDiscTextureIndex { get; set; }
        public float BrakeTextureSize { get; set; }
        public float BrakeOffsetFromCenter { get; set; }
        public float BrakeTextureOrientationDeg { get; set; }

        public void FromStream(BinaryStream bs)
        {
            BrakeCaliperTextureIndex = bs.ReadUInt32();
            BrakeDiscTextureIndex = bs.ReadUInt32();
            BrakeTextureSize = bs.ReadSingle();
            BrakeOffsetFromCenter = bs.ReadSingle();
            BrakeTextureOrientationDeg = bs.ReadSingle();
        }

        public void Write(BinaryStream bs)
        {
            bs.WriteUInt32(BrakeCaliperTextureIndex);
            bs.WriteUInt32(BrakeDiscTextureIndex);
            bs.WriteSingle(BrakeTextureSize);
            bs.WriteSingle(BrakeOffsetFromCenter);
            bs.WriteSingle(BrakeTextureOrientationDeg);
        }
    }
}
