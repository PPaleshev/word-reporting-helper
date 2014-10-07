using System;
using System.Diagnostics;
using System.Threading;
using System.Windows.Forms;
using SampleWordHelper.Core.Native;
using Microsoft.Office.Interop.Word;

namespace SampleWordHelper.Core.Application
{
    /// <summary>
    /// Интерфейс к классу, предоставляющим информацию о главном окне приложения.
    /// </summary>
    public interface IWindowProvider
    {
        /// <summary>
        /// Возвращает ссылку на главное окно приложения в виде <see cref="IWin32Window"/>.
        /// </summary>
        IWin32Window GetMainWindow();
    }


    class MsWordWindowProvider: IWindowProvider
    {
        object window;
        public MsWordWindowProvider(ApplicationEvents4_Event events)
        {
            window = new Win32Window(IntPtr.Zero, "");
            events.WindowActivate += ApplicationOnWindowActivate;
        }

        void ApplicationOnWindowActivate(Document doc, Window wn)
        {
            var win = (IWin32Window) Thread.VolatileRead(ref window);
            var process = Process.GetCurrentProcess();
            var handle = process.MainWindowHandle;
            if (handle == win.Handle)
                return;
            Thread.VolatileWrite(ref window, new Win32Window(handle, process.MainWindowTitle));
        }

        public IWin32Window GetMainWindow()
        {
            return (Win32Window) Thread.VolatileRead(ref window);
        }
    }
}