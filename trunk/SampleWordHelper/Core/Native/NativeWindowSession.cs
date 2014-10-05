using System.Diagnostics;
using System.Windows.Forms;
using SampleWordHelper.Core.Common;

namespace SampleWordHelper.Core.Native
{
    /// <summary>
    /// Вспомогательный класс для создания окон, над которыми могут отображаться Windows Forms формы.
    /// </summary>
    public static class NativeWindowSession
    {
        /// <summary>
        /// Получает главное окно текущего процесса.
        /// </summary>
        public static DisposableNativeWindow GetCurrentProcessMainWindow()
        {
            var mainWindowHandle = Process.GetCurrentProcess().MainWindowHandle;
            var window = new NativeWindow();
            window.AssignHandle(mainWindowHandle);
            return new DisposableNativeWindow(window);
        }

        /// <summary>
        /// Обёртка для отсоединения окна при вызове Dispose.
        /// </summary>
        public sealed class DisposableNativeWindow : BasicDisposable
        {
            /// <summary>
            /// Экземпляр окна.
            /// </summary>
            public readonly NativeWindow window;

            public DisposableNativeWindow(NativeWindow window)
            {
                this.window = window;
            }

            protected override void DisposeManaged()
            {
                window.ReleaseHandle();
            }
        }
    }
}
