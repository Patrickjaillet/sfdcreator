using Silk.NET.OpenGL;

namespace SFDCreator.Rendering.Resources;

public sealed class RenderTarget : IDisposable
{
    private readonly GL _gl;
    private uint _framebuffer;
    private uint? _depthStencilRenderbuffer;

    public Texture2D ColorTexture { get; private set; }

    public uint Width { get; private set; }

    public uint Height { get; private set; }

    public bool HasDepthStencil { get; }

    public RenderTarget(GL gl, uint width, uint height, bool depthStencil = true)
    {
        _gl = gl;
        HasDepthStencil = depthStencil;

        width = Math.Max(width, 1);
        height = Math.Max(height, 1);

        ColorTexture = new Texture2D(gl, width, height);
        Create(width, height);
    }

    public void Resize(uint width, uint height)
    {
        width = Math.Max(width, 1);
        height = Math.Max(height, 1);

        if (width == Width && height == Height)
        {
            return;
        }

        ColorTexture.Dispose();
        ColorTexture = new Texture2D(_gl, width, height);

        if (_depthStencilRenderbuffer is { } renderbuffer)
        {
            _gl.DeleteRenderbuffer(renderbuffer);
        }

        _gl.DeleteFramebuffer(_framebuffer);

        Create(width, height);
    }

    public void Bind() => _gl.BindFramebuffer(FramebufferTarget.Framebuffer, _framebuffer);

    public static void BindDefault(GL gl) => gl.BindFramebuffer(FramebufferTarget.Framebuffer, 0);

    public void Dispose()
    {
        ColorTexture.Dispose();

        if (_depthStencilRenderbuffer is { } renderbuffer)
        {
            _gl.DeleteRenderbuffer(renderbuffer);
        }

        _gl.DeleteFramebuffer(_framebuffer);
    }

    private void Create(uint width, uint height)
    {
        Width = width;
        Height = height;

        _framebuffer = _gl.GenFramebuffers(1);
        _gl.BindFramebuffer(FramebufferTarget.Framebuffer, _framebuffer);
        _gl.FramebufferTexture2D(FramebufferTarget.Framebuffer, FramebufferAttachment.ColorAttachment0, TextureTarget.Texture2D, ColorTexture.Handle, 0);

        if (HasDepthStencil)
        {
            var renderbuffer = _gl.GenRenderbuffers(1);
            _depthStencilRenderbuffer = renderbuffer;
            _gl.BindRenderbuffer(RenderbufferTarget.Renderbuffer, renderbuffer);
            _gl.RenderbufferStorage(RenderbufferTarget.Renderbuffer, InternalFormat.Depth24Stencil8, width, height);
            _gl.FramebufferRenderbuffer(FramebufferTarget.Framebuffer, FramebufferAttachment.DepthStencilAttachment, RenderbufferTarget.Renderbuffer, renderbuffer);
        }

        var status = _gl.CheckFramebufferStatus(FramebufferTarget.Framebuffer);
        if (status != GLEnum.FramebufferComplete)
        {
            throw new InvalidOperationException($"Framebuffer is incomplete: {status}");
        }

        _gl.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
    }
}
