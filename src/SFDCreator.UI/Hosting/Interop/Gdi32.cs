using System.Runtime.InteropServices;

namespace SFDCreator.UI.Hosting.Interop;

[StructLayout(LayoutKind.Sequential)]
internal struct BITMAPINFOHEADER
{
    public uint biSize;
    public int biWidth;
    public int biHeight;
    public ushort biPlanes;
    public ushort biBitCount;
    public uint biCompression;
    public uint biSizeImage;
    public int biXPelsPerMeter;
    public int biYPelsPerMeter;
    public uint biClrUsed;
    public uint biClrImportant;
}

internal static class Gdi32
{
    public const uint DIB_RGB_COLORS = 0;
    public const uint BI_RGB = 0;

    [DllImport("gdi32.dll")]
    public static extern int SetDIBitsToDevice(
        nint hdc,
        int xDest,
        int yDest,
        uint dwWidth,
        uint dwHeight,
        int xSrc,
        int ySrc,
        uint startScan,
        uint linesToDraw,
        nint bits,
        ref BITMAPINFOHEADER bitsInfo,
        uint colorUse);
}
