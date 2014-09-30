using SampleWordHelper.Model;

namespace SampleWordHelper.Providers.Core
{
    /// <summary>
    /// Стратегия работы с каталогом.
    /// </summary>
    public interface ICatalogStrategy
    {
        /// <summary>
        /// Выполняет создание модели каталога.
        /// </summary>
        /// <param name="mode">Режим загрузки каталога.</param>
        CatalogModel LoadCatalog(CatalogLoadMode mode);
    }
}
