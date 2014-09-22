using System.Collections.Generic;
using System.Linq;
using SampleWordHelper.Providers.Core;

namespace SampleWordHelper.Model
{
    /// <summary>
    /// Модель для редактирования настроек приложения.
    /// </summary>
    public class ConfigurationEditorModel : ISettingsEditorModel
    {
        readonly IDictionary<string, ICatalogProvider> providers;

        /// <summary>
        /// Массив с названиями всех доступных поставщиков.
        /// </summary>
        public IEnumerable<ListItem> Providers { get; private set; }

        /// <summary>
        /// Название выбранного поставщика.
        /// Содержит <c>null</c>, если не задан.
        /// </summary>
        public string SelectedProviderName { get; private set; }

        /// <summary>
        /// Модель настроек выбранного поставщика.
        /// </summary>
        public ISettingsModel ProviderSettingsModel { get; private set; }

        public ConfigurationEditorModel(ConfigurationModel configurationModel)
        {
            providers = new Dictionary<string, ICatalogProvider>(configurationModel.Providers);
            Providers = providers.Keys.Select(s => new ListItem("Yello", s)).ToArray();
            SelectedProviderName = configurationModel.CurrentProviderName;
            UpdateProviderSettings();
        }

        public void UpdateSelectedProvider(ListItem newItem)
        {
            SelectedProviderName = newItem.Value;
            UpdateProviderSettings();
        }

        /// <summary>
        /// Обновляет конфигурацию провайдера.
        /// </summary>
        void UpdateProviderSettings()
        {
            if (string.IsNullOrWhiteSpace(SelectedProviderName))
                return;
            var provider = providers[SelectedProviderName];
            ProviderSettingsModel = provider.CreateSettingsModel();
        }

        /// <summary>
        /// Выполняет валидацию свойств модели.
        /// </summary>
        public ValidationResult Validate()
        {
            if (string.IsNullOrWhiteSpace(SelectedProviderName))
                return new ValidationResult("Для продолжения работы должен быть выбран активный поставщик каталога.");
            return ProviderSettingsModel != null ? ProviderSettingsModel.Validate() : ValidationResult.CORRECT;
        }
    }
}
