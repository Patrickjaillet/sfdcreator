namespace SFDCreator.Rendering.Diagnostics;

public sealed class FrameStats
{
    private readonly Queue<float> _frameTimes = new();
    private readonly int _windowSize;
    private float _sum;

    public FrameStats(int windowSize = 120)
    {
        _windowSize = windowSize;
    }

    public int SampleCount => _frameTimes.Count;

    public IReadOnlyList<float> Samples => _frameTimes.ToArray();

    public void Record(float deltaSeconds)
    {
        _frameTimes.Enqueue(deltaSeconds);
        _sum += deltaSeconds;

        while (_frameTimes.Count > _windowSize)
        {
            _sum -= _frameTimes.Dequeue();
        }
    }

    public float AverageFrameSeconds => SampleCount == 0 ? 0f : _sum / SampleCount;

    public float AverageFps => AverageFrameSeconds > 0f ? 1f / AverageFrameSeconds : 0f;

    public float MinFrameSeconds => SampleCount == 0 ? 0f : _frameTimes.Min();

    public float MaxFrameSeconds => SampleCount == 0 ? 0f : _frameTimes.Max();

    public float MaxFps => MinFrameSeconds > 0f ? 1f / MinFrameSeconds : 0f;

    public float MinFps => MaxFrameSeconds > 0f ? 1f / MaxFrameSeconds : 0f;
}
