using SkiaSharp;

namespace SFDCreator.UI.Theming;

public sealed class UiTheme
{
    public required SKColor Background { get; init; }

    public required SKColor Panel { get; init; }

    public required SKColor Border { get; init; }

    public required SKColor Text { get; init; }

    public required SKColor TextMuted { get; init; }

    public required SKColor Accent { get; init; }

    public required SKColor AccentHover { get; init; }

    public static UiTheme Dark { get; } = new()
    {
        Background = new SKColor(0x1A, 0x1B, 0x1E),
        Panel = new SKColor(0x24, 0x25, 0x2A),
        Border = new SKColor(0x36, 0x38, 0x3F),
        Text = new SKColor(0xE8, 0xE9, 0xEC),
        TextMuted = new SKColor(0x9A, 0x9D, 0xA6),
        Accent = new SKColor(0x4F, 0x8E, 0xF7),
        AccentHover = new SKColor(0x6F, 0xA6, 0xF9),
    };

    public static UiTheme Light { get; } = new()
    {
        Background = new SKColor(0xF2, 0xF2, 0xF4),
        Panel = new SKColor(0xFF, 0xFF, 0xFF),
        Border = new SKColor(0xD6, 0xD7, 0xDB),
        Text = new SKColor(0x20, 0x21, 0x24),
        TextMuted = new SKColor(0x6B, 0x6E, 0x76),
        Accent = new SKColor(0x2F, 0x6F, 0xE0),
        AccentHover = new SKColor(0x4E, 0x87, 0xEA),
    };

    public static UiTheme Current { get; set; } = Dark;
}
