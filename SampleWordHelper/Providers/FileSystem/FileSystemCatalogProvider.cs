using System;
using SampleWordHelper.Core;
using SampleWordHelper.Model;
using SampleWordHelper.Providers.Core;

namespace SampleWordHelper.Providers.FileSystem
{
    public class FileSystemCatalogProvider: ICatalogProvider
    {
        /// <summary>
        /// Текущее состояние поставщика каталога.
        /// </summary>
        IProviderState state = new InactiveState();

        public ISettingsModel CreateSettingsModel()
        {
            return LoadSettings();
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

        public bool Initialize(IRuntimeContext context)
        {
            var model = LoadSettings();
            if (!model.Validate().IsValid)
                return false;
            state = new ActiveState(model.CreateSettingsObject());
            state.Initialize();
            return true;
        }

        public CatalogModel LoadCatalog(CatalogLoadMode mode)
        {
            return state.LoadCatalog(mode);
        }

        public void Shutdown()
        {
            try
            {
                state.Shutdown();
            }
            finally
            {
                state = new InactiveState();
            }
        }

        /// <summary>
        /// Загружает объект настроек приложения.
        /// </summary>
        static FileSystemProviderSettingsModel LoadSettings()
        {
            var settingXml = Properties.Settings.Default.FileSystemProviderConfiguration;
            var settings = !string.IsNullOrWhiteSpace(settingXml) ? FileSystemProviderSettings.Deserialize(settingXml) : new FileSystemProviderSettings();
            return new FileSystemProviderSettingsModel(settings);
        }
    }
}
