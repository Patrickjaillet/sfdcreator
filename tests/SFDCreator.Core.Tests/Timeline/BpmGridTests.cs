using SFDCreator.Core.Timeline.Sync;

namespace SFDCreator.Core.Tests.Timeline;

public sealed class BpmGridTests
{
    [Fact]
    public void BeatDurationSeconds_At120Bpm_IsHalfASecond()
    {
        var grid = new BpmGrid(120f);

        Assert.Equal(0.5f, grid.BeatDurationSeconds, 3);
    }

    [Fact]
    public void BeatToTime_ReturnsBeatIndexTimesBeatDuration()
    {
        var grid = new BpmGrid(120f);

        Assert.Equal(2f, grid.BeatToTime(4), 3);
    }

    [Fact]
    public void TimeToNearestBeat_SnapsToClosestBeat()
    {
        var grid = new BpmGrid(120f);

        Assert.Equal(1.0f, grid.TimeToNearestBeat(1.1f), 3);
        Assert.Equal(1.5f, grid.TimeToNearestBeat(1.4f), 3);
    }

    [Fact]
    public void SnapToGrid_IsEquivalentToTimeToNearestBeat()
    {
        var grid = new BpmGrid(140f);

        Assert.Equal(grid.TimeToNearestBeat(3.3f), grid.SnapToGrid(3.3f), 3);
    }

    [Fact]
    public void TimeToNearestBeat_WithZeroBpm_ReturnsInputUnchanged()
    {
        var grid = new BpmGrid(0f);

        Assert.Equal(5f, grid.TimeToNearestBeat(5f), 3);
    }
}
