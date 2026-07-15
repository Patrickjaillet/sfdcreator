using Silk.NET.Input;
using Silk.NET.OpenGL;
using Silk.NET.Windowing;

namespace SFDCreator.Rendering.Windowing;

public sealed class RenderWindow : IDisposable
{
    private readonly IWindow _window;
    private GL? _gl;
    private IInputContext? _input;

    public RenderWindow(RenderWindowOptions options)
    {
        var windowOptions = WindowOptions.Default with
        {
            Title = options.Title,
            Size = new Silk.NET.Maths.Vector2D<int>(options.Width, options.Height),
            VSync = options.VSync,
        };

        _window = Window.Create(windowOptions);
        _window.Load += OnLoad;
        _window.Render += OnRender;
        _window.Resize += OnResize;
        _window.Closing += OnClosing;
    }

    public event Action<GL>? GLReady;

    public event Action<double, GL>? RenderFrame;

    public GL GL => _gl ?? throw new InvalidOperationException("The OpenGL context is not ready yet. Wait for GLReady.");

    public IInputContext Input => _input ?? throw new InvalidOperationException("The input context is not ready yet. Wait for GLReady.");

    public void Run() => _window.Run();

    public void Close() => _window.Close();

    private void OnLoad()
    {
        _gl = _window.CreateOpenGL();
        _input = _window.CreateInput();
        GLReady?.Invoke(_gl);
    }

    private void OnRender(double deltaSeconds)
    {
        if (_gl is not null)
        {
            RenderFrame?.Invoke(deltaSeconds, _gl);
        }
    }

    private void OnResize(Silk.NET.Maths.Vector2D<int> size)
    {
        _gl?.Viewport(0, 0, (uint)size.X, (uint)size.Y);
    }

    private void OnClosing()
    {
        _input?.Dispose();
        _gl?.Dispose();
    }

    public void Dispose()
    {
        _window.Dispose();
    }
}
