using System.Diagnostics;

namespace SFDCreator.Rendering.Diagnostics;

public sealed class FrameClock
{
    private readonly Stopwatch _stopwatch = Stopwatch.StartNew();
    private long _lastTicks;

    public float TotalSeconds { get; private set; }

    public float Tick()
    {
        var currentTicks = _stopwatch.ElapsedTicks;
        var deltaSeconds = (float)((currentTicks - _lastTicks) / (double)Stopwatch.Frequency);
        _lastTicks = currentTicks;
        TotalSeconds += deltaSeconds;
        return deltaSeconds;
    }
}
