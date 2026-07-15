using SkiaSharp;

namespace SFDCreator.UI.Hosting;

public interface IUiPanelContent
{
    void Render(SKCanvas canvas, SKRect bounds, UiInputState input);
}
