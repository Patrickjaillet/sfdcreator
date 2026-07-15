using SFDCreator.Rendering.Diagnostics;

namespace SFDCreator.Rendering.Tests.Diagnostics;

public sealed class FrameStatsTests
{
    [Fact]
    public void Record_ComputesAverageFrameTimeAndFps()
    {
        var stats = new FrameStats(windowSize: 10);

        for (var i = 0; i < 4; i++)
        {
            stats.Record(0.02f);
        }

        Assert.Equal(4, stats.SampleCount);
        Assert.Equal(0.02f, stats.AverageFrameSeconds, 5);
        Assert.Equal(50f, stats.AverageFps, 1);
    }

    [Fact]
    public void Record_DropsOldestSampleBeyondWindowSize()
    {
        var stats = new FrameStats(windowSize: 3);

        stats.Record(0.1f);
        stats.Record(0.1f);
        stats.Record(0.1f);
        stats.Record(0.2f);

        Assert.Equal(3, stats.SampleCount);
        Assert.Equal((0.1f + 0.1f + 0.2f) / 3f, stats.AverageFrameSeconds, 5);
    }

    [Fact]
    public void MinMaxFrameSeconds_TrackExtremesInWindow()
    {
        var stats = new FrameStats(windowSize: 10);

        stats.Record(0.01f);
        stats.Record(0.05f);
        stats.Record(0.02f);

        Assert.Equal(0.01f, stats.MinFrameSeconds, 5);
        Assert.Equal(0.05f, stats.MaxFrameSeconds, 5);
    }

    [Fact]
    public void EmptyStats_ReportsZero()
    {
        var stats = new FrameStats();

        Assert.Equal(0, stats.SampleCount);
        Assert.Equal(0f, stats.AverageFps);
    }
}
