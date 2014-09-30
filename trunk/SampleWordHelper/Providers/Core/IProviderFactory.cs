namespace SampleWordHelper.Providers.Core
{
    /// <summary>
    /// Интерфейс фабрики поставщика каталога. Предоставляет доступ к конфигурации провайдера.
    /// </summary>
    public interface IProviderFactory
    {
        /// <summary>
        /// Возвращает описание провайдеров, создаваемых данной фабрикой.
        /// </summary>
        ProviderDescription GetDescription();

        /// <summary>
        /// Создаёт модель параметров провайдеров.
        /// </summary>
        ISettingsModel CreateSettingsModel();

        /// <summary>
        /// Сохраняет настройки для создания провайдеров.
        /// </summary>
        void ApplyConfiguration(ISettingsModel model);

        /// <summary>
        /// Создаёт провайдер каталога.
        /// </summary>
        ICatalogProviderStrategy CreateStrategy();
    }
}