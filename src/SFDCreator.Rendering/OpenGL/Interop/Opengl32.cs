using System.Runtime.InteropServices;

namespace SFDCreator.Rendering.OpenGL.Interop;

internal static class Opengl32
{
    [DllImport("opengl32.dll")]
    public static extern nint wglCreateContext(nint hdc);

    [DllImport("opengl32.dll")]
    public static extern bool wglDeleteContext(nint hglrc);

    [DllImport("opengl32.dll")]
    public static extern bool wglMakeCurrent(nint hdc, nint hglrc);

    [DllImport("opengl32.dll")]
    public static extern nint wglGetProcAddress(string procName);
}

[UnmanagedFunctionPointer(CallingConvention.StdCall)]
internal delegate nint WglCreateContextAttribsArb(nint hdc, nint hShareContext, int[] attribList);
