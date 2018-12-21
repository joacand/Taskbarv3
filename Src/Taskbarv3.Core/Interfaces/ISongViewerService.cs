using System;

namespace Taskbarv3.Core.Interfaces
{
    public interface ISongViewerService : IDisposable
    {
        string CurrentSong { get; }
        string FilePath { get; }
        bool IsOnline { get; }
        void Start();
        void Stop();
    }
}
