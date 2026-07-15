using SFDCreator.UI.Hosting;
using SFDCreator.UI.Theming;
using SFDCreator.UI.Widgets;
using SkiaSharp;

namespace SFDCreator.UI.Panels;

public sealed class ToolbarPanelContent : IUiPanelContent
{
    private readonly (string Label, int CommandId)[] _buttons;

    public ToolbarPanelContent(params (string Label, int CommandId)[] buttons)
    {
        _buttons = buttons;
    }

    public event Action<int>? CommandInvoked;

    public void Render(SKCanvas canvas, SKRect bounds, UiInputState input)
    {
        var theme = UiTheme.Current;

        using var background = new SKPaint { Color = theme.Panel, Style = SKPaintStyle.Fill };
        canvas.DrawRect(bounds, background);

        const float buttonHeight = 24f;
        const float buttonWidth = 76f;
        var x = 8f;
        var y = (bounds.Height - buttonHeight) / 2f;

        foreach (var (label, commandId) in _buttons)
        {
            var rect = new SKRect(x, y, x + buttonWidth, y + buttonHeight);

            if (UiWidgets.Button(canvas, rect, label, input, theme))
            {
                CommandInvoked?.Invoke(commandId);
            }

            x += buttonWidth + 6f;
        }
    }
}
