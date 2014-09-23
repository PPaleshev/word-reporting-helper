using System;
using System.Drawing;
using System.Runtime.InteropServices;

namespace SampleWordHelper.Native
{
    /// <summary>
    /// Вспомогательный класс для вызова методов WinAPI.
    /// </summary>
    static class WinApi
    {
        public const uint SWP_NOACTIVATE = 0x0010;

        public const uint SWP_NOZORDER = 0x0004;

        [DllImport("user32")]
        public static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        [DllImport("user32.dll")]
        public static extern IntPtr SetParent(IntPtr hWndChild, IntPtr hWndNewParent);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern IntPtr FindWindowEx(IntPtr hwndParent, IntPtr hwndChildAfter, string lpszClass, string lpszWindow);

        [DllImport("user32.dll")]
        public static extern bool GetWindowRect(IntPtr hWnd, ref Rectangle rect);
    }
}
