namespace SFDCreator.UI.Panels.Timeline;

public readonly record struct TimelineKeyframe(int TrackIndex, float Time);

public sealed class TimelineModel
{
    private readonly List<string> _tracks = new();
    private readonly List<TimelineKeyframe> _keyframes = new();

    public float DurationSeconds { get; set; } = 12f;

    public float PlayheadSeconds { get; set; }

    public IReadOnlyList<string> Tracks => _tracks;

    public IReadOnlyList<TimelineKeyframe> Keyframes => _keyframes;

    public void AddTrack(string name) => _tracks.Add(name);

    public void AddKeyframe(int trackIndex, float time) => _keyframes.Add(new TimelineKeyframe(trackIndex, time));

    public void MoveKeyframe(int index, float newTime)
    {
        var existing = _keyframes[index];
        _keyframes[index] = existing with { Time = Math.Clamp(newTime, 0f, DurationSeconds) };
    }

    public float TimeToPixel(float time, float rulerLeft, float rulerWidth) =>
        rulerLeft + (DurationSeconds > 0f ? (time / DurationSeconds) * rulerWidth : 0f);

    public float PixelToTime(float pixel, float rulerLeft, float rulerWidth) =>
        rulerWidth > 0f ? Math.Clamp((pixel - rulerLeft) / rulerWidth * DurationSeconds, 0f, DurationSeconds) : 0f;

    public int? HitTestKeyframe(float pixelX, float pixelY, float rulerLeft, float rulerWidth, float trackTop, float trackHeight, float radius = 7f)
    {
        for (var i = 0; i < _keyframes.Count; i++)
        {
            var keyframe = _keyframes[i];
            var kx = TimeToPixel(keyframe.Time, rulerLeft, rulerWidth);
            var ky = trackTop + (keyframe.TrackIndex * trackHeight) + (trackHeight / 2f);

            if (Math.Abs(pixelX - kx) <= radius && Math.Abs(pixelY - ky) <= radius)
            {
                return i;
            }
        }

        return null;
    }

    public bool HitTestPlayhead(float pixelX, float rulerLeft, float rulerWidth, float tolerance = 6f)
    {
        var playheadPx = TimeToPixel(PlayheadSeconds, rulerLeft, rulerWidth);
        return Math.Abs(pixelX - playheadPx) <= tolerance;
    }
}
