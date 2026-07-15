namespace SFDCreator.Core.Settings;

public sealed record ApplicationSettings
{
    public int WindowX { get; init; } = -1;

    public int WindowY { get; init; } = -1;

    public int WindowWidth { get; init; } = 1280;

    public int WindowHeight { get; init; } = 720;

    public bool WindowMaximized { get; init; }
}
