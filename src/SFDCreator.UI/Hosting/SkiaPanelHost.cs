using System.Runtime.InteropServices;
using SFDCreator.UI.Hosting.Interop;
using SFDCreator.UI.Rendering;
using SFDCreator.Win32.Docking;
using SkiaSharp;
using Silk.NET.Input;

namespace SFDCreator.UI.Hosting;

public sealed class SkiaPanelHost : IDisposable
{
    private readonly Win32ChildPanel _panel;
    private readonly IUiPanelContent _content;
    private readonly SkiaSurfaceHost _surfaceHost = new();
    private readonly UiInputState _input = new();

    public SkiaPanelHost(Win32ChildPanel panel, IUiPanelContent content)
    {
        _panel = panel;
        _content = content;

        panel.Paint += OnPaint;
        panel.Resized += OnResized;
        panel.MouseMove += OnMouseMove;
        panel.MouseDown += OnMouseDown;
        panel.MouseUp += OnMouseUp;
        panel.MouseWheel += OnMouseWheel;
        panel.KeyDown += key => _input.PressedKeys.Add(key);
        panel.KeyUp += key => _input.PressedKeys.Remove(key);

        var (width, height) = panel.ClientSize;
        OnResized(width, height);
    }

    private void OnResized(int width, int height)
    {
        _surfaceHost.Resize(Math.Max(width, 1), Math.Max(height, 1));
        _panel.Invalidate();
    }

    private void OnMouseMove(int x, int y)
    {
        _input.MouseX = x;
        _input.MouseY = y;
        _panel.Invalidate();
    }

    private void OnMouseDown(MouseButton button, int x, int y)
    {
        _input.MouseX = x;
        _input.MouseY = y;

        if (button == MouseButton.Left)
        {
            _input.IsLeftDown = true;
            _input.WasLeftPressedThisFrame = true;
        }
        else if (button == MouseButton.Right)
        {
            _input.IsRightDown = true;
        }

        _panel.Invalidate();
    }

    private void OnMouseUp(MouseButton button, int x, int y)
    {
        _input.MouseX = x;
        _input.MouseY = y;

        if (button == MouseButton.Left)
        {
            _input.IsLeftDown = false;
            _input.WasLeftReleasedThisFrame = true;
        }
        else if (button == MouseButton.Right)
        {
            _input.IsRightDown = false;
        }

        _panel.Invalidate();
    }

    private void OnMouseWheel(float delta, int x, int y)
    {
        _input.MouseX = x;
        _input.MouseY = y;
        _input.WheelDelta = delta;
        _panel.Invalidate();
    }

    private void OnPaint(nint hdc)
    {
        var info = _surfaceHost.Info;
        if (info.Width <= 0 || info.Height <= 0)
        {
            return;
        }

        var canvas = _surfaceHost.BeginFrame(SKColors.Black);
        _content.Render(canvas, new SKRect(0, 0, info.Width, info.Height), _input);
        _input.ResetPerFrameFlags();

        using var image = _surfaceHost.EndFrame();
        using var bitmap = SKBitmap.FromImage(image);
        Blit(hdc, bitmap);
    }

    private static void Blit(nint hdc, SKBitmap bitmap)
    {
        var header = new BITMAPINFOHEADER
        {
            biSize = (uint)Marshal.SizeOf<BITMAPINFOHEADER>(),
            biWidth = bitmap.Width,
            biHeight = -bitmap.Height,
            biPlanes = 1,
            biBitCount = 32,
            biCompression = Gdi32.BI_RGB,
        };

        Gdi32.SetDIBitsToDevice(
            hdc,
            0,
            0,
            (uint)bitmap.Width,
            (uint)bitmap.Height,
            0,
            0,
            0,
            (uint)bitmap.Height,
            bitmap.GetPixels(),
            ref header,
            Gdi32.DIB_RGB_COLORS);
    }

    public void Dispose() => _surfaceHost.Dispose();
}
