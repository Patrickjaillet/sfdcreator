namespace SFDCreator.Core.Timeline.Interpolation;

public sealed class AnimationCurve
{
    private readonly List<FloatKeyframe> _keyframes = new();

    public IReadOnlyList<FloatKeyframe> Keyframes => _keyframes;

    public void AddKeyframe(FloatKeyframe keyframe)
    {
        var insertIndex = _keyframes.FindIndex(k => k.Time > keyframe.Time);

        if (insertIndex < 0)
        {
            _keyframes.Add(keyframe);
        }
        else
        {
            _keyframes.Insert(insertIndex, keyframe);
        }
    }

    public void RemoveKeyframe(int index) => _keyframes.RemoveAt(index);

    public void MoveKeyframe(int index, float newTime)
    {
        var keyframe = _keyframes[index] with { Time = newTime };
        _keyframes.RemoveAt(index);
        AddKeyframe(keyframe);
    }

    public float Evaluate(float time)
    {
        if (_keyframes.Count == 0)
        {
            return 0f;
        }

        if (_keyframes.Count == 1 || time <= _keyframes[0].Time)
        {
            return _keyframes[0].Value;
        }

        if (time >= _keyframes[^1].Time)
        {
            return _keyframes[^1].Value;
        }

        var segment = 0;
        for (var i = 0; i < _keyframes.Count - 1; i++)
        {
            if (time >= _keyframes[i].Time && time <= _keyframes[i + 1].Time)
            {
                segment = i;
                break;
            }
        }

        var left = _keyframes[segment];
        var right = _keyframes[segment + 1];
        var span = right.Time - left.Time;
        var t = span > 0f ? (time - left.Time) / span : 0f;

        return left.Mode switch
        {
            InterpolationMode.Stepped => left.Value,
            InterpolationMode.Bezier => Hermite(left.Value, left.OutTangent, right.Value, right.InTangent, t, span),
            _ => left.Value + ((right.Value - left.Value) * t),
        };
    }

    private static float Hermite(float p0, float m0, float p1, float m1, float t, float span)
    {
        var t2 = t * t;
        var t3 = t2 * t;

        var h00 = (2f * t3) - (3f * t2) + 1f;
        var h10 = t3 - (2f * t2) + t;
        var h01 = (-2f * t3) + (3f * t2);
        var h11 = t3 - t2;

        return (h00 * p0) + (h10 * span * m0) + (h01 * p1) + (h11 * span * m1);
    }
}
