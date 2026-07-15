namespace SFDCreator.Core.Timeline.Playback;

public sealed class TimelinePlaybackController
{
    public bool IsPlaying { get; private set; }

    public float CurrentTime { get; private set; }

    public float PlaybackRate { get; set; } = 1f;

    public void Play() => IsPlaying = true;

    public void Pause() => IsPlaying = false;

    public void Seek(float time) => CurrentTime = Math.Max(time, 0f);

    public void Tick(float deltaSeconds)
    {
        if (!IsPlaying)
        {
            return;
        }

        CurrentTime = Math.Max(CurrentTime + (deltaSeconds * PlaybackRate), 0f);
    }
}
