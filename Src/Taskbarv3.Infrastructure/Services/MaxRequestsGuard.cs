using System;
using System.Collections.Generic;
using System.Threading;
using Taskbarv3.Core.Interfaces;

namespace Taskbarv3.Infrastructure.Services
{
    public class MaxRequestsGuard : IMaxRequestsGuard
    {
        private readonly Queue<DateTime> requestTimes;
        private readonly int maxRequests = 10;
        private readonly TimeSpan timeSpan;

        public MaxRequestsGuard()
        {
            requestTimes = new Queue<DateTime>(maxRequests);
            timeSpan = TimeSpan.FromSeconds(1);
        }

        private void SynchronizeQueue()
        {
            lock (requestTimes)
            {
                while ((requestTimes.Count > 0) && (requestTimes.Peek().Add(timeSpan) < DateTime.UtcNow))
                    requestTimes.Dequeue();
            }
        }

        private bool CanRequestNow()
        {
            SynchronizeQueue();
            return requestTimes.Count < maxRequests;
        }

        public void WaitUntilFreeSlots()
        {
            while (!CanRequestNow())
            {
                Thread.Sleep(requestTimes.Peek().Add(timeSpan).Subtract(DateTime.UtcNow));
            }
            requestTimes.Enqueue(DateTime.UtcNow);
        }
    }
}
