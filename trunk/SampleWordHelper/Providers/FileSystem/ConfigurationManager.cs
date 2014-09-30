using System;
using SampleWordHelper.Providers.Core;

namespace SampleWordHelper.Providers.FileSystem
{
    /// <summary>
    /// Экземпляр для управления конфигурацией поставщика файловой системы.
    /// </summary>
    public class ConfigurationManager: IConfigurationManager
    {
        public ISettingsModel CreateSettingsModel()
        {
            var settingXml = Properties.Settings.Default.FileSystemProviderConfiguration;
            var settings = !string.IsNullOrWhiteSpace(settingXml) ? FileSystemProviderSettings.Deserialize(settingXml) : new FileSystemProviderSettings();
            return new FileSystemProviderSettingsModel(settings);
        }

        public void ApplyConfiguration(ISettingsModel configuration)
        {
            var model = configuration as FileSystemProviderSettingsModel;
            if (model == null)
                throw new ArgumentException("invalid settings model object type");
            var settings = model.CreateSettingsObject();
            Properties.Settings.Default.FileSystemProviderConfiguration = settings.Serialize();
            Properties.Settings.Default.Save();
        }
    }
}
