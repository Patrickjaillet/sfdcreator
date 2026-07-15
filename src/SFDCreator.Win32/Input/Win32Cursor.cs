using Silk.NET.Core;
using Silk.NET.Input;
using SFDCreator.Win32.Interop;

namespace SFDCreator.Win32.Input;

public sealed class Win32Cursor : ICursor
{
    private StandardCursor _standardCursor = StandardCursor.Default;
    private CursorMode _cursorMode = CursorMode.Normal;

    public CursorType Type { get; set; } = CursorType.Standard;

    public StandardCursor StandardCursor
    {
        get => _standardCursor;
        set
        {
            _standardCursor = value;
            User32.SetCursor(User32.LoadCursorW(0, ToResourceId(value)));
        }
    }

    public CursorMode CursorMode
    {
        get => _cursorMode;
        set
        {
            if (_cursorMode == CursorMode.Hidden && value != CursorMode.Hidden)
            {
                User32.ShowCursor(true);
            }
            else if (_cursorMode != CursorMode.Hidden && value == CursorMode.Hidden)
            {
                User32.ShowCursor(false);
            }

            _cursorMode = value;
        }
    }

    public bool IsConfined { get; set; }

    public int HotspotX { get; set; }

    public int HotspotY { get; set; }

    public RawImage Image { get; set; }

    public bool IsSupported(CursorMode mode) => mode is CursorMode.Normal or CursorMode.Hidden;

    public bool IsSupported(StandardCursor standardCursor) => true;

    private static nint ToResourceId(StandardCursor cursor) => cursor switch
    {
        StandardCursor.IBeam => NativeConstants.IDC_IBEAM,
        StandardCursor.Crosshair => NativeConstants.IDC_CROSS,
        StandardCursor.Hand => NativeConstants.IDC_HAND,
        StandardCursor.HResize => NativeConstants.IDC_SIZEWE,
        StandardCursor.VResize => NativeConstants.IDC_SIZENS,
        StandardCursor.NwseResize => NativeConstants.IDC_SIZENWSE,
        StandardCursor.NeswResize => NativeConstants.IDC_SIZENESW,
        StandardCursor.ResizeAll => NativeConstants.IDC_SIZEALL,
        StandardCursor.NotAllowed => NativeConstants.IDC_NO,
        StandardCursor.Wait => NativeConstants.IDC_WAIT,
        StandardCursor.WaitArrow => NativeConstants.IDC_APPSTARTING,
        _ => NativeConstants.IDC_ARROW,
    };
}
