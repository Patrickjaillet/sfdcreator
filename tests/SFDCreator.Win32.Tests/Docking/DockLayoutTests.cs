using SFDCreator.Win32.Docking;

namespace SFDCreator.Win32.Tests.Docking;

public sealed class DockLayoutTests
{
    [Fact]
    public void Compute_PartitionsClientAreaIntoFourNonOverlappingRegions()
    {
        var rects = DockLayout.Compute(clientWidth: 1000, clientHeight: 800, leftExtent: 200, rightExtent: 150, bottomExtent: 100);

        Assert.Equal(new DockLayoutRect(0, 0, 200, 800), rects[DockRegion.Left]);
        Assert.Equal(new DockLayoutRect(850, 0, 150, 800), rects[DockRegion.Right]);
        Assert.Equal(new DockLayoutRect(200, 700, 650, 100), rects[DockRegion.Bottom]);
        Assert.Equal(new DockLayoutRect(200, 0, 650, 700), rects[DockRegion.Center]);
    }

    [Fact]
    public void Compute_ClampsExtentsThatWouldExceedTheClientArea()
    {
        var rects = DockLayout.Compute(clientWidth: 100, clientHeight: 100, leftExtent: 90, rightExtent: 90, bottomExtent: 200);

        Assert.Equal(90, rects[DockRegion.Left].Width);
        Assert.Equal(10, rects[DockRegion.Right].Width);
        Assert.Equal(0, rects[DockRegion.Center].Width);
        Assert.Equal(100, rects[DockRegion.Bottom].Height);
        Assert.Equal(0, rects[DockRegion.Center].Height);
    }

    [Fact]
    public void Compute_WithZeroExtents_CenterFillsTheEntireClientArea()
    {
        var rects = DockLayout.Compute(clientWidth: 640, clientHeight: 480, leftExtent: 0, rightExtent: 0, bottomExtent: 0);

        Assert.Equal(new DockLayoutRect(0, 0, 640, 480), rects[DockRegion.Center]);
    }
}
