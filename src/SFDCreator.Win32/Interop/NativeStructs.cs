using System.Runtime.InteropServices;

namespace SFDCreator.Win32.Interop;

internal delegate nint WndProc(nint hWnd, uint msg, nint wParam, nint lParam);

[StructLayout(LayoutKind.Sequential)]
internal struct POINT
{
    public int X;
    public int Y;
}

[StructLayout(LayoutKind.Sequential)]
internal struct RECT
{
    public int Left;
    public int Top;
    public int Right;
    public int Bottom;

    public readonly int Width => Right - Left;
    public readonly int Height => Bottom - Top;
}

[StructLayout(LayoutKind.Sequential)]
internal struct MSG
{
    public nint hwnd;
    public uint message;
    public nint wParam;
    public nint lParam;
    public uint time;
    public POINT pt;
}

[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
internal struct WNDCLASSEXW
{
    public uint cbSize;
    public uint style;
    public nint lpfnWndProc;
    public int cbClsExtra;
    public int cbWndExtra;
    public nint hInstance;
    public nint hIcon;
    public nint hCursor;
    public nint hbrBackground;
    public string? lpszMenuName;
    public string? lpszClassName;
    public nint hIconSm;
}

[StructLayout(LayoutKind.Sequential)]
internal struct WINDOWPOS
{
    public nint hwnd;
    public nint hwndInsertAfter;
    public int x;
    public int y;
    public int cx;
    public int cy;
    public uint flags;
}

[StructLayout(LayoutKind.Sequential)]
internal struct MINMAXINFO
{
    public POINT ptReserved;
    public POINT ptMaxSize;
    public POINT ptMaxPosition;
    public POINT ptMinTrackSize;
    public POINT ptMaxTrackSize;
}

[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
internal struct MONITORINFOEXW
{
    public uint cbSize;
    public RECT rcMonitor;
    public RECT rcWork;
    public uint dwFlags;

    [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
    public string szDevice;
}

[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
internal struct OPENFILENAMEW
{
    public int lStructSize;
    public nint hwndOwner;
    public nint hInstance;
    public string? lpstrFilter;
    public string? lpstrCustomFilter;
    public int nMaxCustFilter;
    public int nFilterIndex;
    public nint lpstrFile;
    public int nMaxFile;
    public string? lpstrFileTitle;
    public int nMaxFileTitle;
    public string? lpstrInitialDir;
    public string? lpstrTitle;
    public uint Flags;
    public short nFileOffset;
    public short nFileExtension;
    public string? lpstrDefExt;
    public nint lCustData;
    public nint lpfnHook;
    public string? lpTemplateName;
    public nint pvReserved;
    public int dwReserved;
    public uint FlagsEx;
}
