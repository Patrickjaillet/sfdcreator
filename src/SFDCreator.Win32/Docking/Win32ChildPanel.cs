using System.Runtime.InteropServices;
using SFDCreator.Win32.Input;
using SFDCreator.Win32.Interop;
using Silk.NET.Input;

namespace SFDCreator.Win32.Docking;

public sealed class Win32ChildPanel
{
    private const string ClassName = "SFDCreatorPanelClass";

    private static readonly WndProc WndProcDelegate = StaticWndProc;
    private static readonly Dictionary<nint, Win32ChildPanel> Instances = new();
    private static readonly nint ModuleHandle = Kernel32.GetModuleHandleW(null);
    private static bool _classRegistered;
    private static Win32ChildPanel? _pendingInstance;

    private nint _hwnd;
    private int _clientWidth;
    private int _clientHeight;

    public Win32ChildPanel(nint parentHwnd)
    {
        EnsureClassRegistered();

        _pendingInstance = this;

        _hwnd = User32.CreateWindowExW(
            0,
            ClassName,
            string.Empty,
            NativeConstants.WS_CHILD | NativeConstants.WS_VISIBLE,
            0, 0, 0, 0,
            parentHwnd,
            0,
            0,
            0);

        if (_hwnd == 0)
        {
            throw new InvalidOperationException("Failed to create the native child panel.");
        }
    }

    public event Action<nint>? Paint;

    public event Action<int, int>? Resized;

    public event Action<int, int>? MouseMove;

    public event Action<MouseButton, int, int>? MouseDown;

    public event Action<MouseButton, int, int>? MouseUp;

    public event Action<float, int, int>? MouseWheel;

    public event Action<Key>? KeyDown;

    public event Action<Key>? KeyUp;

    public nint Handle => _hwnd;

    public (int Width, int Height) ClientSize => (_clientWidth, _clientHeight);

    public void Invalidate() => User32.InvalidateRect(_hwnd, 0, false);

    private static void EnsureClassRegistered()
    {
        if (_classRegistered)
        {
            return;
        }

        var wndClass = new WNDCLASSEXW
        {
            cbSize = (uint)Marshal.SizeOf<WNDCLASSEXW>(),
            style = NativeConstants.CS_HREDRAW | NativeConstants.CS_VREDRAW,
            lpfnWndProc = Marshal.GetFunctionPointerForDelegate(WndProcDelegate),
            hInstance = ModuleHandle,
            hCursor = User32.LoadCursorW(0, NativeConstants.IDC_ARROW),
            hbrBackground = NativeConstants.COLOR_WINDOW_BRUSH,
            lpszClassName = ClassName,
        };

        if (User32.RegisterClassExW(ref wndClass) == 0)
        {
            throw new InvalidOperationException("Failed to register the native child panel class.");
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

            case NativeConstants.WM_ERASEBKGND:
                return 1;

            case NativeConstants.WM_PAINT:
            {
                var hdc = User32.BeginPaint(_hwnd, out var paintStruct);
                Paint?.Invoke(hdc);
                User32.EndPaint(_hwnd, ref paintStruct);
                return 0;
            }

            case NativeConstants.WM_MOUSEMOVE:
            {
                var (x, y) = DecodePosition(lParam);
                MouseMove?.Invoke(x, y);
                return 0;
            }

            case NativeConstants.WM_LBUTTONDOWN:
                RaiseMouseDown(MouseButton.Left, lParam);
                return 0;
            case NativeConstants.WM_LBUTTONUP:
                RaiseMouseUp(MouseButton.Left, lParam);
                return 0;

            case NativeConstants.WM_RBUTTONDOWN:
                RaiseMouseDown(MouseButton.Right, lParam);
                return 0;
            case NativeConstants.WM_RBUTTONUP:
                RaiseMouseUp(MouseButton.Right, lParam);
                return 0;

            case NativeConstants.WM_MBUTTONDOWN:
                RaiseMouseDown(MouseButton.Middle, lParam);
                return 0;
            case NativeConstants.WM_MBUTTONUP:
                RaiseMouseUp(MouseButton.Middle, lParam);
                return 0;

            case NativeConstants.WM_MOUSEWHEEL:
            {
                var (x, y) = DecodePosition(lParam);
                var delta = (short)((wParam.ToInt64() >> 16) & 0xFFFF) / (float)NativeConstants.WHEEL_DELTA;
                MouseWheel?.Invoke(delta, x, y);
                return 0;
            }

            case NativeConstants.WM_KEYDOWN:
            case NativeConstants.WM_SYSKEYDOWN:
                KeyDown?.Invoke(DecodeKey(wParam, lParam));
                return 0;

            case NativeConstants.WM_KEYUP:
            case NativeConstants.WM_SYSKEYUP:
                KeyUp?.Invoke(DecodeKey(wParam, lParam));
                return 0;

            default:
                return User32.DefWindowProcW(_hwnd, msg, wParam, lParam);
        }
    }

    private void RaiseMouseDown(MouseButton button, nint lParam)
    {
        var (x, y) = DecodePosition(lParam);
        MouseDown?.Invoke(button, x, y);
    }

    private void RaiseMouseUp(MouseButton button, nint lParam)
    {
        var (x, y) = DecodePosition(lParam);
        MouseUp?.Invoke(button, x, y);
    }

    private static (int X, int Y) DecodePosition(nint lParam)
    {
        var bits = lParam.ToInt64();
        var x = (short)(bits & 0xFFFF);
        var y = (short)((bits >> 16) & 0xFFFF);
        return (x, y);
    }

    private static Key DecodeKey(nint wParam, nint lParam)
    {
        var vk = (int)(wParam.ToInt64() & 0xFF);
        var lParamBits = lParam.ToInt64();
        var scancode = (int)((lParamBits >> 16) & 0xFF);
        var extended = (lParamBits & NativeConstants.KEY_EXTENDED_FLAG) != 0;
        return VirtualKeyMap.Map(vk, scancode, extended);
    }
}
