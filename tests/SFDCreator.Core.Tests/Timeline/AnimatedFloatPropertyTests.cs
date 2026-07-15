using SFDCreator.Core.Timeline.Automation;
using SFDCreator.Core.Timeline.Interpolation;

namespace SFDCreator.Core.Tests.Timeline;

public sealed class AnimatedFloatPropertyTests
{
    [Fact]
    public void Apply_InvokesSetterWithCurveEvaluatedValue()
    {
        var curve = new AnimationCurve();
        curve.AddKeyframe(new FloatKeyframe(0f, 0f));
        curve.AddKeyframe(new FloatKeyframe(10f, 100f));

        var captured = 0f;
        var property = new AnimatedFloatProperty(curve, value => captured = value);

        property.Apply(5f);

        Assert.Equal(50f, captured, 2);
    }

    [Fact]
    public void Apply_CalledMultipleTimes_UpdatesEachTime()
    {
        var curve = new AnimationCurve();
        curve.AddKeyframe(new FloatKeyframe(0f, 0f));
        curve.AddKeyframe(new FloatKeyframe(10f, 100f));

        var values = new List<float>();
        var property = new AnimatedFloatProperty(curve, values.Add);

        property.Apply(0f);
        property.Apply(10f);

        Assert.Equal(new[] { 0f, 100f }, values);
    }
}
