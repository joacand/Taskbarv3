using System;
using System.Linq;
using System.Text;
using System.Timers;
using Taskbarv3.Core.Interfaces;
using Taskbarv3.Core.Models;

namespace Taskbarv3.Core.Services
{
    public class SongViewerService : ISongViewerService
    {
        public string CurrentSong { get; private set; } = "";
        public string FilePath { get; private set; } = "";
        public bool IsOnline { get; private set; }

        private string lastSong = "";
        private Timer timer;
        private IntPtr hndl = IntPtr.Zero;
        private IntPtr correctHwnd = IntPtr.Zero;
        private bool correctHwndFound = false;
        private MusicPlayer musicPlayer = MusicPlayer.Zero;
        private const string NOSONGPLAYING = "No song playing";

        private string[] wrongFoobars = {
            "uninteresting", "amip_foo_wrapper_window", "AMIPwindow", "GDI+ Window", "Playback error", "Default IME", "MSCTFIME UI"
        };
        private string[] wrongSpotifies = {
            "tooltip_view_", "Default IME", "MSCTFIME UI", "Msg", "w"
        };

        private enum MusicPlayer
        {
            Zero = 0,
            Foobar2000 = 1,
            Spotify = 2
        }

        public void Start()
        {
            timer = new Timer
            {
                Interval = 1000,
                Enabled = false
            };
            timer.Elapsed += Timer_Elapsed;
            NativeMethods.EnumWindows(new NativeMethods.EnumWindowsProc(EnumTheWindows), IntPtr.Zero);
            EnumAndFetchTitle();

            timer.Start();
            IsOnline = true;
        }

        public void Stop()
        {
            timer.Stop();
            ResetState();
        }

        private void ResetState()
        {
            hndl = IntPtr.Zero;
            correctHwnd = IntPtr.Zero;
            correctHwndFound = false;
            musicPlayer = MusicPlayer.Zero;
            lastSong = "";
            CurrentSong = "";
            FilePath = "";
            IsOnline = false;
        }

        private void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            EnumAndFetchTitle();
        }

        private void EnumAndFetchTitle()
        {
            if (!correctHwndFound)
            {
                NativeMethods.EnumWindows(new NativeMethods.EnumWindowsProc(EnumTheWindows), IntPtr.Zero);
            }
            FetchTitle(correctHwnd);
        }

        private void FetchTitle(IntPtr hWnd)
        {
            var size = NativeMethods.GetWindowTextLength(hWnd);

            if (size++ > 0)
            {
                // Fetch the window title
                var sb = new StringBuilder(size);
                NativeMethods.GetWindowText(hWnd, sb, size);
                var wndTitle = sb.ToString();

                NativeMethods.GetWindowThreadProcessId(hWnd, out uint pId);

                var mySb = new StringBuilder(1024);
                if (!correctHwndFound)
                {
                    hndl = NativeMethods.OpenProcess(ProcessAccessFlags.Limited, false, (int)pId);
                    correctHwnd = hWnd;

                    // Fetch process name
                    if ((int)hndl > 4)
                    {
                        NativeMethods.GetModuleBaseNameA(hndl, IntPtr.Zero, mySb, (uint)mySb.Capacity);
                    }
                    NativeMethods.CloseHandle(hndl);
                }

                var baseName = mySb.ToString();
                if ((correctHwndFound || baseName.Equals("foobar2000.exe")) && IsCorrectFoobarWindow(wndTitle))
                {
                    var wndTitleParts = wndTitle.Split('[');
                    var song = wndTitleParts[0];

                    if (!song.Equals(lastSong))
                    {
                        lastSong = song;
                    }

                    FetchCurrentSong(MusicPlayer.Foobar2000, hWnd, wndTitle);
                }
                else if ((correctHwndFound || baseName.Equals("Spotify.exe")) && IsCorrectSpotify(wndTitle))
                {
                    if (wndTitle.Length > 10)
                    {
                        wndTitle = wndTitle.Substring(0);
                    }

                    if (!wndTitle.Equals(lastSong))
                    {
                        lastSong = wndTitle;
                    }

                    FetchCurrentSong(MusicPlayer.Spotify, hWnd, wndTitle);
                }
            }
        }

        private bool IsCorrectFoobarWindow(string wndTitle)
        {
            return
                !wrongFoobars.Contains(wndTitle) &&
                !wndTitle.Contains("Playlist Search")
                && (musicPlayer == MusicPlayer.Zero || musicPlayer == MusicPlayer.Foobar2000);
        }

        private bool IsCorrectSpotify(string wndTitle)
        {
            return !wrongSpotifies.Contains(wndTitle) && (musicPlayer == MusicPlayer.Zero || musicPlayer == MusicPlayer.Spotify);
        }

        private void FetchCurrentSong(MusicPlayer musicPlayer, IntPtr hWnd, string windowTitle)
        {
            if (!correctHwndFound)
            {
                correctHwnd = hWnd;
                correctHwndFound = true;
                this.musicPlayer = musicPlayer;
            }

            if (windowTitle.Equals("Spotify", StringComparison.InvariantCultureIgnoreCase) ||
                windowTitle.Equals("foobar2000", StringComparison.InvariantCultureIgnoreCase))
            {
                windowTitle = NOSONGPLAYING;
            }
            else if (windowTitle.Length == 1) // One letter titles usually means it has the wrong window handle
            {
                windowTitle = "ERROR 011: Window Handle wrong";
            }
            else
            {
                if (this.musicPlayer == MusicPlayer.Foobar2000)
                {
                    FetchSongFoobar(windowTitle);
                }
                else
                {
                    CurrentSong = windowTitle;
                }
            }
        }

        private void FetchSongFoobar(string windowTitle)
        {
            if (!windowTitle.Equals(NOSONGPLAYING))
            {
                var parts = windowTitle.Split(new string[] { "###" }, StringSplitOptions.None);
                if (parts.Length >= 3)
                {
                    var isPaused = parts[2].Trim().Equals("Paused", StringComparison.InvariantCultureIgnoreCase);
                    CurrentSong = parts[0].Trim();
                    FilePath = parts[1].Trim();

                    if (isPaused)
                    {
                        CurrentSong += " (Paused)";
                    }
                }
            }
        }

        private bool EnumTheWindows(IntPtr hWnd, IntPtr lParam)
        {
            if (correctHwndFound)
            {
                FetchTitle(correctHwnd);
            }
            else
            {
                FetchTitle(hWnd);
            }
            return true;
        }

        ~SongViewerService()
        {
            Dispose(false);
        }

        private bool disposed;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                // Dispose of resources held by this instance.
                disposed = true;

                // Suppress finalization of this disposed instance.
                if (disposing)
                {
                    if (timer != null)
                    {
                        timer.Dispose();
                    }
                    GC.SuppressFinalize(this);
                }
            }
        }

        public void Dispose()
        {
            Dispose(true);
        }
    }
}