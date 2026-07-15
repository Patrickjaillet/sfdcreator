using SFDCreator.Core.Timeline.Interpolation;

namespace SFDCreator.Core.Timeline.UndoRedo;

public sealed class MoveKeyframeCommand : ICommand
{
    private const float TimeTolerance = 0.0001f;

    private readonly AnimationCurve _curve;
    private readonly float _oldTime;
    private readonly float _newTime;

    public MoveKeyframeCommand(AnimationCurve curve, float oldTime, float newTime)
    {
        _curve = curve;
        _oldTime = oldTime;
        _newTime = newTime;
    }

    public void Execute()
    {
        var index = FindIndex(_oldTime);
        if (index >= 0)
        {
            _curve.MoveKeyframe(index, _newTime);
        }
    }

    public void Undo()
    {
        var index = FindIndex(_newTime);
        if (index >= 0)
        {
            _curve.MoveKeyframe(index, _oldTime);
        }
    }

    private int FindIndex(float time)
    {
        for (var i = 0; i < _curve.Keyframes.Count; i++)
        {
            if (MathF.Abs(_curve.Keyframes[i].Time - time) < TimeTolerance)
            {
                return i;
            }
        }

        return -1;
    }
}
