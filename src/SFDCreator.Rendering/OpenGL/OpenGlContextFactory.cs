using System.Runtime.InteropServices;
using SFDCreator.Rendering.Backend;
using SFDCreator.Rendering.OpenGL.Interop;
using Silk.NET.OpenGL;

namespace SFDCreator.Rendering.OpenGL;

public static class OpenGlContextFactory
{
    private const string DummyClassName = "SFDCreatorGlBootstrapWindow";
    private static readonly WndProc DummyWndProc = (hWnd, msg, wParam, lParam) => User32Minimal.DefWindowProcW(hWnd, msg, wParam, lParam);
    private static bool _dummyClassRegistered;

    public static OpenGlDevice Create(nint targetHwnd, GraphicsDeviceOptions options)
    {
        var (createContextAttribsArb, glModule) = BootstrapArbFunctions();

        var targetDc = User32Minimal.GetDC(targetHwnd);
        if (targetDc == 0)
        {
            throw new InvalidOperationException("Failed to get a device context for the target window.");
        }

        SetPixelFormat(targetDc);

        var context = CreateModernContext(createContextAttribsArb, targetDc, options);

        if (!Opengl32.wglMakeCurrent(targetDc, context))
        {
            throw new InvalidOperationException("wglMakeCurrent failed for the real rendering context.");
        }

        var gl = GL.GetApi(name => ResolveProcAddress(glModule, name));

        return new OpenGlDevice(gl, targetHwnd, targetDc, context);
    }

    private static (WglCreateContextAttribsArb CreateContextAttribsArb, nint GlModule) BootstrapArbFunctions()
    {
        EnsureDummyClassRegistered();

        var dummyHwnd = User32Minimal.CreateWindowExW(
            0, DummyClassName, string.Empty, NativeConstants.WS_POPUP, 0, 0, 1, 1, 0, 0, 0, 0);

        if (dummyHwnd == 0)
        {
            throw new InvalidOperationException("Failed to create the temporary bootstrap window.");
        }

        try
        {
            var dummyDc = User32Minimal.GetDC(dummyHwnd);
            SetPixelFormat(dummyDc);

            var dummyContext = Opengl32.wglCreateContext(dummyDc);
            if (dummyContext == 0 || !Opengl32.wglMakeCurrent(dummyDc, dummyContext))
            {
                throw new InvalidOperationException("Failed to create the legacy bootstrap OpenGL context.");
            }

            var createContextAttribsPtr = Opengl32.wglGetProcAddress("wglCreateContextAttribsARB");
            if (createContextAttribsPtr == 0)
            {
                throw new InvalidOperationException("wglCreateContextAttribsARB is not available on this system.");
            }

            var createContextAttribsArb = Marshal.GetDelegateForFunctionPointer<WglCreateContextAttribsArb>(createContextAttribsPtr);

            Opengl32.wglMakeCurrent(0, 0);
            Opengl32.wglDeleteContext(dummyContext);

            var glModule = Kernel32Minimal.GetModuleHandleW("opengl32.dll");
            return (createContextAttribsArb, glModule);
        }
        finally
        {
            User32Minimal.DestroyWindow(dummyHwnd);
        }
    }

    private static void SetPixelFormat(nint dc)
    {
        var pfd = new PIXELFORMATDESCRIPTOR
        {
            nSize = (ushort)Marshal.SizeOf<PIXELFORMATDESCRIPTOR>(),
            nVersion = 1,
            dwFlags = NativeConstants.PFD_DRAW_TO_WINDOW | NativeConstants.PFD_SUPPORT_OPENGL | NativeConstants.PFD_DOUBLEBUFFER,
            iPixelType = NativeConstants.PFD_TYPE_RGBA,
            cColorBits = 32,
            cDepthBits = 24,
            cStencilBits = 8,
            iLayerType = NativeConstants.PFD_MAIN_PLANE,
        };

        var formatIndex = Gdi32.ChoosePixelFormat(dc, ref pfd);
        if (formatIndex == 0 || !Gdi32.SetPixelFormat(dc, formatIndex, ref pfd))
        {
            throw new InvalidOperationException("Failed to set a pixel format on the target device context.");
        }
    }

    private static nint CreateModernContext(WglCreateContextAttribsArb createContextAttribsArb, nint dc, GraphicsDeviceOptions options)
    {
        var context = createContextAttribsArb(dc, 0, BuildAttribs(options.MajorVersion, options.MinorVersion));

        if (context == 0)
        {
            context = createContextAttribsArb(dc, 0, BuildAttribs(3, 3));
        }

        if (context == 0)
        {
            throw new InvalidOperationException("Failed to create a modern OpenGL core-profile context.");
        }

        return context;
    }

    private static int[] BuildAttribs(int major, int minor) => new[]
    {
        NativeConstants.WGL_CONTEXT_MAJOR_VERSION_ARB, major,
        NativeConstants.WGL_CONTEXT_MINOR_VERSION_ARB, minor,
        NativeConstants.WGL_CONTEXT_PROFILE_MASK_ARB, NativeConstants.WGL_CONTEXT_CORE_PROFILE_BIT_ARB,
        0,
    };

    private static nint ResolveProcAddress(nint glModule, string name)
    {
        var address = Opengl32.wglGetProcAddress(name);

        if (address is not 0 and not 1 and not 2 and not 3 && address != unchecked((nint)(-1)))
        {
            return address;
        }

        return Kernel32Minimal.GetProcAddress(glModule, name);
    }

    private static void EnsureDummyClassRegistered()
    {
        if (_dummyClassRegistered)
        {
            return;
        }

        var wndClass = new WNDCLASSEXW
        {
            cbSize = (uint)Marshal.SizeOf<WNDCLASSEXW>(),
            style = NativeConstants.CS_HREDRAW | NativeConstants.CS_VREDRAW,
            lpfnWndProc = Marshal.GetFunctionPointerForDelegate(DummyWndProc),
            hInstance = Kernel32Minimal.GetModuleHandleW(null),
            lpszClassName = DummyClassName,
        };

        if (User32Minimal.RegisterClassExW(ref wndClass) == 0)
        {
            throw new InvalidOperationException("Failed to register the bootstrap window class.");
        }

        _dummyClassRegistered = true;
    }
}
