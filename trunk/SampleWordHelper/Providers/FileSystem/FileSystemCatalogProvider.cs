using System;
using SampleWordHelper.Core;
using SampleWordHelper.Providers.Core;

namespace SampleWordHelper.Providers.FileSystem
{
    public class FileSystemCatalogProvider: ICatalogProvider
    {
        public ISettingsModel CreateSettingsModel()
        {
            var model = new FileSystemProviderSettingsModel();
            model.LoadFrom(LoadSettings());
            return model;
        }

        public void ApplyConfiguration(ISettingsModel configuration)
        {
            var model = configuration as FileSystemProviderSettingsModel;
            if (model == null)
                throw new ArgumentException("invalid settings model object type");
            var settings = new FileSystemProviderSettings();
            model.SaveTo(settings);
            Properties.Settings.Default.FileSystemProviderConfiguration = settings.Serialize();
            Properties.Settings.Default.Save();
        }

        public void Initialize(IRuntimeContext context)
        {
        }

        public void Shutdown()
        {
        }

        /// <summary>
        /// Загружает объект настроек приложения.
        /// </summary>
        static FileSystemProviderSettings LoadSettings()
        {
            var settingXml = Properties.Settings.Default.FileSystemProviderConfiguration;
            return !string.IsNullOrWhiteSpace(settingXml) ? FileSystemProviderSettings.Deserialize(settingXml) : new FileSystemProviderSettings();
        }
    }
}
