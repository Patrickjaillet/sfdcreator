using SFDCreator.Win32.Interop;

namespace SFDCreator.Win32.Dialogs;

public static class NativeMessageBox
{
    public static void Show(nint ownerHwnd, string text, string caption) => User32.MessageBoxW(ownerHwnd, text, caption, 0);
}
