namespace SFDCreator.Win32.Docking;

public static class DockLayout
{
    public static IReadOnlyDictionary<DockRegion, DockLayoutRect> Compute(
        int clientWidth,
        int clientHeight,
        int leftExtent,
        int rightExtent,
        int topExtent,
        int bottomExtent)
    {
        topExtent = Math.Clamp(topExtent, 0, Math.Max(0, clientHeight));
        var remainingHeight = Math.Max(0, clientHeight - topExtent);

        leftExtent = Math.Clamp(leftExtent, 0, Math.Max(0, clientWidth));
        rightExtent = Math.Clamp(rightExtent, 0, Math.Max(0, clientWidth - leftExtent));
        bottomExtent = Math.Clamp(bottomExtent, 0, remainingHeight);

        var centerWidth = Math.Max(0, clientWidth - leftExtent - rightExtent);
        var centerHeight = Math.Max(0, remainingHeight - bottomExtent);

        return new Dictionary<DockRegion, DockLayoutRect>
        {
            [DockRegion.Top] = new DockLayoutRect(0, 0, clientWidth, topExtent),
            [DockRegion.Left] = new DockLayoutRect(0, topExtent, leftExtent, remainingHeight),
            [DockRegion.Right] = new DockLayoutRect(clientWidth - rightExtent, topExtent, rightExtent, remainingHeight),
            [DockRegion.Bottom] = new DockLayoutRect(leftExtent, topExtent + centerHeight, centerWidth, bottomExtent),
            [DockRegion.Center] = new DockLayoutRect(leftExtent, topExtent, centerWidth, centerHeight),
        };
    }
}
