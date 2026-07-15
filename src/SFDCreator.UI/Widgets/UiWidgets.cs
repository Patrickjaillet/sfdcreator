using SFDCreator.UI.Hosting;
using SFDCreator.UI.Theming;
using SkiaSharp;

namespace SFDCreator.UI.Widgets;

public static class UiWidgets
{
    private static readonly SKFont Font = new(SKTypeface.Default, 13f);

    public static bool Button(SKCanvas canvas, SKRect rect, string label, UiInputState input, UiTheme theme)
    {
        var hovered = rect.Contains(input.MouseX, input.MouseY);
        var clicked = hovered && input.WasLeftPressedThisFrame;

        using var fillPaint = new SKPaint
        {
            IsAntialias = true,
            Color = hovered ? theme.AccentHover : theme.Panel,
            Style = SKPaintStyle.Fill,
        };

        using var borderPaint = new SKPaint
        {
            IsAntialias = true,
            Color = theme.Border,
            Style = SKPaintStyle.Stroke,
            StrokeWidth = 1f,
        };

        canvas.DrawRoundRect(rect, 4f, 4f, fillPaint);
        canvas.DrawRoundRect(rect, 4f, 4f, borderPaint);

        DrawCenteredText(canvas, rect, label, hovered ? theme.Background : theme.Text);

        return clicked;
    }

    public static (bool Changed, float Value) Slider(
        SKCanvas canvas,
        SKRect rect,
        string label,
        float value,
        float min,
        float max,
        UiInputState input,
        UiTheme theme)
    {
        using var trackPaint = new SKPaint { IsAntialias = true, Color = theme.Border, Style = SKPaintStyle.Fill };
        using var fillPaint = new SKPaint { IsAntialias = true, Color = theme.Accent, Style = SKPaintStyle.Fill };

        var trackRect = new SKRect(rect.Left, rect.MidY - 3f, rect.Right, rect.MidY + 3f);
        canvas.DrawRoundRect(trackRect, 3f, 3f, trackPaint);

        var handleX = SliderMath.ValueToPosition(trackRect.Left, trackRect.Width, min, max, value);
        var filledRect = new SKRect(trackRect.Left, trackRect.Top, handleX, trackRect.Bottom);
        canvas.DrawRoundRect(filledRect, 3f, 3f, fillPaint);

        using var handlePaint = new SKPaint { IsAntialias = true, Color = theme.Text, Style = SKPaintStyle.Fill };
        canvas.DrawCircle(handleX, rect.MidY, 6f, handlePaint);

        var hovered = rect.Contains(input.MouseX, input.MouseY);
        var dragging = input.IsLeftDown && (hovered || input.WasLeftPressedThisFrame);

        var changed = false;
        var newValue = value;

        if (dragging && input.IsLeftDown)
        {
            newValue = SliderMath.PositionToValue(trackRect.Left, trackRect.Width, min, max, input.MouseX);
            changed = Math.Abs(newValue - value) > float.Epsilon;
        }

        DrawCenteredText(canvas, new SKRect(rect.Left, rect.Top - 16f, rect.Right, rect.Top), $"{label}: {value:0.###}", theme.TextMuted, SKTextAlign.Left);

        return (changed, newValue);
    }

    public static (bool Changed, bool Value) Checkbox(SKCanvas canvas, SKRect rect, string label, bool isChecked, UiInputState input, UiTheme theme)
    {
        var boxRect = new SKRect(rect.Left, rect.MidY - 7f, rect.Left + 14f, rect.MidY + 7f);

        using var boxPaint = new SKPaint { IsAntialias = true, Color = isChecked ? theme.Accent : theme.Panel, Style = SKPaintStyle.Fill };
        using var borderPaint = new SKPaint { IsAntialias = true, Color = theme.Border, Style = SKPaintStyle.Stroke, StrokeWidth = 1f };

        canvas.DrawRoundRect(boxRect, 2f, 2f, boxPaint);
        canvas.DrawRoundRect(boxRect, 2f, 2f, borderPaint);

        using var labelPaint = new SKPaint { IsAntialias = true, Color = theme.Text };
        canvas.DrawText(label, boxRect.Right + 8f, rect.MidY + 4f, SKTextAlign.Left, Font, labelPaint);

        var hovered = rect.Contains(input.MouseX, input.MouseY);
        var clicked = hovered && input.WasLeftPressedThisFrame;

        return (clicked, clicked ? !isChecked : isChecked);
    }

    public static void Label(SKCanvas canvas, float x, float y, string text, UiTheme theme, bool muted = false)
    {
        using var paint = new SKPaint { IsAntialias = true, Color = muted ? theme.TextMuted : theme.Text };
        canvas.DrawText(text, x, y, SKTextAlign.Left, Font, paint);
    }

    private static void DrawCenteredText(SKCanvas canvas, SKRect rect, string text, SKColor color, SKTextAlign align = SKTextAlign.Center)
    {
        using var paint = new SKPaint { IsAntialias = true, Color = color };
        var x = align == SKTextAlign.Center ? rect.MidX : rect.Left;
        var y = rect.MidY + (Font.Size * 0.35f);
        canvas.DrawText(text, x, y, align, Font, paint);
    }
}
