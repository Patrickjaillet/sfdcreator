namespace SFDCreator.Core.Timeline.Sequencing;

public sealed record TransitionSpec(TransitionKind Kind, float DurationSeconds)
{
    public static TransitionSpec Cut { get; } = new(TransitionKind.Cut, 0f);
}
