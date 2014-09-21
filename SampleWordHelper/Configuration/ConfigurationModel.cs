using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using SampleWordHelper.Core;
using SampleWordHelper.Model;
using SampleWordHelper.Providers.Core;

namespace SampleWordHelper.Configuration
{
    /// <summary>
    /// Модель конфигурации приложения.
    /// </summary>
    public class ConfigurationModel
    {
        readonly IConfigurationChangedCallback callback;
        readonly ReportHelperConfigurationSection section;

        /// <summary>
        /// Отображение из имени провайдера в реализующий его класс.
        /// </summary>
        public IDictionary<string, ICatalogProvider> Providers { get; private set; }

        /// <summary>
        /// Название выбранного провайдера.
        /// </summary>
        public string CurrentProviderName { get; private set; }

        public ConfigurationModel(string sectionName, IConfigurationChangedCallback callback)
        {
            this.callback = callback;
            section = (ReportHelperConfigurationSection) ConfigurationManager.GetSection(sectionName);
            Providers = new Dictionary<string, ICatalogProvider>();
            LoadProvidersSafe();
        }

        public void Update(string newProviderName)
        {
            ICatalogProvider provider;
            if (!Providers.TryGetValue(newProviderName, out provider))
                throw new ArgumentException("invalid provider name: " + newProviderName);
            CurrentProviderName = newProviderName;
            callback.OnCurrentProviderChanged();
        }

        /// <summary>
        /// Безопасно с точки зрения исключений загружает все описанные в конфигурации провайдеры каталогов.
        /// </summary>
        void LoadProvidersSafe()
        {
            foreach (CatalogProviderConfigurationElement providerElement in section.CatalogProviders)
                LoadOneSafe(providerElement);
            var providerName = section.CatalogProviders.CurrentProviderName;
            if (!string.IsNullOrWhiteSpace(providerName) && Providers.ContainsKey(providerName))
                CurrentProviderName = providerName;
        }

        /// <summary>
        /// Безопасно с точки зрения исключений загружает описание провайдера.
        /// </summary>
        void LoadOneSafe(CatalogProviderConfigurationElement providerElement)
        {
            if (!typeof(ICatalogProvider).IsAssignableFrom(providerElement.Class))
                return;
            var instance = (ICatalogProvider) Activator.CreateInstance(providerElement.Class);
            Providers.Add(providerElement.Name, instance);
        }
    }

    public interface IConfigurationChangedCallback
    {
        void OnCurrentProviderChanged();
    }
}
