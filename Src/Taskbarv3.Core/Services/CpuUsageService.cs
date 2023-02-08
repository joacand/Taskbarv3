using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Taskbarv3.Core.Interfaces;
using Taskbarv3.Core.Models;

namespace Taskbarv3.Core.Services
{
    public sealed class CpuUsageService : ICpuUsageService, IDisposable
    {
        private readonly MainConfig config;
        private PerformanceCounter cpuCounter;
        private bool initialized;

        public CpuUsageService(IConfigHandler configHandler)
        {
            config = configHandler.LoadFromFile();

            if (!config.CpuMonitorDisabled)
            {
                // PerformanceCounter can have a slow start-up time - start in a separate thread
                Task.Run(() =>
                {
                    cpuCounter = new PerformanceCounter("Processor Information", "% Processor Time", "_Total");
                    cpuCounter.NextValue();
                    initialized = true;
                });
            }
        }

        public int NextValue => !initialized ? 0 : (int)Math.Round(cpuCounter.NextValue());

        public void Dispose()
        {
            cpuCounter.Dispose();
        }
    }
}
