namespace SFDCreator.Core.Timeline.Interpolation;

public readonly record struct FloatKeyframe(
    float Time,
    float Value,
    InterpolationMode Mode = InterpolationMode.Linear,
    float InTangent = 0f,
    float OutTangent = 0f);
