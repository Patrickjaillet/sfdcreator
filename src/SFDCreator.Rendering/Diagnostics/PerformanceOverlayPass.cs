using System.Numerics;
using System.Runtime.InteropServices;
using Silk.NET.OpenGL;
using SFDCreator.Rendering.Resources;

namespace SFDCreator.Rendering.Diagnostics;

public sealed class PerformanceOverlayPass : IDisposable
{
    private const int MaxSamples = 64;
    private const float MarginX = 16f;
    private const float MarginY = 16f;
    private const float GraphWidth = 160f;
    private const float GraphHeight = 48f;

    private readonly ShaderProgram _shader;
    private readonly GpuBuffer _vertexBuffer;
    private readonly Resources.VertexArray _vao;
    private readonly List<Vertex> _scratch = new();

    public PerformanceOverlayPass(GL gl, string shaderDirectory)
    {
        _shader = ShaderLoader.Load(gl, shaderDirectory, "scene.vert", "scene.frag");
        _vertexBuffer = new GpuBuffer(gl, BufferTargetARB.ArrayBuffer);
        _vao = new Resources.VertexArray(gl, _vertexBuffer, null, (uint)(sizeof(float) * 6), new[]
        {
            new VertexAttribute(0, 3, 0),
            new VertexAttribute(1, 3, sizeof(float) * 3u),
        });
    }

    public void Render(GL gl, FrameStats stats, int screenWidth, int screenHeight)
    {
        if (screenWidth <= 0 || screenHeight <= 0)
        {
            return;
        }

        var samples = stats.Samples;
        var count = Math.Min(samples.Count, MaxSamples);
        if (count == 0)
        {
            return;
        }

        _scratch.Clear();
        var barWidth = GraphWidth / MaxSamples;

        for (var i = 0; i < count; i++)
        {
            var sample = samples[samples.Count - count + i];
            var (normalizedHeight, band) = PerformanceGraphMath.NormalizeFrameTime(sample, 60f);

            var barLeftPx = MarginX + i * barWidth;
            var barRightPx = barLeftPx + barWidth * 0.8f;
            var barBottomPx = screenHeight - MarginY;
            var barTopPx = barBottomPx - normalizedHeight * GraphHeight;

            var color = band switch
            {
                PerformanceBand.Good => new Vector3(0.25f, 0.85f, 0.35f),
                PerformanceBand.Warning => new Vector3(0.95f, 0.8f, 0.2f),
                _ => new Vector3(0.95f, 0.25f, 0.25f),
            };

            var topLeft = PixelToNdc(barLeftPx, barTopPx, screenWidth, screenHeight);
            var topRight = PixelToNdc(barRightPx, barTopPx, screenWidth, screenHeight);
            var bottomLeft = PixelToNdc(barLeftPx, barBottomPx, screenWidth, screenHeight);
            var bottomRight = PixelToNdc(barRightPx, barBottomPx, screenWidth, screenHeight);

            _scratch.Add(new Vertex(bottomLeft, color));
            _scratch.Add(new Vertex(bottomRight, color));
            _scratch.Add(new Vertex(topRight, color));

            _scratch.Add(new Vertex(bottomLeft, color));
            _scratch.Add(new Vertex(topRight, color));
            _scratch.Add(new Vertex(topLeft, color));
        }

        gl.Disable(EnableCap.DepthTest);

        _shader.Use();
        _shader.SetUniform("uModel", Matrix4x4.Identity);
        _shader.SetUniform("uView", Matrix4x4.Identity);
        _shader.SetUniform("uProjection", Matrix4x4.Identity);

        _vertexBuffer.Upload<Vertex>(CollectionsMarshal.AsSpan(_scratch), BufferUsageARB.StreamDraw);
        _vao.Bind();
        gl.DrawArrays(PrimitiveType.Triangles, 0, (uint)_scratch.Count);

        gl.Enable(EnableCap.DepthTest);
    }

    private static Vector3 PixelToNdc(float px, float py, int screenWidth, int screenHeight)
    {
        var ndcX = (px / screenWidth * 2f) - 1f;
        var ndcY = 1f - (py / screenHeight * 2f);
        return new Vector3(ndcX, ndcY, 0f);
    }

    public void Dispose()
    {
        _shader.Dispose();
        _vao.Dispose();
        _vertexBuffer.Dispose();
    }

    [StructLayout(LayoutKind.Sequential)]
    private readonly struct Vertex
    {
        public readonly Vector3 Position;
        public readonly Vector3 Color;

        public Vertex(Vector3 position, Vector3 color)
        {
            Position = position;
            Color = color;
        }
    }
}
