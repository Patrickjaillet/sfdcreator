using System.Numerics;
using Silk.NET.OpenGL;
using SFDCreator.Rendering.Resources;

namespace SFDCreator.Rendering.PostProcessing;

public sealed class BloomEffect : IDisposable
{
    private readonly ShaderProgram _brightPass;
    private readonly ShaderProgram _blur;
    private readonly ShaderProgram _composite;
    private readonly RenderTarget _brightTarget;
    private readonly RenderTarget _blurTargetA;
    private readonly RenderTarget _blurTargetB;

    public float Threshold { get; set; } = 0.8f;

    public float Intensity { get; set; } = 0.6f;

    public BloomEffect(GL gl, string shaderDirectory, uint width, uint height)
    {
        _brightPass = ShaderLoader.Load(gl, shaderDirectory, "fullscreen.vert", "bloom_bright_pass.frag");
        _blur = ShaderLoader.Load(gl, shaderDirectory, "fullscreen.vert", "blur.frag");
        _composite = ShaderLoader.Load(gl, shaderDirectory, "fullscreen.vert", "bloom_composite.frag");

        _brightTarget = new RenderTarget(gl, width, height, depthStencil: false);
        _blurTargetA = new RenderTarget(gl, width, height, depthStencil: false);
        _blurTargetB = new RenderTarget(gl, width, height, depthStencil: false);
    }

    public void Resize(uint width, uint height)
    {
        _brightTarget.Resize(width, height);
        _blurTargetA.Resize(width, height);
        _blurTargetB.Resize(width, height);
    }

    public void Apply(GL gl, FullScreenQuad quad, RenderTarget source, RenderTarget destination)
    {
        _brightTarget.Bind();
        gl.Viewport(0, 0, _brightTarget.Width, _brightTarget.Height);
        _brightPass.Use();
        source.ColorTexture.Bind(0);
        _brightPass.SetUniform("uSource", 0);
        _brightPass.SetUniform("uThreshold", Threshold);
        quad.Draw();

        _blurTargetA.Bind();
        gl.Viewport(0, 0, _blurTargetA.Width, _blurTargetA.Height);
        _blur.Use();
        _brightTarget.ColorTexture.Bind(0);
        _blur.SetUniform("uSource", 0);
        _blur.SetUniform("uDirection", new Vector2(1, 0));
        _blur.SetUniform("uTexelSize", new Vector2(1f / _brightTarget.Width, 1f / _brightTarget.Height));
        quad.Draw();

        _blurTargetB.Bind();
        gl.Viewport(0, 0, _blurTargetB.Width, _blurTargetB.Height);
        _blurTargetA.ColorTexture.Bind(0);
        _blur.SetUniform("uSource", 0);
        _blur.SetUniform("uDirection", new Vector2(0, 1));
        quad.Draw();

        destination.Bind();
        gl.Viewport(0, 0, destination.Width, destination.Height);
        _composite.Use();
        source.ColorTexture.Bind(0);
        _composite.SetUniform("uBase", 0);
        _blurTargetB.ColorTexture.Bind(1);
        _composite.SetUniform("uBloom", 1);
        _composite.SetUniform("uIntensity", Intensity);
        quad.Draw();
    }

    public void Dispose()
    {
        _brightPass.Dispose();
        _blur.Dispose();
        _composite.Dispose();
        _brightTarget.Dispose();
        _blurTargetA.Dispose();
        _blurTargetB.Dispose();
    }
}
