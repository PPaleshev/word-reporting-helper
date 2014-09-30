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
        readonly IDictionary<string, IProviderStrategy> strategies;

        /// <summary>
        /// Массив с названиями всех доступных поставщиков.
        /// </summary>
        public IEnumerable<ListItem> Providers { get; private set; }

        /// <summary>
        /// Название выбранного поставщика.
        /// Содержит <c>null</c>, если не задан.
        /// </summary>
        public string SelectedStrategyName { get; private set; }

        /// <summary>
        /// Модель настроек выбранного поставщика.
        /// </summary>
        public ISettingsModel ProviderSettingsModel { get; private set; }

        public ConfigurationEditorModel(ConfigurationModel configurationModel)
        {
            strategies = new Dictionary<string, IProviderStrategy>(configurationModel.Strategies);
            Providers = strategies.Keys.Select(s => new ListItem("Yello", s)).ToArray();
            SelectedStrategyName = configurationModel.CurrentProviderName;
            UpdateProviderSettings();
        }

        public void UpdateSelectedProvider(ListItem newItem)
        {
            SelectedStrategyName = newItem.Value;
            UpdateProviderSettings();
        }

        /// <summary>
        /// Обновляет конфигурацию провайдера.
        /// </summary>
        void UpdateProviderSettings()
        {
            if (string.IsNullOrWhiteSpace(SelectedStrategyName))
                return;
            var strategy = strategies[SelectedStrategyName];
            ProviderSettingsModel = strategy.ConfigurationManager.CreateSettingsModel();
        }

        /// <summary>
        /// Выполняет валидацию свойств модели.
        /// </summary>
        public ValidationResult Validate()
        {
            if (string.IsNullOrWhiteSpace(SelectedStrategyName))
                return new ValidationResult("Для продолжения работы должен быть выбран активный поставщик каталога.");
            return ProviderSettingsModel != null ? ProviderSettingsModel.Validate() : ValidationResult.CORRECT;
        }
    }
}
