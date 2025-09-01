using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

using Syroot.BinaryData;

namespace PDTools.Files.Courses.ReplayData;

public class CurveGT4
{
    public List<KeyFrame> Points { get; set; } = [];

    public void Read(BinaryStream bs, uint version)
    {
        uint numKeyFrames = bs.ReadUInt32();
        if (version >= 3)
            numKeyFrames /= 2;

        for (int i = 0; i < numKeyFrames; i++)
        {
            var keyFrame = new KeyFrame();
            keyFrame.Read(bs, version);
            Points.Add(keyFrame);
        }
    }

    public Vector3 GetValue(float value)
    {
        int numKeyframes = Points.Count;

        // Binary search for the correct interval
        int left = 0;
        int right = numKeyframes;

        while (left < right)
        {
            int mid = (left + right) / 2;
            if (value < Points[mid].Time)
                right = mid;
            else
                left = mid + 1;
        }

        int idx = left - 1;
        if (idx >= numKeyframes - 1)
            idx = numKeyframes - 2;
        if (idx < 0)
            idx = 0;

        KeyFrame k0 = Points[idx];
        KeyFrame k1 = Points[idx + 1];

        // Linear interpolation
        float t = (value - k0.Time) / (k1.Time - k0.Time);
        return Vector3.Lerp(k0.Value, k1.Value, t);
    }

}

public class KeyFrame
{
    public float Time { get; set; }
    public Vector3 Value { get; set; }

    public void Read(BinaryStream bs, uint version)
    {
        Time = bs.ReadSingle();
        if (version >= 3)
            Value = new Vector3(bs.ReadSingle(), bs.ReadSingle(), bs.ReadSingle());
        else
            Value = new Vector3(bs.ReadSingle(), 0.0f, 0.0f);
    }
}
