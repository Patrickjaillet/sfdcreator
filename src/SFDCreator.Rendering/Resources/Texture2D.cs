using Silk.NET.OpenGL;

namespace SFDCreator.Rendering.Resources;

public sealed class Texture2D : IDisposable
{
    private readonly GL _gl;

    public uint Handle { get; }

    public uint Width { get; private set; }

    public uint Height { get; private set; }

    public Texture2D(GL gl, uint width, uint height, InternalFormat internalFormat = InternalFormat.Rgba8, PixelFormat pixelFormat = PixelFormat.Rgba)
    {
        _gl = gl;
        Handle = _gl.GenTextures(1);
        Allocate(width, height, internalFormat, pixelFormat);
    }

    public unsafe void Allocate(uint width, uint height, InternalFormat internalFormat, PixelFormat pixelFormat)
    {
        Width = width;
        Height = height;

        Bind();
        _gl.TexImage2D(TextureTarget.Texture2D, 0, internalFormat, width, height, 0, pixelFormat, PixelType.UnsignedByte, null);
        _gl.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)GLEnum.Linear);
        _gl.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)GLEnum.Linear);
        _gl.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)GLEnum.ClampToEdge);
        _gl.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)GLEnum.ClampToEdge);
    }

    public void Bind(uint unit = 0)
    {
        _gl.ActiveTexture(TextureUnit.Texture0 + (int)unit);
        _gl.BindTexture(TextureTarget.Texture2D, Handle);
    }

    public void Dispose() => _gl.DeleteTexture(Handle);
}
