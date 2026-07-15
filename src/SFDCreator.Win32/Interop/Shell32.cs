using System.Runtime.InteropServices;
using System.Text;

namespace SFDCreator.Win32.Interop;

internal static class Shell32
{
    [DllImport("shell32.dll")]
    public static extern void DragAcceptFiles(nint hWnd, bool fAccept);

    [DllImport("shell32.dll", CharSet = CharSet.Unicode)]
    public static extern uint DragQueryFileW(nint hDrop, uint iFile, StringBuilder? lpszFile, uint cch);

    [DllImport("shell32.dll")]
    public static extern bool DragQueryPoint(nint hDrop, out POINT ppt);

    [DllImport("shell32.dll")]
    public static extern void DragFinish(nint hDrop);
}
