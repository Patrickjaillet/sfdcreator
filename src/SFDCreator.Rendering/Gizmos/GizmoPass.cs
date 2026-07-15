using System.Numerics;
using Silk.NET.OpenGL;
using SFDCreator.Rendering.Resources;

namespace SFDCreator.Rendering.Gizmos;

public sealed class GizmoPass : IDisposable
{
    private readonly ShaderProgram _shader;
    private readonly GpuBuffer _vertexBuffer;
    private readonly Resources.VertexArray _vao;
    private readonly int _vertexCount;

    public GizmoPass(GL gl, string shaderDirectory)
    {
        _shader = ShaderLoader.Load(gl, shaderDirectory, "scene.vert", "scene.frag");

        var vertices = GizmoGeometry.BuildAxisLines();
        _vertexCount = vertices.Length;

        _vertexBuffer = new GpuBuffer(gl, BufferTargetARB.ArrayBuffer);
        _vertexBuffer.Upload<GizmoGeometry.Vertex>(vertices);

        _vao = new Resources.VertexArray(gl, _vertexBuffer, null, (uint)(sizeof(float) * 6), new[]
        {
            new VertexAttribute(0, 3, 0),
            new VertexAttribute(1, 3, sizeof(float) * 3u),
        });
    }

    public void Render(GL gl, Matrix4x4 view, Matrix4x4 projection)
    {
        gl.Disable(EnableCap.DepthTest);

        _shader.Use();
        _shader.SetUniform("uModel", Matrix4x4.Identity);
        _shader.SetUniform("uView", view);
        _shader.SetUniform("uProjection", projection);

        _vao.Bind();
        gl.DrawArrays(PrimitiveType.Lines, 0, (uint)_vertexCount);

        gl.Enable(EnableCap.DepthTest);
    }

    public void Dispose()
    {
        _shader.Dispose();
        _vao.Dispose();
        _vertexBuffer.Dispose();
    }
}
