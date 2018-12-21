using Taskbarv3.Core.Models;

namespace Taskbarv3.Core.Interfaces
{
    public interface IWindowService
    {
        void ShowWindow(PopupWindow window, object dataContext);
        void CloseWindow(PopupWindow window);
    }
}
