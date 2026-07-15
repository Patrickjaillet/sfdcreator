using System.Numerics;
using Silk.NET.OpenGL;
using SFDCreator.Rendering.Resources;

namespace SFDCreator.Rendering.PostProcessing;

public sealed class ColorGradingEffect : IDisposable
{
    private readonly ShaderProgram _shader;

    public Vector3 Lift { get; set; } = Vector3.Zero;

    public Vector3 Gamma { get; set; } = Vector3.One;

    public Vector3 Gain { get; set; } = Vector3.One;

    public float Saturation { get; set; } = 1f;

    public float Contrast { get; set; } = 1f;

    public ColorGradingEffect(GL gl, string shaderDirectory)
    {
        _shader = ShaderLoader.Load(gl, shaderDirectory, "fullscreen.vert", "color_grading.frag");
    }

    public void Apply(GL gl, FullScreenQuad quad, RenderTarget source, RenderTarget destination)
    {
        destination.Bind();
        gl.Viewport(0, 0, destination.Width, destination.Height);
        _shader.Use();
        source.ColorTexture.Bind(0);
        _shader.SetUniform("uSource", 0);
        _shader.SetUniform("uLift", Lift);
        _shader.SetUniform("uGamma", Gamma);
        _shader.SetUniform("uGain", Gain);
        _shader.SetUniform("uSaturation", Saturation);
        _shader.SetUniform("uContrast", Contrast);
        quad.Draw();
    }

    public void Dispose() => _shader.Dispose();
}
