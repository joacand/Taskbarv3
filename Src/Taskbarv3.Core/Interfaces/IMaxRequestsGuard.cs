namespace Taskbarv3.Core.Interfaces
{
    public interface IMaxRequestsGuard
    {
        void WaitUntilFreeSlots();
    }
}
