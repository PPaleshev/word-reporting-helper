using System;
using System.Collections.Generic;
using System.Configuration;
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
        /// Отображение из имени фабрики провайдера в реализующий его класс.
        /// </summary>
        readonly IDictionary<string, IProviderFactory> factories;

        /// <summary>
        /// Создаёт конфигурацию приложения, загружая её из секции с названием <paramref name="sectionName"/>.
        /// </summary>
        /// <param name="sectionName">Название секции, в которой содержатся настройки.</param>
        public ConfigurationModel(string sectionName)
        {
            factories = new Dictionary<string, IProviderFactory>();
            LoadFactoriesSafe((ReportHelperConfigurationSection) ConfigurationManager.GetSection(sectionName));
        }

        /// <summary>
        /// Название выбранного провайдера.
        /// </summary>
        static string CurrentFactoryName
        {
            get { return Properties.Settings.Default.CurrentProviderName; }
            set { Properties.Settings.Default.CurrentProviderName = value; }
        }

        /// <summary>
        /// Создаёт модель для редактирования конфигурации.
        /// </summary>
        public ConfigurationEditorModel CreateEditorModel()
        {
            return new ConfigurationEditorModel(factories, CurrentFactoryName);
        }

        /// <summary>
        /// Обновляет текущую конфигурацию на основании отредактированной модели.
        /// </summary>
        public void Update(ConfigurationEditorModel model)
        {
            if (!factories.ContainsKey(model.SelectedProviderName))
                throw new ArgumentException("invalid provider name: " + model.SelectedProviderName);
            CurrentFactoryName = model.SelectedProviderName;
            factories[CurrentFactoryName].ApplyConfiguration(model.ProviderSettingsModel);
        }

        /// <summary>
        /// Безопасно с точки зрения исключений загружает все описанные в конфигурации провайдеры каталогов.
        /// </summary>
        void LoadFactoriesSafe(ReportHelperConfigurationSection section)
        {
            foreach (ProviderFactoryConfigurationElement providerElement in section.ProviderFactories)
                LoadOneSafe(providerElement);
        }

        /// <summary>
        /// Безопасно с точки зрения исключений загружает описание провайдера.
        /// </summary>
        void LoadOneSafe(ProviderFactoryConfigurationElement factoryElement)
        {
            if (!typeof(IProviderFactory).IsAssignableFrom(factoryElement.Class))
                return;
            var instance = (IProviderFactory) Activator.CreateInstance(factoryElement.Class);
            factories.Add(factoryElement.Name, instance);
        }

        /// <summary>
        /// Возвращает true, если в конфигурации содержится выбранный провайдер, иначе false.
        /// </summary>
        public bool HasConfiguredProvider
        {
            get { return !string.IsNullOrWhiteSpace(CurrentFactoryName) && factories.ContainsKey(CurrentFactoryName); }
        }

        /// <summary>
        /// Возвращает стратегию выбранного провайдера.
        /// </summary>
        public ICatalogProviderStrategy GetConfiguredProviderStrategy()
        {
            IProviderFactory f;
            if(string.IsNullOrWhiteSpace(CurrentFactoryName)||!factories.TryGetValue(CurrentFactoryName, out f))
                throw new InvalidOperationException();
            return f.CreateStrategy();
        }
    }
}
