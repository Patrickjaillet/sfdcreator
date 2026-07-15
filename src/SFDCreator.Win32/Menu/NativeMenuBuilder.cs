using SFDCreator.Win32.Interop;

namespace SFDCreator.Win32.Menu;

public sealed class NativeMenuBuilder
{
    private sealed class TopLevelMenu
    {
        public required string Text { get; init; }
        public List<(string Text, int CommandId)> Items { get; } = new();
    }

    private readonly List<TopLevelMenu> _topLevelMenus = new();
    private TopLevelMenu? _current;

    public NativeMenuBuilder AddTopLevel(string text)
    {
        _current = new TopLevelMenu { Text = text };
        _topLevelMenus.Add(_current);
        return this;
    }

    public NativeMenuBuilder AddItem(string text, int commandId)
    {
        if (_current is null)
        {
            throw new InvalidOperationException("Call AddTopLevel before AddItem.");
        }

        _current.Items.Add((text, commandId));
        return this;
    }

    internal nint Build()
    {
        var menuBar = User32.CreateMenu();

        foreach (var topLevel in _topLevelMenus)
        {
            var popup = User32.CreatePopupMenu();

            foreach (var (text, commandId) in topLevel.Items)
            {
                User32.AppendMenuW(popup, NativeConstants.MF_STRING, (nuint)commandId, text);
            }

            User32.AppendMenuW(menuBar, NativeConstants.MF_POPUP, (nuint)popup, topLevel.Text);
        }

        return menuBar;
    }
}
