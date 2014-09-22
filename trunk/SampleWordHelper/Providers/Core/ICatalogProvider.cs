using SampleWordHelper.Core;

namespace SampleWordHelper.Providers.Core
{
    /// <summary>
    /// Интерфейс компонента, предоставляющего доступ.
    /// </summary>
    public interface ICatalogProvider
    {
        /// <summary>
        /// Создаёт модель для редактирования параметров текущего провайдера.
        /// </summary>
        ISettingsModel CreateSettingsModel();

        /// <summary>
        /// Сохраняет конфигурацию провайдера.
        /// </summary>
        /// <param name="model">Модель конфигурации.</param>
        void ApplyConfiguration(ISettingsModel model);

        /// <summary>
        /// Выполняет инициализацию провайдера.
        /// </summary>
        /// <param name="context">Контекст времени исполнения.</param>
        void Initialize(IRuntimeContext context);

        /// <summary>
        /// Вызывается для завершения активности текущего провайдера.
        /// </summary>
        void Shutdown();
    }
}
