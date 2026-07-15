namespace SFDCreator.Rendering.Backend;

public sealed class GraphicsDeviceOptions
{
    public bool VSync { get; init; } = true;

    public int MajorVersion { get; init; } = 4;

    public int MinorVersion { get; init; } = 6;

    public int DepthBits { get; init; } = 24;

    public int StencilBits { get; init; } = 8;
}
