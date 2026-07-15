using SFDCreator.Win32.Interop;

namespace SFDCreator.Win32.Docking;

public sealed class DockPanelHost
{
    private readonly nint _parentHwnd;
    private readonly Dictionary<DockRegion, nint> _panels = new();

    public int LeftExtent { get; set; } = 250;

    public int RightExtent { get; set; } = 250;

    public int BottomExtent { get; set; } = 200;

    public DockPanelHost(Win32Window host)
    {
        _parentHwnd = host.Handle;

        foreach (var region in Enum.GetValues<DockRegion>())
        {
            var child = User32.CreateWindowExW(
                0,
                "STATIC",
                region.ToString(),
                NativeConstants.WS_CHILD | NativeConstants.WS_VISIBLE,
                0, 0, 0, 0,
                _parentHwnd,
                0,
                0,
                0);

            _panels[region] = child;
        }

        host.Resized += OnHostResized;
        var (width, height) = host.ClientSize;
        Layout(width, height);
    }

    public nint GetPanelHandle(DockRegion region) => _panels[region];

    private void OnHostResized(int width, int height) => Layout(width, height);

    private void Layout(int width, int height)
    {
        var rects = DockLayout.Compute(width, height, LeftExtent, RightExtent, BottomExtent);

        foreach (var (region, hwnd) in _panels)
        {
            var rect = rects[region];
            User32.MoveWindow(hwnd, rect.X, rect.Y, Math.Max(rect.Width, 0), Math.Max(rect.Height, 0), true);
        }
    }
}
