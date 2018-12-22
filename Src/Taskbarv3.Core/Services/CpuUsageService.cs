using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Taskbarv3.Core.Interfaces;

namespace Taskbarv3.Core.Services
{
    public sealed class CpuUsageService : ICpuUsageService, IDisposable
    {
        private PerformanceCounter cpuCounter;
        private bool initialized;

        public CpuUsageService()
        {
            // PerformanceCounter can have a slow start-up time - start in a separate thread
            Task.Run(() =>
            {
                cpuCounter = new PerformanceCounter("Processor Information", "% Processor Time", "_Total");
                cpuCounter.NextValue();
                initialized = true;
            });
        }

        public int NextValue {
            get {
                if (!initialized)
                {
                    return 0;
                }
                return (int)Math.Round(cpuCounter.NextValue());
            }
        }

        public void Dispose()
        {
            cpuCounter.Dispose();
        }
    }
}
