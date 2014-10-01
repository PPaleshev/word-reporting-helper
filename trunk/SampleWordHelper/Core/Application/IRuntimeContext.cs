using Microsoft.Office.Tools.Word;

namespace SampleWordHelper.Core.Application
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
        Microsoft.Office.Interop.Word.Application Application { get; }

        /// <summary>
        /// Фабрика вспомогательных объектов для надстроек уровня приложения.
        /// </summary>
        ApplicationFactory ApplicationFactory { get; }
    }
}