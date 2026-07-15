namespace SFDCreator.Core.Timeline.Sequencing;

public sealed record TimelineSegment(string Name, float Start, float Duration, TransitionSpec TransitionIn);
