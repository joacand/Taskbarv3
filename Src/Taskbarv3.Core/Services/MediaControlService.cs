using System;
using Taskbarv3.Core.Interfaces;
using Taskbarv3.Core.Models;

namespace Taskbarv3.Core.Services
{
    public class MediaControlService : IMediaControlService
    {
        public void PlayPause()
        {
            NativeMethods.keybd_event((byte)VirtualKeyCodes.VK_MEDIA_PLAY_PAUSE, 0x45, 0, IntPtr.Zero);
        }

        public void NextSong()
        {
            NativeMethods.keybd_event((byte)VirtualKeyCodes.VK_MEDIA_NEXT_TRACK, 0x45, 0, IntPtr.Zero);
        }

        private enum VirtualKeyCodes
        {
            VK_MEDIA_PLAY_PAUSE = 0xB3,
            VK_MEDIA_NEXT_TRACK = 0xB0
        }
    }
}
