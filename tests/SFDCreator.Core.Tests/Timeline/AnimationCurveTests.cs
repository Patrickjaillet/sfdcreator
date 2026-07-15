using SFDCreator.Core.Timeline.Interpolation;

namespace SFDCreator.Core.Tests.Timeline;

public sealed class AnimationCurveTests
{
    [Fact]
    public void Evaluate_WithNoKeyframes_ReturnsZero()
    {
        var curve = new AnimationCurve();

        Assert.Equal(0f, curve.Evaluate(5f));
    }

    [Fact]
    public void Evaluate_BeforeFirstKeyframe_ClampsToFirstValue()
    {
        var curve = new AnimationCurve();
        curve.AddKeyframe(new FloatKeyframe(2f, 10f));
        curve.AddKeyframe(new FloatKeyframe(4f, 20f));

        Assert.Equal(10f, curve.Evaluate(-5f));
    }

    [Fact]
    public void Evaluate_AfterLastKeyframe_ClampsToLastValue()
    {
        var curve = new AnimationCurve();
        curve.AddKeyframe(new FloatKeyframe(2f, 10f));
        curve.AddKeyframe(new FloatKeyframe(4f, 20f));

        Assert.Equal(20f, curve.Evaluate(100f));
    }

    [Fact]
    public void Evaluate_Linear_InterpolatesProportionally()
    {
        var curve = new AnimationCurve();
        curve.AddKeyframe(new FloatKeyframe(0f, 0f, InterpolationMode.Linear));
        curve.AddKeyframe(new FloatKeyframe(10f, 100f));

        Assert.Equal(50f, curve.Evaluate(5f), 2);
    }

    [Fact]
    public void Evaluate_Stepped_HoldsLeftKeyframeValue()
    {
        var curve = new AnimationCurve();
        curve.AddKeyframe(new FloatKeyframe(0f, 1f, InterpolationMode.Stepped));
        curve.AddKeyframe(new FloatKeyframe(10f, 99f));

        Assert.Equal(1f, curve.Evaluate(9.9f), 2);
        Assert.Equal(99f, curve.Evaluate(10f), 2);
    }

    [Fact]
    public void Evaluate_Bezier_PassesThroughBothEndpoints()
    {
        var curve = new AnimationCurve();
        curve.AddKeyframe(new FloatKeyframe(0f, 0f, InterpolationMode.Bezier, OutTangent: 1f));
        curve.AddKeyframe(new FloatKeyframe(4f, 10f, InterpolationMode.Bezier, InTangent: 1f));

        Assert.Equal(0f, curve.Evaluate(0f), 2);
        Assert.Equal(10f, curve.Evaluate(4f), 2);
    }

    [Fact]
    public void Evaluate_Bezier_IsMonotonicForSimpleRisingCurve()
    {
        var curve = new AnimationCurve();
        curve.AddKeyframe(new FloatKeyframe(0f, 0f, InterpolationMode.Bezier, OutTangent: 0.5f));
        curve.AddKeyframe(new FloatKeyframe(10f, 100f, InterpolationMode.Bezier, InTangent: 0.5f));

        var a = curve.Evaluate(2f);
        var b = curve.Evaluate(5f);
        var c = curve.Evaluate(8f);

        Assert.True(a < b);
        Assert.True(b < c);
    }

    [Fact]
    public void AddKeyframe_KeepsKeyframesSortedByTime()
    {
        var curve = new AnimationCurve();
        curve.AddKeyframe(new FloatKeyframe(5f, 1f));
        curve.AddKeyframe(new FloatKeyframe(1f, 2f));
        curve.AddKeyframe(new FloatKeyframe(3f, 3f));

        Assert.Equal(new[] { 1f, 3f, 5f }, curve.Keyframes.Select(k => k.Time));
    }

    [Fact]
    public void MoveKeyframe_ResortsWhenMovedPastANeighbor()
    {
        var curve = new AnimationCurve();
        curve.AddKeyframe(new FloatKeyframe(0f, 10f));
        curve.AddKeyframe(new FloatKeyframe(5f, 20f));
        curve.AddKeyframe(new FloatKeyframe(10f, 30f));

        curve.MoveKeyframe(0, 12f);

        Assert.Equal(new[] { 5f, 10f, 12f }, curve.Keyframes.Select(k => k.Time));
        Assert.Equal(new[] { 20f, 30f, 10f }, curve.Keyframes.Select(k => k.Value));
    }

    [Fact]
    public void RemoveKeyframe_RemovesAtIndex()
    {
        var curve = new AnimationCurve();
        curve.AddKeyframe(new FloatKeyframe(0f, 1f));
        curve.AddKeyframe(new FloatKeyframe(5f, 2f));

        curve.RemoveKeyframe(0);

        Assert.Single(curve.Keyframes);
        Assert.Equal(5f, curve.Keyframes[0].Time);
    }
}
