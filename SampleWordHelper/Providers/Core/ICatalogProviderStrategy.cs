using SampleWordHelper.Core.Application;
using SampleWordHelper.Model;

namespace SampleWordHelper.Providers.Core
{
    /// <summary>
    /// Интерфейс компонента, предоставляющего доступ.
    /// </summary>
    public interface ICatalogProviderStrategy
    {
        /// <summary>
        /// Выполняет инициализацию провайдера.
        /// </summary>
        /// <param name="context">Контекст времени исполнения.</param>
        /// <returns>Возвращает true, если настройки успешно загружены и провайдер успешно проинициализирован, и false, если требуется настройка пользователем.</returns>
        bool Initialize(IRuntimeContext context);

        /// <summary>
        /// Выполняет создание модели каталога.
        /// </summary>
        CatalogLoadResult LoadCatalog();

        /// <summary>
        /// Вызывается для завершения активности текущего провайдера.
        /// </summary>
        void Shutdown();
    }
}
