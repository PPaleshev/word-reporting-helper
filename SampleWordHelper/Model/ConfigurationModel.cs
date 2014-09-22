using System;
using System.Collections.Generic;
using System.Configuration;
using System.Windows.Forms;
using SampleWordHelper.Configuration;
using SampleWordHelper.Providers.Core;

namespace SampleWordHelper.Model
{
    /// <summary>
    /// Модель конфигурации приложения.
    /// </summary>
    public class ConfigurationModel
    {
        /// <summary>
        /// Секция с настройками приложения.
        /// </summary>
        readonly ReportHelperConfigurationSection section;

        /// <summary>
        /// Отображение из имени провайдера в реализующий его класс.
        /// </summary>
        public IDictionary<string, ICatalogProvider> Providers { get; private set; }

        /// <summary>
        /// Название выбранного провайдера.
        /// </summary>
        public string CurrentProviderName { get; private set; }

        /// <summary>
        /// Флаг, равный true, если активный поставщик установлен, иначе false.
        /// </summary>
        public bool HasActiveProvider
        {
            get { return string.IsNullOrWhiteSpace(CurrentProviderName); }
        }

        /// <summary>
        /// Создаёт конфигурацию приложения, загружая её из секции с названием <paramref name="sectionName"/>.
        /// </summary>
        /// <param name="sectionName">Название секции, в которой содержатся настройки.</param>
        public ConfigurationModel(string sectionName)
        {
            section = (ReportHelperConfigurationSection) ConfigurationManager.GetSection(sectionName);
            Providers = new Dictionary<string, ICatalogProvider>();
            LoadProvidersSafe();
        }

        /// <summary>
        /// Создаёт модель для редактирования конфигурации.
        /// </summary>
        /// <returns></returns>
        public ConfigurationEditorModel CreateEditorModel()
        {
            return new ConfigurationEditorModel(this);
        }

        /// <summary>
        /// Обновляет текущую конфигурацию на основании отредактированной модели.
        /// </summary>
        /// <param name="model"></param>
        public UpdateResult Update(ConfigurationEditorModel model)
        {
            if (!Providers.ContainsKey(model.SelectedProviderName))
                throw new ArgumentException("invalid provider name: " + model.SelectedProviderName);
            var result = new UpdateResult(!string.Equals(CurrentProviderName, model.SelectedProviderName), GetActiveProvider());
            CurrentProviderName = model.SelectedProviderName;
            return result;
        }

        /// <summary>
        /// Безопасно с точки зрения исключений загружает все описанные в конфигурации провайдеры каталогов.
        /// </summary>
        void LoadProvidersSafe()
        {
            foreach (CatalogProviderConfigurationElement providerElement in section.CatalogProviders)
                LoadOneSafe(providerElement);
            var providerName = Properties.Settings.Default.CurrentProviderName;
            if (!string.IsNullOrWhiteSpace(providerName) && Providers.ContainsKey(providerName))
                CurrentProviderName = providerName;
        }

        /// <summary>
        /// Безопасно с точки зрения исключений загружает описание провайдера.
        /// </summary>
        void LoadOneSafe(CatalogProviderConfigurationElement providerElement)
        {
            if (!typeof (ICatalogProvider).IsAssignableFrom(providerElement.Class))
                return;
            var instance = (ICatalogProvider) Activator.CreateInstance(providerElement.Class);
            Providers.Add(providerElement.Name, instance);
        }

        /// <summary>
        /// Возвращает поставщик каталога, используемый в данный момент.
        /// Если поставщик каталога не установлен, возвращает <c>null</c>.
        /// </summary>
        public ICatalogProvider GetActiveProvider()
        {
            return string.IsNullOrWhiteSpace(CurrentProviderName) ? null : Providers[CurrentProviderName];
        }
    }

    /// <summary>
    /// Результат обновления конфигурации.
    /// </summary>
    public struct UpdateResult
    {
        /// <summary>
        /// Флаг, равный true, если после обновления поставщик был изменён.
        /// </summary>
        public readonly bool providerChanged;

        /// <summary>
        /// Предыдущий активный поставщик каталога.
        /// В случае, если не поставщик не менялся, содержит ссылку на последнего активного поставщика.
        /// </summary>
        public ICatalogProvider previousProvider;

        public UpdateResult(bool providerChanged, ICatalogProvider previousProvider)
        {
            this.providerChanged = providerChanged;
            this.previousProvider = previousProvider;
        }
    }
}
