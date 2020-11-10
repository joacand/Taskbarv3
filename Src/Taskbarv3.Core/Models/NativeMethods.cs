using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace Taskbarv3.Core.Models
{
    internal static class NativeMethods
    {
        [DllImport("user32.dll")]
        public static extern void keybd_event(byte vkCode, byte scanCode, int flags, IntPtr extraInfo);
        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        public static extern int GetWindowText(IntPtr hWnd, StringBuilder strText, int maxCount);
        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        public static extern int GetWindowTextLength(IntPtr hWnd);
        [DllImport("user32.dll")]
        public static extern bool EnumWindows(EnumWindowsProc enumProc, IntPtr lParam);
        [DllImport("user32.dll", SetLastError = true)]
        public static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint lpdwProcessId);
        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern IntPtr OpenProcess(ProcessAccessFlags dwDesiredAccess, [MarshalAs(UnmanagedType.Bool)] bool bInheritHandle, int dwProcessId);
        [DllImport("kernel32.dll")]
        public static extern bool CloseHandle(IntPtr handle);
        [DllImport("psapi.dll")]
        public static extern uint GetModuleBaseNameA(IntPtr hProcess, IntPtr hModule, StringBuilder lpBaseName, uint nSize);
        [DllImport("user32.dll")]
        public static extern bool EnumDisplayMonitors(IntPtr hdc, IntPtr lprcClip, MonitorEnumDelegate lpfnEnum, IntPtr dwData);
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern bool SystemParametersInfo(int uiAction, int uiParam, ref Rect pvParam, int fWinIni);
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern bool GetMonitorInfo(IntPtr hmon, ref MonitorInfo monitorinfo);

        public delegate bool MonitorEnumDelegate(IntPtr hMonitor, IntPtr hdcMonitor, ref Rect lprcMonitor, IntPtr dwData);
        public static string LastWin32Error() => Marshal.GetLastWin32Error().ToString();
        public delegate bool EnumWindowsProc(IntPtr hWnd, IntPtr lParam);

        public static List<DisplayInfo> QueryDisplays()
        {
            var displays = new List<DisplayInfo>();

            EnumDisplayMonitors(IntPtr.Zero, IntPtr.Zero,
                delegate (IntPtr hMonitor, IntPtr hdcMonitor, ref Rect lprcMonitor, IntPtr dwData)
                {
                    var monitor = new MonitorInfo();
                    monitor.Size = (uint)Marshal.SizeOf(monitor);
                    monitor.DeviceName = null;
                    var success = GetMonitorInfo(hMonitor, ref monitor);
                    if (success)
                    {
                        DisplayInfo displayinfo = new DisplayInfo
                        {
                            ScreenWidth = monitor.Monitor.Right - monitor.Monitor.Left,
                            ScreenHeight = monitor.Monitor.Bottom - monitor.Monitor.Top
                        };
                        displayinfo.MonitorArea = new Windows.Foundation.Rect(
                            monitor.Monitor.Left,
                            monitor.Monitor.Top,
                            displayinfo.ScreenWidth,
                            displayinfo.ScreenHeight);
                        displays.Add(displayinfo);
                    }
                    return true;
                }, IntPtr.Zero);
            return displays;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        private struct MonitorInfo
        {
            public uint Size;
            public Rect Monitor;
            public Rect WorkArea;
            public uint Flags;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
            public string DeviceName;
        }
    }
}
