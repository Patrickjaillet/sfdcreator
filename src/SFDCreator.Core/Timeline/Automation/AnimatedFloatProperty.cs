using SFDCreator.Core.Timeline.Interpolation;

namespace SFDCreator.Core.Timeline.Automation;

public sealed class AnimatedFloatProperty
{
    private readonly AnimationCurve _curve;
    private readonly Action<float> _setter;

    public AnimatedFloatProperty(AnimationCurve curve, Action<float> setter)
    {
        _curve = curve;
        _setter = setter;
    }

    public void Apply(float time) => _setter(_curve.Evaluate(time));
}
