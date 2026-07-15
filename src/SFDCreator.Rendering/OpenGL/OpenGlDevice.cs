using SFDCreator.Rendering.Backend;
using SFDCreator.Rendering.OpenGL.Interop;
using Silk.NET.OpenGL;

namespace SFDCreator.Rendering.OpenGL;

public sealed class OpenGlDevice : IGraphicsDevice
{
    private readonly nint _hwnd;
    private readonly nint _dc;
    private readonly nint _context;
    private bool _disposed;

    internal OpenGlDevice(GL gl, nint hwnd, nint dc, nint context)
    {
        Api = gl;
        _hwnd = hwnd;
        _dc = dc;
        _context = context;
    }

    public GL Api { get; }

    public GraphicsBackendKind Backend => GraphicsBackendKind.OpenGL;

    public void MakeCurrent() => Opengl32.wglMakeCurrent(_dc, _context);

    public void SwapBuffers() => Gdi32.SwapBuffers(_dc);

    public void Dispose()
    {
        if (_disposed)
        {
            return;
        }

        Opengl32.wglMakeCurrent(0, 0);
        Opengl32.wglDeleteContext(_context);
        User32Minimal.ReleaseDC(_hwnd, _dc);
        _disposed = true;
    }
}
