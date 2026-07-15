using Silk.NET.OpenGL;

namespace SFDCreator.Rendering.Resources;

public sealed class GpuBuffer : IDisposable
{
    private readonly GL _gl;
    private readonly BufferTargetARB _target;

    public uint Handle { get; }

    public GpuBuffer(GL gl, BufferTargetARB target)
    {
        _gl = gl;
        _target = target;
        Handle = _gl.GenBuffers(1);
    }

    public void Bind() => _gl.BindBuffer(_target, Handle);

    public void Upload<T>(ReadOnlySpan<T> data, BufferUsageARB usage = BufferUsageARB.StaticDraw) where T : unmanaged
    {
        Bind();
        _gl.BufferData(_target, data, usage);
    }

    public void Dispose() => _gl.DeleteBuffer(Handle);
}
