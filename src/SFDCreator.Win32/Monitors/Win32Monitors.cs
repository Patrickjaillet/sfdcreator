using SFDCreator.Win32.Interop;

namespace SFDCreator.Win32.Monitors;

public static class Win32Monitors
{
    public static IReadOnlyList<MonitorInfo> EnumerateAll()
    {
        var monitors = new List<MonitorInfo>();

        bool Callback(nint hMonitor, nint hdcMonitor, nint lprcMonitor, nint dwData)
        {
            var info = new MONITORINFOEXW { cbSize = (uint)System.Runtime.InteropServices.Marshal.SizeOf<MONITORINFOEXW>() };

            if (User32.GetMonitorInfoW(hMonitor, ref info))
            {
                monitors.Add(new MonitorInfo(
                    info.rcMonitor.Left,
                    info.rcMonitor.Top,
                    info.rcMonitor.Right,
                    info.rcMonitor.Bottom,
                    info.rcWork.Left,
                    info.rcWork.Top,
                    info.rcWork.Right,
                    info.rcWork.Bottom,
                    (info.dwFlags & NativeConstants.MONITORINFOF_PRIMARY) != 0));
            }

            return true;
        }

        User32.EnumDisplayMonitors(0, 0, Callback, 0);
        return monitors;
    }

    public static uint GetDpi(nint hwnd) => User32.GetDpiForWindow(hwnd);
}
