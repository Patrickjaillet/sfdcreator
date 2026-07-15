using System.Runtime.InteropServices;

namespace SFDCreator.Win32.Interop;

internal static class ComDlg32
{
    [DllImport("comdlg32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
    public static extern bool GetOpenFileNameW(ref OPENFILENAMEW lpofn);

    [DllImport("comdlg32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
    public static extern bool GetSaveFileNameW(ref OPENFILENAMEW lpofn);

    [DllImport("comdlg32.dll")]
    public static extern uint CommDlgExtendedError();
}
