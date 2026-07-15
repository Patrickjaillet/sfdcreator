namespace SFDCreator.Core.Timeline.Sync;

public sealed class BpmGrid
{
    public BpmGrid(float bpm, int beatsPerBar = 4)
    {
        Bpm = bpm;
        BeatsPerBar = beatsPerBar;
    }

    public float Bpm { get; }

    public int BeatsPerBar { get; }

    public float BeatDurationSeconds => Bpm > 0f ? 60f / Bpm : 0f;

    public float BeatToTime(int beatIndex) => beatIndex * BeatDurationSeconds;

    public float TimeToNearestBeat(float time)
    {
        var beatDuration = BeatDurationSeconds;
        if (beatDuration <= 0f)
        {
            return time;
        }

        var nearestBeatIndex = (int)MathF.Round(time / beatDuration);
        return nearestBeatIndex * beatDuration;
    }

    public float SnapToGrid(float time) => TimeToNearestBeat(time);
}
