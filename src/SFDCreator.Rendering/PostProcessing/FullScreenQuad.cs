using Silk.NET.OpenGL;
using SFDCreator.Rendering.Resources;

namespace SFDCreator.Rendering.PostProcessing;

public sealed class FullScreenQuad : IDisposable
{
    private readonly GL _gl;
    private readonly GpuBuffer _vertexBuffer;
    private readonly Resources.VertexArray _vertexArray;

    public FullScreenQuad(GL gl)
    {
        _gl = gl;

        ReadOnlySpan<float> vertices = stackalloc float[]
        {
            -1f, -1f, 0f, 0f,
             1f, -1f, 1f, 0f,
             1f, 1f, 1f, 1f,
            -1f, -1f, 0f, 0f,
             1f, 1f, 1f, 1f,
            -1f, 1f, 0f, 1f,
        };

        _vertexBuffer = new GpuBuffer(gl, BufferTargetARB.ArrayBuffer);
        _vertexBuffer.Upload(vertices);

        _vertexArray = new Resources.VertexArray(gl, _vertexBuffer, null, 4 * sizeof(float), new[]
        {
            new VertexAttribute(0, 2, 0),
            new VertexAttribute(1, 2, 2 * sizeof(float)),
        });
    }

    public void Draw()
    {
        _vertexArray.Bind();
        _gl.DrawArrays(PrimitiveType.Triangles, 0, 6);
    }

    public void Dispose()
    {
        _vertexArray.Dispose();
        _vertexBuffer.Dispose();
    }
}
