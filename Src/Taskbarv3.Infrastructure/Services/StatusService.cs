using System;
using Taskbarv3.Core.Interfaces;

namespace Taskbarv3.Infrastructure.Services
{
    public class StatusService : IStatusService
    {
        public Action<string> SetStatusAction { get; set; }

        public StatusService()
        {
        }

        public void SetStatus(string message)
        {
            SetStatusAction?.Invoke(message);
        }
    }
}
