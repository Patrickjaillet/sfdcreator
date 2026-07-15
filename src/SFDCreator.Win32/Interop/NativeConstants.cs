namespace SFDCreator.Win32.Interop;

internal static class NativeConstants
{
    public const int CS_HREDRAW = 0x0002;
    public const int CS_VREDRAW = 0x0001;
    public const int CS_DBLCLKS = 0x0008;

    public const uint WS_OVERLAPPEDWINDOW = 0x00CF0000;
    public const uint WS_CHILD = 0x40000000;
    public const uint WS_VISIBLE = 0x10000000;
    public const uint WS_CLIPCHILDREN = 0x02000000;

    public const int SW_SHOW = 5;
    public const int SW_SHOWMAXIMIZED = 3;

    public const int CW_USEDEFAULT = unchecked((int)0x80000000);

    public const uint WM_DESTROY = 0x0002;
    public const uint WM_SIZE = 0x0005;
    public const uint WM_CLOSE = 0x0010;
    public const uint WM_PAINT = 0x000F;
    public const uint WM_COMMAND = 0x0111;
    public const uint WM_DROPFILES = 0x0233;
    public const uint WM_DPICHANGED = 0x02E0;
    public const uint WM_GETMINMAXINFO = 0x0024;

    public const uint WM_KEYDOWN = 0x0100;
    public const uint WM_KEYUP = 0x0101;
    public const uint WM_CHAR = 0x0102;
    public const uint WM_SYSKEYDOWN = 0x0104;
    public const uint WM_SYSKEYUP = 0x0105;

    public const uint WM_MOUSEMOVE = 0x0200;
    public const uint WM_LBUTTONDOWN = 0x0201;
    public const uint WM_LBUTTONUP = 0x0202;
    public const uint WM_RBUTTONDOWN = 0x0204;
    public const uint WM_RBUTTONUP = 0x0205;
    public const uint WM_MBUTTONDOWN = 0x0207;
    public const uint WM_MBUTTONUP = 0x0208;
    public const uint WM_MOUSEWHEEL = 0x020A;
    public const uint WM_XBUTTONDOWN = 0x020B;
    public const uint WM_XBUTTONUP = 0x020C;
    public const uint WM_XBUTTONDBLCLK = 0x020D;
    public const uint WM_MOUSEHWHEEL = 0x020E;
    public const uint WM_LBUTTONDBLCLK = 0x0203;
    public const uint WM_RBUTTONDBLCLK = 0x0206;
    public const uint WM_MBUTTONDBLCLK = 0x0209;

    public const int GWLP_USERDATA = -21;

    public const uint MF_STRING = 0x00000000;
    public const uint MF_POPUP = 0x00000010;

    public const uint MONITORINFOF_PRIMARY = 0x00000001;

    public const int WHEEL_DELTA = 120;

    public const int XBUTTON1 = 0x0001;
    public const int XBUTTON2 = 0x0002;

    public const uint OFN_FILEMUSTEXIST = 0x00001000;
    public const uint OFN_PATHMUSTEXIST = 0x00000800;
    public const uint OFN_OVERWRITEPROMPT = 0x00000002;
    public const uint OFN_EXPLORER = 0x00080000;

    public const int IDC_ARROW = 32512;
    public const int IDC_IBEAM = 32513;
    public const int IDC_CROSS = 32515;
    public const int IDC_SIZEALL = 32646;
    public const int IDC_SIZENWSE = 32642;
    public const int IDC_SIZENESW = 32643;
    public const int IDC_SIZEWE = 32644;
    public const int IDC_SIZENS = 32645;
    public const int IDC_NO = 32648;
    public const int IDC_HAND = 32649;
    public const int IDC_WAIT = 32514;
    public const int IDC_APPSTARTING = 32650;

    public const uint CF_UNICODETEXT = 13;

    public const int SM_CXDOUBLECLK = 36;
    public const int SM_CYDOUBLECLK = 37;

    public const uint SWP_NOZORDER = 0x0004;
    public const uint SWP_NOACTIVATE = 0x0010;

    public const int KEY_EXTENDED_FLAG = 0x01000000;

    public const uint PM_REMOVE = 0x0001;
    public const uint WM_QUIT = 0x0012;

    public const nint COLOR_WINDOW_BRUSH = 6;
}
