using System.Numerics;

namespace SFDCreator.Rendering.Cameras;

public static class CameraPath
{
    public static (Vector3 Position, Vector3 LookAt) Evaluate(IReadOnlyList<CameraPathKeyframe> keyframes, float time)
    {
        if (keyframes.Count == 0)
        {
            throw new ArgumentException("At least one keyframe is required.", nameof(keyframes));
        }

        if (keyframes.Count == 1 || time <= keyframes[0].Time)
        {
            return (keyframes[0].Position, keyframes[0].LookAt);
        }

        if (time >= keyframes[^1].Time)
        {
            return (keyframes[^1].Position, keyframes[^1].LookAt);
        }

        var segment = 0;
        for (var i = 0; i < keyframes.Count - 1; i++)
        {
            if (time >= keyframes[i].Time && time <= keyframes[i + 1].Time)
            {
                segment = i;
                break;
            }
        }

        var k1 = keyframes[segment];
        var k2 = keyframes[segment + 1];
        var k0 = keyframes[Math.Max(segment - 1, 0)];
        var k3 = keyframes[Math.Min(segment + 2, keyframes.Count - 1)];

        var span = k2.Time - k1.Time;
        var t = span > 0f ? (time - k1.Time) / span : 0f;

        var position = CatmullRom(k0.Position, k1.Position, k2.Position, k3.Position, t);
        var lookAt = CatmullRom(k0.LookAt, k1.LookAt, k2.LookAt, k3.LookAt, t);

        return (position, lookAt);
    }

    private static Vector3 CatmullRom(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float t)
    {
        var t2 = t * t;
        var t3 = t2 * t;

        return 0.5f * (
            (2f * p1) +
            (p2 - p0) * t +
            ((2f * p0) - (5f * p1) + (4f * p2) - p3) * t2 +
            ((3f * p1) - (3f * p2) + p3 - p0) * t3);
    }
}
