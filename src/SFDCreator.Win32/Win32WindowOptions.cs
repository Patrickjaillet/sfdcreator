using SFDCreator.Win32.Menu;

namespace SFDCreator.Win32;

public sealed class Win32WindowOptions
{
    public string Title { get; init; } = "SFD Creator";

    public int Width { get; init; } = 1280;

    public int Height { get; init; } = 720;

    public int? X { get; init; }

    public int? Y { get; init; }

    public bool Maximized { get; init; }

    public NativeMenuBuilder? MenuBuilder { get; init; }
}
