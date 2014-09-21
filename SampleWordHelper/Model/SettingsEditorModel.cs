using System.Collections.Generic;
using System.Linq;
using SampleWordHelper.Configuration;
using SampleWordHelper.Providers.Core;

namespace SampleWordHelper.Model
{
    /// <summary>
    /// Модель для редактирования настроек приложения.
    /// </summary>
    public class SettingsEditorModel
    {
        readonly IDictionary<string, ICatalogProvider> providers;

        /// <summary>
        /// Массив с названиями всех доступных поставщиков.
        /// </summary>
        public Item<string>[] Providers { get; private set; }

        /// <summary>
        /// Название выбранного поставщика.
        /// Содержит <c>null</c>, если не задан.
        /// </summary>
        public string SelectedProviderName { get; private set; }

        /// <summary>
        /// Модель настроек выбранного поставщика.
        /// </summary>
        public ISettingsModel ProviderSettingsModel { get; private set; }


        public SettingsEditorModel(ConfigurationModel configuration)
        {
            providers = new Dictionary<string, ICatalogProvider>(configuration.Providers);
            Providers = providers.Keys.Select(s => new Item<string>("Yello", s)).ToArray();
            SelectedProviderName = configuration.CurrentProviderName;
            UpdateProviderSettings();
        }

        public void UpdateSelectedProvider(Item<string> newItem)
        {
            SelectedProviderName = newItem.Object;
            UpdateProviderSettings();
        }

        void UpdateProviderSettings()
        {
            if (string.IsNullOrWhiteSpace(SelectedProviderName))
                return;
            var provider = providers[SelectedProviderName];
            ProviderSettingsModel = provider.CreateSettingsModel();
        }

        public ValidationResult Validate()
        {
            if (string.IsNullOrWhiteSpace(SelectedProviderName))
                return new ValidationResult("Для продолжения работы должен быть выбран активный поставщик каталога.");
            return ProviderSettingsModel != null ? ProviderSettingsModel.Validate() : ValidationResult.CORRECT;
        }
    }
}
