namespace SFDCreator.Rendering.Windowing;

public sealed class RenderWindowOptions
{
    public string Title { get; init; } = "SFD Creator";

    public int Width { get; init; } = 1280;

    public int Height { get; init; } = 720;

    public bool VSync { get; init; } = true;
}
