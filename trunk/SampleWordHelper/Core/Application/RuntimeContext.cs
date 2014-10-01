using Microsoft.Office.Tools.Word;

namespace SampleWordHelper.Core.Application
{
    /// <summary>
    /// Экземпляр контекста времени исполнения надстройки.
    /// </summary>
    public class RuntimeContext : IRuntimeContext
    {
        public IViewFactory ViewFactory { get; private set; }

        public Microsoft.Office.Interop.Word.Application Application { get; private set; }

        public ApplicationFactory ApplicationFactory { get; private set; }

        /// <summary>
        /// Создаёт новый экземпляр контекста.
        /// </summary>
        /// <param name="application">Объект приложения, в рамках которого выполняется работает надстройка.</param>
        /// <param name="viewFactory">Фабрика представлений.</param>
        /// <param name="applicationFactory">Фабрика вспомогательных объектов.</param>
        public RuntimeContext(Microsoft.Office.Interop.Word.Application application, IViewFactory viewFactory, ApplicationFactory applicationFactory)
        {
            Application = application;
            ViewFactory = viewFactory;
            ApplicationFactory = applicationFactory;
        }
    }
}
