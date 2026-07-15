using System.Numerics;
using Silk.NET.OpenGL;
using SFDCreator.Rendering.Resources;

namespace SFDCreator.Rendering.PostProcessing;

public sealed class MotionBlurEffect : IDisposable
{
    private readonly ShaderProgram _shader;
    private Matrix4x4? _previousViewProjection;

    public int SampleCount { get; set; } = 8;

    public float Intensity { get; set; } = 1f;

    public MotionBlurEffect(GL gl, string shaderDirectory)
    {
        _shader = ShaderLoader.Load(gl, shaderDirectory, "fullscreen.vert", "motion_blur.frag");
    }

    public void Apply(GL gl, FullScreenQuad quad, RenderTarget source, RenderTarget destination, Matrix4x4 viewProjection)
    {
        var direction = Vector2.Zero;

        if (_previousViewProjection is { } previous)
        {
            var currentNdc = Vector4.Transform(Vector4.UnitW, viewProjection);
            var previousNdc = Vector4.Transform(Vector4.UnitW, previous);

            if (MathF.Abs(currentNdc.W) > 0.0001f && MathF.Abs(previousNdc.W) > 0.0001f)
            {
                var currentScreen = new Vector2(currentNdc.X / currentNdc.W, currentNdc.Y / currentNdc.W);
                var previousScreen = new Vector2(previousNdc.X / previousNdc.W, previousNdc.Y / previousNdc.W);
                direction = (currentScreen - previousScreen) * Intensity;
            }
        }

        _previousViewProjection = viewProjection;

        destination.Bind();
        gl.Viewport(0, 0, destination.Width, destination.Height);
        _shader.Use();
        source.ColorTexture.Bind(0);
        _shader.SetUniform("uSource", 0);
        _shader.SetUniform("uBlurDirection", direction);
        _shader.SetUniform("uSampleCount", SampleCount);
        quad.Draw();
    }

    public void Dispose() => _shader.Dispose();
}
