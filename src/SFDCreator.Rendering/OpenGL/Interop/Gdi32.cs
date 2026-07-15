using System.Runtime.InteropServices;

namespace SFDCreator.Rendering.OpenGL.Interop;

internal static class Gdi32
{
    [DllImport("gdi32.dll")]
    public static extern int ChoosePixelFormat(nint hdc, ref PIXELFORMATDESCRIPTOR ppfd);

    [DllImport("gdi32.dll")]
    public static extern bool SetPixelFormat(nint hdc, int format, ref PIXELFORMATDESCRIPTOR ppfd);

    [DllImport("gdi32.dll")]
    public static extern bool SwapBuffers(nint hdc);
}
