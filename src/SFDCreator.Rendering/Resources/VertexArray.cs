using Silk.NET.OpenGL;

namespace SFDCreator.Rendering.Resources;

public readonly record struct VertexAttribute(uint Index, int ComponentCount, uint OffsetBytes);

public sealed class VertexArray : IDisposable
{
    private readonly GL _gl;

    public uint Handle { get; }

    public VertexArray(GL gl, GpuBuffer vertexBuffer, GpuBuffer? indexBuffer, uint strideBytes, IReadOnlyList<VertexAttribute> attributes)
    {
        _gl = gl;
        Handle = _gl.GenVertexArrays(1);
        Bind();

        vertexBuffer.Bind();
        indexBuffer?.Bind();

        foreach (var attribute in attributes)
        {
            _gl.VertexAttribPointer(attribute.Index, attribute.ComponentCount, VertexAttribPointerType.Float, false, strideBytes, (nint)attribute.OffsetBytes);
            _gl.EnableVertexAttribArray(attribute.Index);
        }

        _gl.BindVertexArray(0);
    }

    public void Bind() => _gl.BindVertexArray(Handle);

    public void Dispose() => _gl.DeleteVertexArray(Handle);
}
