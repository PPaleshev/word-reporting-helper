namespace SampleWordHelper.Providers.FileSystem
{
    /// <summary>
    /// Фасад для загрузки настроек.
    /// </summary>
    static class SettingsFacade
    {
        /// <summary>
        /// Загружает объект настроек фабрики поставщика каталога файловой системы.
        /// </summary>
        public static ProviderSettings LoadSettings()
        {
            var settingXml = Properties.Settings.Default.FileSystemProviderConfiguration;
            var settings = !string.IsNullOrWhiteSpace(settingXml) ? ProviderSettings.Deserialize(settingXml) : new ProviderSettings();
            return settings;
        }

        /// <summary>
        /// Сохраняет настройки провайдера.
        /// </summary>
        public static void SaveSettings(ProviderSettings settings)
        {
            Properties.Settings.Default.FileSystemProviderConfiguration = settings.Serialize();
            Properties.Settings.Default.Save();
        }
    }
}