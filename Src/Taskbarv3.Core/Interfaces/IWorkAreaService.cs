using Taskbarv3.Core.Models;

namespace Taskbarv3.Core.Interfaces
{
    public interface IWorkAreaService
    {
        bool SetWorkArea(RECT rect);
        RECT GetWorkArea();
    }
}
