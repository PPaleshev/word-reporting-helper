using System;
using System.Windows.Forms;

namespace SampleWordHelper.Core.Native
{
    public class Win32Window : IWin32Window
    {
        readonly string title;

        public IntPtr Handle { get; private set; }

        public Win32Window(IntPtr handle, string title)
        {
            this.title = title;
            Handle = handle;
        }
    }
}