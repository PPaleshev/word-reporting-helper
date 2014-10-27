using System.Windows.Forms;
using Microsoft.Office.Tools.Word;

namespace SampleWordHelper.Core.Application
{
    /// <summary>
    /// Экземпляр контекста времени исполнения надстройки.
    /// </summary>
    public class RuntimeContext : IRuntimeContext
    {
        /// <summary>
        /// Объект, предоставляющий доступ к текущему главному окну приложения.
        /// </summary>
        readonly IWindowProvider winProvider;

        public IViewFactory ViewFactory { get; private set; }

        public Microsoft.Office.Interop.Word.Application Application { get; private set; }

        public ApplicationFactory ApplicationFactory { get; private set; }

        /// <summary>
        /// Создаёт новый экземпляр контекста.
        /// </summary>
        /// <param name="application">Объект приложения, в рамках которого выполняется работает надстройка.</param>
        /// <param name="viewFactory">Фабрика представлений.</param>
        /// <param name="applicationFactory">Фабрика вспомогательных объектов.</param>
        /// <param name="windowProvider">Объект, предоставляющий доступ к текущему главному окну приложения.</param>
        public RuntimeContext(Microsoft.Office.Interop.Word.Application application, IViewFactory viewFactory, ApplicationFactory applicationFactory, IWindowProvider windowProvider)
        {
            winProvider = windowProvider;
            Application = application;
            ViewFactory = viewFactory;
            ApplicationFactory = applicationFactory;
        }

        public IWin32Window GetMainWindow()
        {
            return winProvider.GetMainWindow();
        }
    }
}
