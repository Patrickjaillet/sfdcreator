using System.Numerics;
using Silk.NET.Input;
using SFDCreator.Win32.Interop;

namespace SFDCreator.Win32.Input;

public sealed class Win32Mouse : IMouse
{
    private static readonly MouseButton[] AllButtons = Enum.GetValues<MouseButton>().Where(b => b != MouseButton.Unknown).ToArray();
    private static readonly ScrollWheel[] SingleWheel = { new(0, 0) };

    private readonly HashSet<MouseButton> _pressedButtons = new();
    private readonly nint _hwnd;
    private Vector2 _position;
    private int _doubleClickTime;
    private int _doubleClickRange;

    public Win32Mouse(nint hwnd)
    {
        _hwnd = hwnd;
        _doubleClickTime = (int)User32.GetDoubleClickTime();
        _doubleClickRange = User32.GetSystemMetrics(NativeConstants.SM_CXDOUBLECLK);
        Cursor = new Win32Cursor();
    }

    public string Name => "Win32 Mouse";

    public int Index => 0;

    public bool IsConnected => true;

    public IReadOnlyList<MouseButton> SupportedButtons => AllButtons;

    public IReadOnlyList<ScrollWheel> ScrollWheels => SingleWheel;

    public Vector2 Position
    {
        get => _position;
        set
        {
            _position = value;
            var point = new POINT { X = (int)value.X, Y = (int)value.Y };
            User32.ClientToScreen(_hwnd, ref point);
            User32.SetCursorPos(point.X, point.Y);
        }
    }

    public ICursor Cursor { get; }

    public int DoubleClickTime
    {
        get => _doubleClickTime;
        set
        {
            _doubleClickTime = value;
            User32.SetDoubleClickTime((uint)value);
        }
    }

    public int DoubleClickRange
    {
        get => _doubleClickRange;
        set => _doubleClickRange = value;
    }

    public event Action<IMouse, MouseButton>? MouseDown;

    public event Action<IMouse, MouseButton>? MouseUp;

    public event Action<IMouse, MouseButton, Vector2>? Click;

    public event Action<IMouse, MouseButton, Vector2>? DoubleClick;

    public event Action<IMouse, Vector2>? MouseMove;

    public event Action<IMouse, ScrollWheel>? Scroll;

    public bool IsButtonPressed(MouseButton button) => _pressedButtons.Contains(button);

    internal void HandleMessage(uint msg, nint wParam, nint lParam)
    {
        switch (msg)
        {
            case NativeConstants.WM_MOUSEMOVE:
                _position = DecodePosition(lParam);
                MouseMove?.Invoke(this, _position);
                break;

            case NativeConstants.WM_LBUTTONDOWN:
                RaiseDown(MouseButton.Left);
                break;
            case NativeConstants.WM_LBUTTONUP:
                RaiseUp(MouseButton.Left);
                break;
            case NativeConstants.WM_LBUTTONDBLCLK:
                RaiseDoubleClick(MouseButton.Left);
                break;

            case NativeConstants.WM_RBUTTONDOWN:
                RaiseDown(MouseButton.Right);
                break;
            case NativeConstants.WM_RBUTTONUP:
                RaiseUp(MouseButton.Right);
                break;
            case NativeConstants.WM_RBUTTONDBLCLK:
                RaiseDoubleClick(MouseButton.Right);
                break;

            case NativeConstants.WM_MBUTTONDOWN:
                RaiseDown(MouseButton.Middle);
                break;
            case NativeConstants.WM_MBUTTONUP:
                RaiseUp(MouseButton.Middle);
                break;
            case NativeConstants.WM_MBUTTONDBLCLK:
                RaiseDoubleClick(MouseButton.Middle);
                break;

            case NativeConstants.WM_XBUTTONDOWN:
                RaiseDown(DecodeXButton(wParam));
                break;
            case NativeConstants.WM_XBUTTONUP:
                RaiseUp(DecodeXButton(wParam));
                break;
            case NativeConstants.WM_XBUTTONDBLCLK:
                RaiseDoubleClick(DecodeXButton(wParam));
                break;

            case NativeConstants.WM_MOUSEWHEEL:
            {
                var delta = (short)((wParam.ToInt64() >> 16) & 0xFFFF) / (float)NativeConstants.WHEEL_DELTA;
                Scroll?.Invoke(this, new ScrollWheel(0, delta));
                break;
            }

            case NativeConstants.WM_MOUSEHWHEEL:
            {
                var delta = (short)((wParam.ToInt64() >> 16) & 0xFFFF) / (float)NativeConstants.WHEEL_DELTA;
                Scroll?.Invoke(this, new ScrollWheel(delta, 0));
                break;
            }
        }
    }

    private void RaiseDown(MouseButton button)
    {
        _pressedButtons.Add(button);
        MouseDown?.Invoke(this, button);
    }

    private void RaiseUp(MouseButton button)
    {
        _pressedButtons.Remove(button);
        MouseUp?.Invoke(this, button);
        Click?.Invoke(this, button, _position);
    }

    private void RaiseDoubleClick(MouseButton button)
    {
        _pressedButtons.Add(button);
        MouseDown?.Invoke(this, button);
        DoubleClick?.Invoke(this, button, _position);
    }

    private static MouseButton DecodeXButton(nint wParam)
    {
        var xButton = (int)((wParam.ToInt64() >> 16) & 0xFFFF);
        return xButton == NativeConstants.XBUTTON2 ? MouseButton.Button5 : MouseButton.Button4;
    }

    private static Vector2 DecodePosition(nint lParam)
    {
        var bits = lParam.ToInt64();
        var x = (short)(bits & 0xFFFF);
        var y = (short)((bits >> 16) & 0xFFFF);
        return new Vector2(x, y);
    }
}
