namespace SFDCreator.Core.Timeline.Sequencing;

public readonly record struct SegmentBlend(TimelineSegment Current, TimelineSegment? Next, float BlendWeight);

public sealed class MasterTimeline
{
    private readonly List<TimelineSegment> _segments = new();

    public IReadOnlyList<TimelineSegment> Segments => _segments;

    public float TotalDuration => _segments.Count == 0 ? 0f : _segments.Max(s => s.Start + s.Duration);

    public void AddSegment(TimelineSegment segment) => _segments.Add(segment);

    public SegmentBlend Evaluate(float time)
    {
        if (_segments.Count == 0)
        {
            throw new InvalidOperationException("The master timeline has no segments.");
        }

        var total = TotalDuration;
        var wrapped = total > 0f ? Mod(time, total) : 0f;

        var currentIndex = _segments.Count - 1;
        for (var i = 0; i < _segments.Count; i++)
        {
            if (wrapped >= _segments[i].Start && wrapped < _segments[i].Start + _segments[i].Duration)
            {
                currentIndex = i;
                break;
            }
        }

        var current = _segments[currentIndex];
        var nextIndex = (currentIndex + 1) % _segments.Count;
        var next = _segments[nextIndex];

        if (next.TransitionIn.Kind != TransitionKind.Crossfade || next.TransitionIn.DurationSeconds <= 0f)
        {
            return new SegmentBlend(current, null, 0f);
        }

        var currentEnd = current.Start + current.Duration;
        var windowStart = currentEnd - next.TransitionIn.DurationSeconds;

        if (wrapped >= windowStart && wrapped < currentEnd)
        {
            var blend = (wrapped - windowStart) / next.TransitionIn.DurationSeconds;
            return new SegmentBlend(current, next, blend);
        }

        return new SegmentBlend(current, null, 0f);
    }

    private static float Mod(float value, float modulus)
    {
        var result = value % modulus;
        return result < 0f ? result + modulus : result;
    }
}
