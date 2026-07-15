using System.Numerics;
using Silk.NET.OpenGL;
using SFDCreator.Rendering.Resources;

namespace SFDCreator.Rendering.PostProcessing;

public sealed class PostProcessChain : IDisposable
{
    private readonly FullScreenQuad _quad;
    private readonly BloomEffect _bloom;
    private readonly ColorGradingEffect _colorGrading;
    private readonly CrtScanlineEffect _crtScanline;
    private readonly MotionBlurEffect _motionBlur;
    private readonly ShaderProgram _blit;
    private RenderTarget _pingA;
    private RenderTarget _pingB;

    public bool MotionBlurEnabled { get; set; }

    public BloomEffect Bloom => _bloom;

    public ColorGradingEffect ColorGrading => _colorGrading;

    public CrtScanlineEffect CrtScanline => _crtScanline;

    public MotionBlurEffect MotionBlur => _motionBlur;

    public PostProcessChain(GL gl, string shaderDirectory, uint width, uint height)
    {
        _quad = new FullScreenQuad(gl);
        _bloom = new BloomEffect(gl, shaderDirectory, width, height);
        _colorGrading = new ColorGradingEffect(gl, shaderDirectory);
        _crtScanline = new CrtScanlineEffect(gl, shaderDirectory);
        _motionBlur = new MotionBlurEffect(gl, shaderDirectory);
        _blit = ShaderLoader.Load(gl, shaderDirectory, "fullscreen.vert", "blit.frag");
        _pingA = new RenderTarget(gl, width, height, depthStencil: false);
        _pingB = new RenderTarget(gl, width, height, depthStencil: false);
    }

    public void Resize(uint width, uint height)
    {
        _bloom.Resize(width, height);
        _pingA.Resize(width, height);
        _pingB.Resize(width, height);
    }

    public void Apply(GL gl, RenderTarget source, int screenWidth, int screenHeight, Matrix4x4 viewProjection)
    {
        _bloom.Apply(gl, _quad, source, _pingA);
        var current = _pingA;

        _colorGrading.Apply(gl, _quad, current, _pingB);
        current = _pingB;

        if (MotionBlurEnabled)
        {
            var other = ReferenceEquals(current, _pingA) ? _pingB : _pingA;
            _motionBlur.Apply(gl, _quad, current, other, viewProjection);
            current = other;
        }

        var final = ReferenceEquals(current, _pingA) ? _pingB : _pingA;
        _crtScanline.Apply(gl, _quad, current, final);

        RenderTarget.BindDefault(gl);
        gl.Viewport(0, 0, (uint)screenWidth, (uint)screenHeight);
        _blit.Use();
        final.ColorTexture.Bind(0);
        _blit.SetUniform("uSource", 0);
        _quad.Draw();
    }

    public void Dispose()
    {
        _quad.Dispose();
        _bloom.Dispose();
        _colorGrading.Dispose();
        _crtScanline.Dispose();
        _motionBlur.Dispose();
        _blit.Dispose();
        _pingA.Dispose();
        _pingB.Dispose();
    }
}
