using SampleWordHelper.Indexation;
using SampleWordHelper.Model;

namespace SampleWordHelper.Core.Application
{
    /// <summary>
    /// Контекст приложения.
    /// </summary>
    public class ApplicationContext : IApplicationContext
    {
        /// <summary>
        /// Создаёт новый экземпляр контекста приложения.
        /// </summary>
        public ApplicationContext(IRuntimeContext runtime, ICatalog catalog, ISearchEngine searchEngine)
        {
            Environment = runtime;
            Catalog = catalog;
            SearchEngine = searchEngine;
        }

        public IRuntimeContext Environment { get; private set; }

        public ICatalog Catalog { get; set; }

        public ISearchEngine SearchEngine { get; private set; }
    }
}
