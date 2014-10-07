using System;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.InteropServices;

namespace SampleWordHelper.Core.Native
{
    /// <summary>
    /// Вспомогательный класс для вызова методов WinAPI.
    /// </summary>
    static class WinApi
    {
        /// <summary>
        /// Последовательность названий классов окон, ведущих к окну редактируемого документа в Word.
        /// </summary>
        static readonly string[] WIN_CLASSES = new[] {"OpusApp", "_WwF", "_WwB", "_WwG"};

        public const uint SWP_NOACTIVATE = 0x0010;

        public const uint SWP_NOZORDER = 0x0004;

        [DllImport("user32")]
        public static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        [DllImport("user32.dll")]
        public static extern IntPtr SetParent(IntPtr hWndChild, IntPtr hWndNewParent);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern IntPtr FindWindowEx(IntPtr hwndParent, IntPtr hwndChildAfter, string lpszClass, string lpszWindow);

        [DllImport("user32.dll")]
        public static extern bool GetWindowRect(IntPtr hWnd, ref RECT rect);

        [DllImport("user32.dll", EntryPoint = "SetWindowPos")]
        public static extern bool SetWindowPos(
            uint hWnd, // handle to window
            int hWndInsertAfter, // placement-order handle
            int X, // horizontal position
            int Y, // vertical position
            int cx, // width
            int cy, // height
            uint uFlags // window-positioning options
            );

        [DllImport("user32.dll")]
        public static extern int SetActiveWindow(int hWnd);

        /// <summary>
        /// Выполняет поиск дескриптора окна редактируемого документа Ms Word.
        /// </summary>
        /// <param name="activeWindowTitle">Заголовок активного окна.</param>
        /// <returns>Возвращает дескриптор найденного окна.</returns>
        public static IntPtr FindMicrosoftWordDocumentWindow(string activeWindowTitle)
        {
            var handle = FindWindow(WIN_CLASSES[0], string.IsNullOrWhiteSpace(activeWindowTitle) ? null : activeWindowTitle);
            Debug.WriteLine("Root handle: {0}", handle);
            for (var i = 1; i < WIN_CLASSES.Length; i++)
                handle = FindWindowEx(handle, IntPtr.Zero, WIN_CLASSES[i], null);
            return handle;
        }

        /// <summary>
        /// Возвращает границы окна.
        /// </summary>
        /// <param name="handle">Дескриптор окна.</param>
        public static Rectangle GetWindowBounds(IntPtr handle)
        {
            var r = new RECT();
            GetWindowRect(handle, ref r);
            return new Rectangle(r.Left, r.Top, r.Right - r.Left + 1, r.Bottom - r.Top + 1);
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct RECT
    {
        public int Left;
        public int Top;
        public int Right;
        public int Bottom;

        public RECT(Rectangle r)
            : this()
        {
            Left = r.Left;
            Top = r.Top;
            Right = r.Right;
            Bottom = r.Bottom;
        }
    }
}
