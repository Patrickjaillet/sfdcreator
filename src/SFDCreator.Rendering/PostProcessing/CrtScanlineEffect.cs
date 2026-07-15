using System.Numerics;
using Silk.NET.OpenGL;
using SFDCreator.Rendering.Resources;

namespace SFDCreator.Rendering.PostProcessing;

public sealed class CrtScanlineEffect : IDisposable
{
    private readonly ShaderProgram _shader;

    public float ScanlineStrength { get; set; } = 0.25f;

    public float VignetteStrength { get; set; } = 0.35f;

    public float Curvature { get; set; } = 0.08f;

    public CrtScanlineEffect(GL gl, string shaderDirectory)
    {
        _shader = ShaderLoader.Load(gl, shaderDirectory, "fullscreen.vert", "crt_scanline.frag");
    }

    public void Apply(GL gl, FullScreenQuad quad, RenderTarget source, RenderTarget destination)
    {
        destination.Bind();
        gl.Viewport(0, 0, destination.Width, destination.Height);
        _shader.Use();
        source.ColorTexture.Bind(0);
        _shader.SetUniform("uSource", 0);
        _shader.SetUniform("uResolution", new Vector2(destination.Width, destination.Height));
        _shader.SetUniform("uScanlineStrength", ScanlineStrength);
        _shader.SetUniform("uVignetteStrength", VignetteStrength);
        _shader.SetUniform("uCurvature", Curvature);
        quad.Draw();
    }

    public void Dispose() => _shader.Dispose();
}
