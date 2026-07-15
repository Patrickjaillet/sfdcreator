using SkiaSharp;

namespace SFDCreator.UI.Rendering;

public sealed class SkiaSurfaceHost : IDisposable
{
    private SKSurface? _surface;
    private SKImageInfo _info;

    public SKImageInfo Info => _info;

    public void Resize(int width, int height)
    {
        if (width <= 0 || height <= 0)
        {
            return;
        }

        _info = new SKImageInfo(width, height, SKColorType.Bgra8888, SKAlphaType.Premul);
        _surface?.Dispose();
        _surface = SKSurface.Create(_info);
    }

    public SKCanvas BeginFrame(SKColor clearColor)
    {
        if (_surface is null)
        {
            throw new InvalidOperationException("Call Resize before BeginFrame.");
        }

        var canvas = _surface.Canvas;
        canvas.Clear(clearColor);
        return canvas;
    }

    public SKImage EndFrame()
    {
        if (_surface is null)
        {
            throw new InvalidOperationException("Call Resize before EndFrame.");
        }

        return _surface.Snapshot();
    }

    public void Dispose()
    {
        _surface?.Dispose();
    }
}
