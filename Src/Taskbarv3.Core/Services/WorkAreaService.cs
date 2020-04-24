using System;
using Taskbarv3.Core.Interfaces;
using Taskbarv3.Core.Models;

namespace Taskbarv3.Core.Services
{
    public class WorkAreaService : IWorkAreaService
    {
        private const int SPI_SETWORKAREA = 0x002F;

        private enum WinIniFlags
        {
            SPIF_NONE = 0x000,
            SPIF_UPDATEINIFILE = 0x0001,
            SPIF_SENDWININICHANGE = 0x0002,
            SPIF_SENDCHANGE = SPIF_SENDWININICHANGE
        }

        public void SetWorkArea(RECT rect)
        {
            bool successful = NativeMethods.SystemParametersInfo(SPI_SETWORKAREA, 0, ref rect, (int)WinIniFlags.SPIF_UPDATEINIFILE);

            if (!successful)
            {
                throw new Exception($"Win32 error code: {NativeMethods.LastWin32Error()}");
            }
        }

        public RECT GetSecondaryMonitorScreenBounds()
        {
            var displays = NativeMethods.QueryDisplays();
            if (displays.Count < 2)
            {
                throw new Exception("Could not find a secondary monitor");
            }
            var secondaryDisplayArea = displays[1].MonitorArea;

            return new RECT
            {
                Top = (int)secondaryDisplayArea.Top,
                Bottom = (int)secondaryDisplayArea.Bottom,
                Left = (int)secondaryDisplayArea.Left,
                Right = (int)secondaryDisplayArea.Right
            };
        }
    }
}
