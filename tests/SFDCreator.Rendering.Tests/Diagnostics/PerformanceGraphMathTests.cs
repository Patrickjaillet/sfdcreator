using SFDCreator.Rendering.Diagnostics;

namespace SFDCreator.Rendering.Tests.Diagnostics;

public sealed class PerformanceGraphMathTests
{
    [Fact]
    public void NormalizeFrameTime_AtTargetFrameTime_IsGoodBand()
    {
        var sample = PerformanceGraphMath.NormalizeFrameTime(frameSeconds: 1f / 60f, targetFps: 60f);

        Assert.Equal(PerformanceBand.Good, sample.Band);
        Assert.Equal(0.5f, sample.NormalizedHeight, 2);
    }

    [Fact]
    public void NormalizeFrameTime_SlightlyOverTarget_IsStillGoodBand()
    {
        var sample = PerformanceGraphMath.NormalizeFrameTime(frameSeconds: 1.05f / 60f, targetFps: 60f);

        Assert.Equal(PerformanceBand.Good, sample.Band);
    }

    [Fact]
    public void NormalizeFrameTime_ModeratelyOverTarget_IsWarningBand()
    {
        var sample = PerformanceGraphMath.NormalizeFrameTime(frameSeconds: 1.3f / 60f, targetFps: 60f);

        Assert.Equal(PerformanceBand.Warning, sample.Band);
    }

    [Fact]
    public void NormalizeFrameTime_FarOverTarget_IsBadBand()
    {
        var sample = PerformanceGraphMath.NormalizeFrameTime(frameSeconds: 3f / 60f, targetFps: 60f);

        Assert.Equal(PerformanceBand.Bad, sample.Band);
    }

    [Fact]
    public void NormalizeFrameTime_ClampsNormalizedHeightToOne()
    {
        var sample = PerformanceGraphMath.NormalizeFrameTime(frameSeconds: 100f, targetFps: 60f);

        Assert.Equal(1f, sample.NormalizedHeight, 3);
    }

    [Fact]
    public void NormalizeFrameTime_VeryFastFrame_HasSmallNormalizedHeight()
    {
        var sample = PerformanceGraphMath.NormalizeFrameTime(frameSeconds: 0f, targetFps: 60f);

        Assert.Equal(0f, sample.NormalizedHeight, 3);
        Assert.Equal(PerformanceBand.Good, sample.Band);
    }
}
