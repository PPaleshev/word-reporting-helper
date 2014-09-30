using SampleWordHelper.Core;

namespace SampleWordHelper.Providers.Core
{
    /// <summary>
    /// Интерфейс стратегии компонента, предоставляющего доступ к каталогу.
    /// </summary>
    public interface IProviderStrategy
    {
        /// <summary>
        /// Возвращает объект для управления конфигурацией поставщика.
        /// </summary>
        IConfigurationManager ConfigurationManager { get; }

        /// <summary>
        /// Возвращает стратегию работы с каталогом.
        /// </summary>
        ICatalogStrategy CatalogStrategy { get; }

        /// <summary>
        /// Выполняет инициализацию провайдера.
        /// </summary>
        /// <param name="context">Контекст времени исполнения.</param>
        /// <param name="reinitialize">Флаг, равный true, если выполняется повторная инициализация провайдера без завершения его работы.</param>
        /// <returns>Возвращает true, если настройки успешно загружены и провайдер успешно проинициализирован, и false, если требуется настройка пользователем.</returns>
        bool Initialize(IRuntimeContext context, bool reinitialize);

        /// <summary>
        /// Вызывается для завершения работы с каталогом.
        /// </summary>
        void Shutdown();
    }
}
