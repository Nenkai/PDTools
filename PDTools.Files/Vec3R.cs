using Syroot.BinaryData;
using System.Numerics;

namespace PDTools.Files
{
    public struct Vec3R
    {
        public Vector3 Position { get; set; }
        public float AngleRad { get; set; }

        public const uint Size = 0x10;

        public Vec3R(float x, float y, float z, float angleRadians)
        {
            Position = new Vector3(x, y, z);
            AngleRad = angleRadians;
        }

        public static Vec3R FromStream(BinaryStream bs)
        {
            float x = bs.ReadSingle();
            float y = bs.ReadSingle();
            float z = bs.ReadSingle();
            float r = bs.ReadSingle();
            return new(x, y, z, r);
        }

        public void ToStream(BinaryStream bs)
        {
            bs.WriteSingle(Position.X);
            bs.WriteSingle(Position.Y);
            bs.WriteSingle(Position.Z);
            bs.WriteSingle(AngleRad);
        }
    }
}
