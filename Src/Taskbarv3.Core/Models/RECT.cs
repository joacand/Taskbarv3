using System;
using System.Runtime.InteropServices;

namespace Taskbarv3.Core.Models
{
    [StructLayout(LayoutKind.Sequential)]
    public struct RECT
    {
        public Int32 Left;
        public Int32 Top;
        public Int32 Right;
        public Int32 Bottom;
    }
}
