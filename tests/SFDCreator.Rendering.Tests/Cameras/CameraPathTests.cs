using System.Numerics;
using SFDCreator.Rendering.Cameras;

namespace SFDCreator.Rendering.Tests.Cameras;

public sealed class CameraPathTests
{
    private static readonly CameraPathKeyframe[] Keyframes =
    {
        new(0f, new Vector3(0f, 0f, 0f), Vector3.Zero),
        new(1f, new Vector3(10f, 0f, 0f), Vector3.Zero),
        new(2f, new Vector3(20f, 0f, 0f), Vector3.Zero),
    };

    [Fact]
    public void Evaluate_AtKeyframeTime_ReturnsExactKeyframePosition()
    {
        var (position, _) = CameraPath.Evaluate(Keyframes, 1f);

        Assert.Equal(new Vector3(10f, 0f, 0f), position);
    }

    [Fact]
    public void Evaluate_BeforeFirstKeyframe_ClampsToFirst()
    {
        var (position, _) = CameraPath.Evaluate(Keyframes, -5f);

        Assert.Equal(Keyframes[0].Position, position);
    }

    [Fact]
    public void Evaluate_AfterLastKeyframe_ClampsToLast()
    {
        var (position, _) = CameraPath.Evaluate(Keyframes, 50f);

        Assert.Equal(Keyframes[^1].Position, position);
    }

    [Fact]
    public void Evaluate_Midway_InterpolatesBetweenKeyframes()
    {
        var (position, _) = CameraPath.Evaluate(Keyframes, 0.5f);

        Assert.True(position.X > 0f && position.X < 10f);
    }

    [Fact]
    public void Evaluate_SingleKeyframe_AlwaysReturnsThatKeyframe()
    {
        var single = new[] { new CameraPathKeyframe(0f, new Vector3(1f, 2f, 3f), Vector3.One) };

        var (position, lookAt) = CameraPath.Evaluate(single, 100f);

        Assert.Equal(new Vector3(1f, 2f, 3f), position);
        Assert.Equal(Vector3.One, lookAt);
    }
}
