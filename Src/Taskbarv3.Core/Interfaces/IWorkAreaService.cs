using Taskbarv3.Core.Models;

namespace Taskbarv3.Core.Interfaces
{
    public interface IWorkAreaService
    {
        void SetWorkArea(Rect rect);
        Rect GetSecondaryMonitorScreenBounds();
    }
}
