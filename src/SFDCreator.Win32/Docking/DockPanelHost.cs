namespace SFDCreator.Win32.Docking;

public sealed class DockPanelHost
{
    private readonly nint _parentHwnd;
    private readonly Dictionary<DockRegion, Win32ChildPanel> _panels = new();

    public int TopExtent { get; set; } = 36;

    public int LeftExtent { get; set; } = 250;

    public int RightExtent { get; set; } = 250;

    public int BottomExtent { get; set; } = 200;

    public DockPanelHost(Win32Window host)
    {
        _parentHwnd = host.Handle;

        foreach (var region in Enum.GetValues<DockRegion>())
        {
            _panels[region] = new Win32ChildPanel(_parentHwnd);
        }

        host.Resized += OnHostResized;
        var (width, height) = host.ClientSize;
        Layout(width, height);
    }

    public event Action<DockRegion, int, int>? PanelResized;

    public Win32ChildPanel GetPanel(DockRegion region) => _panels[region];

    public nint GetPanelHandle(DockRegion region) => _panels[region].Handle;

    public (int Width, int Height) GetPanelSize(DockRegion region) => _panels[region].ClientSize;

    private void OnHostResized(int width, int height) => Layout(width, height);

    private void Layout(int width, int height)
    {
        var rects = DockLayout.Compute(width, height, LeftExtent, RightExtent, TopExtent, BottomExtent);

        foreach (var (region, panel) in _panels)
        {
            var rect = rects[region];
            var panelWidth = Math.Max(rect.Width, 0);
            var panelHeight = Math.Max(rect.Height, 0);
            Interop.User32.MoveWindow(panel.Handle, rect.X, rect.Y, panelWidth, panelHeight, true);
            PanelResized?.Invoke(region, panelWidth, panelHeight);
        }
    }
}
