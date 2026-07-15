namespace SFDCreator.Win32.Docking;

public static class DockLayout
{
    public static IReadOnlyDictionary<DockRegion, DockLayoutRect> Compute(
        int clientWidth,
        int clientHeight,
        int leftExtent,
        int rightExtent,
        int bottomExtent)
    {
        leftExtent = Math.Clamp(leftExtent, 0, Math.Max(0, clientWidth));
        rightExtent = Math.Clamp(rightExtent, 0, Math.Max(0, clientWidth - leftExtent));
        bottomExtent = Math.Clamp(bottomExtent, 0, Math.Max(0, clientHeight));

        var centerWidth = Math.Max(0, clientWidth - leftExtent - rightExtent);
        var centerHeight = Math.Max(0, clientHeight - bottomExtent);

        return new Dictionary<DockRegion, DockLayoutRect>
        {
            [DockRegion.Left] = new DockLayoutRect(0, 0, leftExtent, clientHeight),
            [DockRegion.Right] = new DockLayoutRect(clientWidth - rightExtent, 0, rightExtent, clientHeight),
            [DockRegion.Bottom] = new DockLayoutRect(leftExtent, centerHeight, centerWidth, bottomExtent),
            [DockRegion.Center] = new DockLayoutRect(leftExtent, 0, centerWidth, centerHeight),
        };
    }
}
