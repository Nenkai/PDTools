using System;
using System.Collections.Generic;
using System.Text;
using System.Numerics;

namespace PDTools.Utils
{
    public static class MathUtils
    {
        public static float GetAxis(this Vector3 vec, int axis)
        {
            if (axis == 0)
                return vec.X;
            else if (axis == 1)
                return vec.Y;
            else
                return vec.Z;
        }

        public static void SetAxis(this ref Vector3 vec, int axis, float value)
        {
            if (axis == 0)
                vec.X = value;
            else if (axis == 1)
                vec.Y = value;
            else
                vec.Z = value;
        }

        public static float Lerp(float value1, float value2, float amount)
        {
            return (value1 * (1f - amount)) + (value2 * amount);
        }
    }
}
