using SFDCreator.UI.Panels.Timeline;

namespace SFDCreator.UI.Tests.Panels.Timeline;

public sealed class TimelineModelTests
{
    [Fact]
    public void TimeToPixel_AndBack_RoundTrips()
    {
        var model = new TimelineModel { DurationSeconds = 10f };

        var pixel = model.TimeToPixel(5f, rulerLeft: 20f, rulerWidth: 200f);
        var time = model.PixelToTime(pixel, rulerLeft: 20f, rulerWidth: 200f);

        Assert.Equal(5f, time, 2);
    }

    [Fact]
    public void PixelToTime_ClampsToDuration()
    {
        var model = new TimelineModel { DurationSeconds = 10f };

        var time = model.PixelToTime(pixel: 10000f, rulerLeft: 0f, rulerWidth: 100f);

        Assert.Equal(10f, time, 2);
    }

    [Fact]
    public void PixelToTime_ClampsToZero()
    {
        var model = new TimelineModel { DurationSeconds = 10f };

        var time = model.PixelToTime(pixel: -500f, rulerLeft: 0f, rulerWidth: 100f);

        Assert.Equal(0f, time, 2);
    }

    [Fact]
    public void HitTestKeyframe_FindsKeyframeWithinRadius()
    {
        var model = new TimelineModel { DurationSeconds = 10f };
        model.AddTrack("Track");
        model.AddKeyframe(trackIndex: 0, time: 5f);

        var kx = model.TimeToPixel(5f, rulerLeft: 0f, rulerWidth: 100f);
        var ky = 13f;

        var hit = model.HitTestKeyframe(kx, ky, rulerLeft: 0f, rulerWidth: 100f, trackTop: 0f, trackHeight: 26f);

        Assert.Equal(0, hit);
    }

    [Fact]
    public void HitTestKeyframe_ReturnsNullWhenFarFromAnyKeyframe()
    {
        var model = new TimelineModel { DurationSeconds = 10f };
        model.AddTrack("Track");
        model.AddKeyframe(trackIndex: 0, time: 5f);

        var hit = model.HitTestKeyframe(9999f, 9999f, rulerLeft: 0f, rulerWidth: 100f, trackTop: 0f, trackHeight: 26f);

        Assert.Null(hit);
    }

    [Fact]
    public void MoveKeyframe_ClampsToDurationBounds()
    {
        var model = new TimelineModel { DurationSeconds = 10f };
        model.AddTrack("Track");
        model.AddKeyframe(trackIndex: 0, time: 5f);

        model.MoveKeyframe(0, 999f);

        Assert.Equal(10f, model.Keyframes[0].Time, 2);
    }

    [Fact]
    public void HitTestPlayhead_TrueWithinTolerance()
    {
        var model = new TimelineModel { DurationSeconds = 10f, PlayheadSeconds = 5f };

        var playheadPx = model.TimeToPixel(5f, rulerLeft: 0f, rulerWidth: 100f);

        Assert.True(model.HitTestPlayhead(playheadPx + 2f, rulerLeft: 0f, rulerWidth: 100f));
        Assert.False(model.HitTestPlayhead(playheadPx + 50f, rulerLeft: 0f, rulerWidth: 100f));
    }
}
