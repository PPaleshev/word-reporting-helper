namespace SampleWordHelper.Providers.Core
{
    /// <summary>
    /// Интерфейс обращения к конфигурационным данным поставщика каталога.
    /// </summary>
    public interface IConfigurationManager
    {
        /// <summary>
        /// Создаёт модель для редактирования параметров текущего провайдера.
        /// Модель не должна изменять актуальные параметры.
        /// </summary>
        ISettingsModel CreateSettingsModel();

        /// <summary>
        /// Сохраняет конфигурацию провайдера.
        /// </summary>
        /// <param name="model">Модель конфигурации.</param>
        void ApplyConfiguration(ISettingsModel model);
    }
}