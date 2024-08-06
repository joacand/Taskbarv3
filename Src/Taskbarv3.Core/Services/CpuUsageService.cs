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
        private bool Initialized => cpuCounter != null;

        public CpuUsageService(IConfigHandler configHandler)
        {
            config = configHandler.LoadFromFile();

            if (!config.CpuMonitorDisabled)
            {
                // PerformanceCounter can have a slow start-up time - start in a separate thread
                Task.Run(() =>
                {
                    if (OperatingSystem.IsWindows())
                    {
                        cpuCounter = new PerformanceCounter("Processor Information", "% Processor Time", "_Total");
                        cpuCounter.NextValue();
                    }
                });
            }
        }

#pragma warning disable CA1416 // Validate platform compatibility
        public int NextValue => !Initialized ? 0 : (int)Math.Round(cpuCounter.NextValue());
#pragma warning restore CA1416 // Validate platform compatibility

        public void Dispose()
        {
            cpuCounter?.Dispose();
        }
    }
}
