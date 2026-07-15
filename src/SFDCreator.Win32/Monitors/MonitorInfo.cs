namespace SFDCreator.Win32.Monitors;

public readonly record struct MonitorInfo(
    int Left,
    int Top,
    int Right,
    int Bottom,
    int WorkLeft,
    int WorkTop,
    int WorkRight,
    int WorkBottom,
    bool IsPrimary);
