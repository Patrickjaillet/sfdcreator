using SFDCreator.Core.Timeline.Playback;

namespace SFDCreator.Core.Tests.Timeline;

public sealed class TimelinePlaybackControllerTests
{
    [Fact]
    public void Tick_WhilePlaying_AdvancesCurrentTime()
    {
        var controller = new TimelinePlaybackController();
        controller.Play();

        controller.Tick(0.5f);

        Assert.Equal(0.5f, controller.CurrentTime, 3);
    }

    [Fact]
    public void Tick_WhilePaused_DoesNotAdvance()
    {
        var controller = new TimelinePlaybackController();

        controller.Tick(1f);

        Assert.Equal(0f, controller.CurrentTime, 3);
        Assert.False(controller.IsPlaying);
    }

    [Fact]
    public void Seek_JumpsImmediatelyRegardlessOfPlayState()
    {
        var controller = new TimelinePlaybackController();

        controller.Seek(7f);

        Assert.Equal(7f, controller.CurrentTime, 3);
    }

    [Fact]
    public void Seek_DoesNotAllowNegativeTime()
    {
        var controller = new TimelinePlaybackController();

        controller.Seek(-5f);

        Assert.Equal(0f, controller.CurrentTime, 3);
    }

    [Fact]
    public void PlaybackRate_ScalesTickAdvancement()
    {
        var controller = new TimelinePlaybackController { PlaybackRate = 2f };
        controller.Play();

        controller.Tick(1f);

        Assert.Equal(2f, controller.CurrentTime, 3);
    }

    [Fact]
    public void PauseThenTick_HoldsCurrentTime()
    {
        var controller = new TimelinePlaybackController();
        controller.Play();
        controller.Tick(1f);
        controller.Pause();
        controller.Tick(1f);

        Assert.Equal(1f, controller.CurrentTime, 3);
    }
}
