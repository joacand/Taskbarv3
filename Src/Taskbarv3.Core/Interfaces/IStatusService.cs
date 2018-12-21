using System;

namespace Taskbarv3.Core.Interfaces
{
    public interface IStatusService : IStatusSetter
    {
        Action<string> SetStatusAction { get; set; }
    }
}
