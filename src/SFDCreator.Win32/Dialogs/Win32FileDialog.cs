using System.Runtime.InteropServices;
using SFDCreator.Win32.Interop;

namespace SFDCreator.Win32.Dialogs;

public static class Win32FileDialog
{
    public static string? OpenFile(nint ownerHwnd, string title, string filter, string? initialDirectory = null)
        => Show(ownerHwnd, title, filter, initialDirectory, save: false);

    public static string? SaveFile(nint ownerHwnd, string title, string filter, string? initialDirectory = null)
        => Show(ownerHwnd, title, filter, initialDirectory, save: true);

    private static string? Show(nint ownerHwnd, string title, string filter, string? initialDirectory, bool save)
    {
        const int maxPath = 32768;
        var fileBuffer = Marshal.AllocHGlobal(maxPath * sizeof(char));

        try
        {
            Marshal.WriteInt16(fileBuffer, 0, 0);

            var ofn = new OPENFILENAMEW
            {
                lStructSize = Marshal.SizeOf<OPENFILENAMEW>(),
                hwndOwner = ownerHwnd,
                lpstrFilter = NormalizeFilter(filter),
                lpstrFile = fileBuffer,
                nMaxFile = maxPath,
                lpstrInitialDir = initialDirectory,
                lpstrTitle = title,
                Flags = save
                    ? NativeConstants.OFN_OVERWRITEPROMPT | NativeConstants.OFN_EXPLORER
                    : NativeConstants.OFN_FILEMUSTEXIST | NativeConstants.OFN_PATHMUSTEXIST | NativeConstants.OFN_EXPLORER,
            };

            var succeeded = save ? ComDlg32.GetSaveFileNameW(ref ofn) : ComDlg32.GetOpenFileNameW(ref ofn);

            return succeeded ? Marshal.PtrToStringUni(fileBuffer) : null;
        }
        finally
        {
            Marshal.FreeHGlobal(fileBuffer);
        }
    }

    private static string NormalizeFilter(string filter) => filter.Replace('|', '\0') + "\0\0";
}
