using Microsoft.Office.Interop.Word;
using Microsoft.Office.Tools.Word;

namespace SampleWordHelper.Core
{
    /// <summary>
    /// Контекст работы надстройки.
    /// </summary>
    public interface IRuntimeContext
    {
        /// <summary>
        /// Фабрика представлений.
        /// </summary>
        IViewFactory ViewFactory { get; }

        /// <summary>
        /// Ссылка на экземпляр Microsoft Word, в рамках которого работает текущая надстройка.
        /// </summary>
        Application Application { get; }

        /// <summary>
        /// Фабрика вспомогательных объектов для надстроек уровня приложения.
        /// </summary>
        ApplicationFactory ApplicationFactory { get; }
    }
}