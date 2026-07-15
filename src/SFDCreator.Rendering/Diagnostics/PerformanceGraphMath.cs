namespace SFDCreator.Rendering.Diagnostics;

public enum PerformanceBand
{
    Good,
    Warning,
    Bad,
}

public readonly record struct PerformanceSample(float NormalizedHeight, PerformanceBand Band);

public static class PerformanceGraphMath
{
    public static PerformanceSample NormalizeFrameTime(float frameSeconds, float targetFps)
    {
        var targetFrameSeconds = targetFps > 0f ? 1f / targetFps : 1f / 60f;
        var ratio = frameSeconds / targetFrameSeconds;
        var height = Math.Clamp(ratio, 0f, 2f) / 2f;

        var band = ratio switch
        {
            <= 1.1f => PerformanceBand.Good,
            <= 1.5f => PerformanceBand.Warning,
            _ => PerformanceBand.Bad,
        };

        return new PerformanceSample(height, band);
    }
}
