using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using SampleWordHelper.Providers.Core;

namespace SampleWordHelper.Model
{
    /// <summary>
    /// Модель для редактирования настроек приложения.
    /// </summary>
    public class ConfigurationEditorModel : ISettingsEditorModel
    {
        /// <summary>
        /// Отображение из названия провайдера в его стратегию.
        /// </summary>
        readonly IDictionary<string, IProviderFactory> factoriesMap;

        /// <summary>
        /// Массив с названиями всех доступных фабрик.
        /// </summary>
        public IEnumerable<ListItem> Factories { get; private set; }

        /// <summary>
        /// Название выбранного поставщика.
        /// Содержит <c>null</c>, если не задан.
        /// </summary>
        public string SelectedProviderName { get; private set; }

        /// <summary>
        /// Описание выбранного поставщика.
        /// </summary>
        public string SelectedProviderDescription { get; private set; }

        /// <summary>
        /// Путь к каталогу с логами.
        /// </summary>
        public string LogDirectory { get; private set; }

        /// <summary>
        /// Модель настроек выбранного поставщика.
        /// </summary>
        public ISettingsModel ProviderSettingsModel { get; private set; }

        public ConfigurationEditorModel(IDictionary<string, IProviderFactory> factories, string currentName)
        {
            factoriesMap = new Dictionary<string, IProviderFactory>(factories);
            Factories = factoriesMap.Select(pair => new ListItem(pair.Value.GetDescription().DisplayName, pair.Key)).ToArray();
            SelectedProviderName = currentName;
            UpdateProviderSettings();
            LogDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "WordHelper");
        }

        /// <summary>
        /// Обновляет информацию о выбранном поставщике.
        /// </summary>
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
            var factory = factoriesMap[SelectedProviderName];
            ProviderSettingsModel = factory.CreateSettingsModel();
            SelectedProviderDescription = factory.GetDescription().Description;
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
