using SFDCreator.Rendering.PostProcessing;
using SFDCreator.UI.Hosting;
using SFDCreator.UI.Theming;
using SFDCreator.UI.Widgets;
using SkiaSharp;

namespace SFDCreator.UI.Panels;

public sealed class PropertyInspectorContent : IUiPanelContent
{
    private readonly BloomEffect _bloom;
    private readonly ColorGradingEffect _colorGrading;
    private readonly CrtScanlineEffect _crt;
    private readonly Func<float> _getRotationSpeed;
    private readonly Action<float> _setRotationSpeed;

    public PropertyInspectorContent(
        BloomEffect bloom,
        ColorGradingEffect colorGrading,
        CrtScanlineEffect crt,
        Func<float> getRotationSpeed,
        Action<float> setRotationSpeed)
    {
        _bloom = bloom;
        _colorGrading = colorGrading;
        _crt = crt;
        _getRotationSpeed = getRotationSpeed;
        _setRotationSpeed = setRotationSpeed;
    }

    public void Render(SKCanvas canvas, SKRect bounds, UiInputState input)
    {
        var theme = UiTheme.Current;

        using var background = new SKPaint { Color = theme.Panel, Style = SKPaintStyle.Fill };
        canvas.DrawRect(bounds, background);

        UiWidgets.Label(canvas, bounds.Left + 12f, bounds.Top + 16f, "Property Inspector", theme);

        var y = bounds.Top + 40f;

        float Row(string label, float value, float min, float max)
        {
            var rect = new SKRect(bounds.Left + 12f, y, bounds.Right - 12f, y + 16f);
            var (changed, newValue) = UiWidgets.Slider(canvas, rect, label, value, min, max, input, theme);
            y += 40f;
            return changed ? newValue : value;
        }

        _setRotationSpeed(Row("Rotation Speed", _getRotationSpeed(), 0f, 3f));
        _bloom.Threshold = Row("Bloom Threshold", _bloom.Threshold, 0f, 2f);
        _bloom.Intensity = Row("Bloom Intensity", _bloom.Intensity, 0f, 3f);
        _colorGrading.Saturation = Row("Saturation", _colorGrading.Saturation, 0f, 2f);
        _colorGrading.Contrast = Row("Contrast", _colorGrading.Contrast, 0f, 2f);
        _crt.ScanlineStrength = Row("Scanline Strength", _crt.ScanlineStrength, 0f, 1f);
        _crt.VignetteStrength = Row("Vignette Strength", _crt.VignetteStrength, 0f, 1f);
        _crt.Curvature = Row("Curvature", _crt.Curvature, 0f, 0.3f);
    }
}
