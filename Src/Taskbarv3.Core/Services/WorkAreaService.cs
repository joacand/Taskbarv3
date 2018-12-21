using Taskbarv3.Core.Interfaces;
using Taskbarv3.Core.Models;

namespace Taskbarv3.Core.Services
{
    public class WorkAreaService : IWorkAreaService
    {
        private const uint SPIF_SENDWININICHANGE = 2;
        private const uint SPIF_UPDATEINIFILE = 1;
        private const uint SPIF_CHANGE = SPIF_UPDATEINIFILE | SPIF_SENDWININICHANGE;
        private const uint WM_SETTINGCHANGE = 0x1a;
        private const uint SPI_GETWORKAREA = 48;
        private const uint SPI_SETWORKAREA = 0x002F;

        public bool SetWorkArea(RECT rect)
        {
            bool successful = NativeMethods.SystemParametersInfo(SPI_SETWORKAREA, 0, ref rect, SPIF_UPDATEINIFILE);
            return successful;
        }

        public RECT GetWorkArea()
        {
            var rect = new RECT();
            NativeMethods.SystemParametersInfo(SPI_GETWORKAREA, 0, ref rect, 0);
            return rect;
        }
    }
}
