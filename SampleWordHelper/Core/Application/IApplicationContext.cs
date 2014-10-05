using SampleWordHelper.Indexation;
using SampleWordHelper.Model;

namespace SampleWordHelper.Core.Application
{
    /// <summary>
    /// Контекст исполнения бизнес-приложения.
    /// </summary>
    public interface IApplicationContext 
    {
        /// <summary>
        /// Внешний контекст выполнения надстройки.
        /// </summary>
        IRuntimeContext Environment { get; }

        /// <summary>
        /// Модель используемого каталога, используемого приложением в данный момент.
        /// </summary>
        ICatalog Catalog { get; }

        /// <summary>
        /// Экземпляр механизма, используемого для поиска по индексированному содержимому документов.
        /// </summary>
        ISearchEngine SearchEngine { get; }
    }
}
