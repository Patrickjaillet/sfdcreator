using System.Runtime.InteropServices;
using System.Text;
using Silk.NET.Input;
using SFDCreator.Win32.Input;
using SFDCreator.Win32.Interop;

namespace SFDCreator.Win32;

public sealed class Win32Window : IDisposable
{
    private const string ClassName = "SFDCreatorWindowClass";

    private static readonly WndProc WndProcDelegate = StaticWndProc;
    private static readonly Dictionary<nint, Win32Window> Instances = new();
    private static readonly nint ModuleHandle = Kernel32.GetModuleHandleW(null);
    private static bool _classRegistered;
    private static Win32Window? _pendingInstance;

    private nint _hwnd;
    private Win32InputContext? _input;
    private int _clientWidth;
    private int _clientHeight;

    public Win32Window(Win32WindowOptions options)
    {
        EnsureClassRegistered();

        _pendingInstance = this;

        const uint style = NativeConstants.WS_OVERLAPPEDWINDOW | NativeConstants.WS_CLIPCHILDREN;
        var x = options.X ?? NativeConstants.CW_USEDEFAULT;
        var y = options.Y ?? NativeConstants.CW_USEDEFAULT;

        _hwnd = User32.CreateWindowExW(
            0,
            ClassName,
            options.Title,
            style,
            x,
            y,
            options.Width,
            options.Height,
            0,
            0,
            ModuleHandle,
            0);

        if (_hwnd == 0)
        {
            throw new InvalidOperationException("Failed to create the native window.");
        }

        _input = new Win32InputContext(_hwnd);

        if (options.MenuBuilder is not null)
        {
            User32.SetMenu(_hwnd, options.MenuBuilder.Build());
        }

        Shell32.DragAcceptFiles(_hwnd, true);

        User32.ShowWindow(_hwnd, options.Maximized ? NativeConstants.SW_SHOWMAXIMIZED : NativeConstants.SW_SHOW);
        User32.UpdateWindow(_hwnd);
    }

    public event Action<int, int>? Resized;

    public event Action? Closing;

    public event Action? Closed;

    public event Action<uint>? DpiChanged;

    public event Action<IReadOnlyList<string>>? FilesDropped;

    public event Action<int>? MenuCommand;

    public nint Handle => _hwnd;

    public (int Width, int Height) ClientSize => (_clientWidth, _clientHeight);

    public IInputContext Input => _input ?? throw new InvalidOperationException("The window has not been created yet.");

    public uint Dpi => User32.GetDpiForWindow(_hwnd);

    public (int X, int Y, int Width, int Height) GetWindowBounds()
    {
        User32.GetWindowRect(_hwnd, out var rect);
        return (rect.Left, rect.Top, rect.Width, rect.Height);
    }

    public void Run()
    {
        while (User32.GetMessageW(out var msg, 0, 0, 0))
        {
            User32.TranslateMessage(ref msg);
            User32.DispatchMessageW(ref msg);
        }
    }

    public void RunWithIdle(Action onIdle)
    {
        while (true)
        {
            while (User32.PeekMessageW(out var msg, 0, 0, 0, NativeConstants.PM_REMOVE))
            {
                if (msg.message == NativeConstants.WM_QUIT)
                {
                    return;
                }

                User32.TranslateMessage(ref msg);
                User32.DispatchMessageW(ref msg);
            }

            onIdle();
        }
    }

    public void Close() => User32.PostMessageW(_hwnd, NativeConstants.WM_CLOSE, 0, 0);

    public void Dispose()
    {
        if (_hwnd != 0)
        {
            User32.DestroyWindow(_hwnd);
        }
    }

    private static void EnsureClassRegistered()
    {
        if (_classRegistered)
        {
            return;
        }

        var wndClass = new WNDCLASSEXW
        {
            cbSize = (uint)Marshal.SizeOf<WNDCLASSEXW>(),
            style = NativeConstants.CS_HREDRAW | NativeConstants.CS_VREDRAW | NativeConstants.CS_DBLCLKS,
            lpfnWndProc = Marshal.GetFunctionPointerForDelegate(WndProcDelegate),
            hInstance = ModuleHandle,
            hCursor = User32.LoadCursorW(0, NativeConstants.IDC_ARROW),
            hbrBackground = NativeConstants.COLOR_WINDOW_BRUSH,
            lpszClassName = ClassName,
        };

        if (User32.RegisterClassExW(ref wndClass) == 0)
        {
            throw new InvalidOperationException("Failed to register the native window class.");
        }

        _classRegistered = true;
    }

    private static nint StaticWndProc(nint hWnd, uint msg, nint wParam, nint lParam)
    {
        if (!Instances.TryGetValue(hWnd, out var instance))
        {
            if (_pendingInstance is null)
            {
                return User32.DefWindowProcW(hWnd, msg, wParam, lParam);
            }

            instance = _pendingInstance;
            instance._hwnd = hWnd;
            Instances[hWnd] = instance;
            _pendingInstance = null;
        }

        return instance.WndProc(msg, wParam, lParam);
    }

    private nint WndProc(uint msg, nint wParam, nint lParam)
    {
        switch (msg)
        {
            case NativeConstants.WM_SIZE:
            {
                _clientWidth = (int)(lParam.ToInt64() & 0xFFFF);
                _clientHeight = (int)((lParam.ToInt64() >> 16) & 0xFFFF);
                Resized?.Invoke(_clientWidth, _clientHeight);
                return 0;
            }

            case NativeConstants.WM_CLOSE:
                Closing?.Invoke();
                User32.DestroyWindow(_hwnd);
                return 0;

            case NativeConstants.WM_DESTROY:
                Closed?.Invoke();
                Instances.Remove(_hwnd);
                User32.PostQuitMessage(0);
                return 0;

            case NativeConstants.WM_COMMAND:
                MenuCommand?.Invoke((int)(wParam.ToInt64() & 0xFFFF));
                return 0;

            case NativeConstants.WM_DROPFILES:
                HandleDropFiles(wParam);
                return 0;

            case NativeConstants.WM_DPICHANGED:
            {
                var newDpi = (uint)(wParam.ToInt64() & 0xFFFF);
                var suggested = Marshal.PtrToStructure<RECT>(lParam);
                User32.SetWindowPos(
                    _hwnd,
                    0,
                    suggested.Left,
                    suggested.Top,
                    suggested.Width,
                    suggested.Height,
                    NativeConstants.SWP_NOZORDER | NativeConstants.SWP_NOACTIVATE);
                DpiChanged?.Invoke(newDpi);
                return 0;
            }

            case NativeConstants.WM_PAINT:
            {
                User32.BeginPaint(_hwnd, out var paintStruct);
                User32.EndPaint(_hwnd, ref paintStruct);
                return 0;
            }

            case NativeConstants.WM_KEYDOWN:
            case NativeConstants.WM_KEYUP:
            case NativeConstants.WM_CHAR:
                _input?.HandleKeyboardMessage(msg, wParam, lParam);
                return 0;

            case NativeConstants.WM_SYSKEYDOWN:
            case NativeConstants.WM_SYSKEYUP:
                _input?.HandleKeyboardMessage(msg, wParam, lParam);
                return User32.DefWindowProcW(_hwnd, msg, wParam, lParam);

            case NativeConstants.WM_MOUSEMOVE:
            case NativeConstants.WM_LBUTTONDOWN:
            case NativeConstants.WM_LBUTTONUP:
            case NativeConstants.WM_LBUTTONDBLCLK:
            case NativeConstants.WM_RBUTTONDOWN:
            case NativeConstants.WM_RBUTTONUP:
            case NativeConstants.WM_RBUTTONDBLCLK:
            case NativeConstants.WM_MBUTTONDOWN:
            case NativeConstants.WM_MBUTTONUP:
            case NativeConstants.WM_MBUTTONDBLCLK:
            case NativeConstants.WM_XBUTTONDOWN:
            case NativeConstants.WM_XBUTTONUP:
            case NativeConstants.WM_XBUTTONDBLCLK:
            case NativeConstants.WM_MOUSEWHEEL:
            case NativeConstants.WM_MOUSEHWHEEL:
                _input?.HandleMouseMessage(msg, wParam, lParam);
                return 0;

            default:
                return User32.DefWindowProcW(_hwnd, msg, wParam, lParam);
        }
    }

    private void HandleDropFiles(nint hDrop)
    {
        var count = Shell32.DragQueryFileW(hDrop, 0xFFFFFFFF, null, 0);
        var files = new List<string>((int)count);

        for (uint i = 0; i < count; i++)
        {
            var length = Shell32.DragQueryFileW(hDrop, i, null, 0);
            var buffer = new StringBuilder((int)length + 1);
            Shell32.DragQueryFileW(hDrop, i, buffer, (uint)buffer.Capacity);
            files.Add(buffer.ToString());
        }

        Shell32.DragFinish(hDrop);
        FilesDropped?.Invoke(files);
    }
}
