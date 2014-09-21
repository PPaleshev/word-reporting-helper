using System;
using System.Configuration;
using SampleWordHelper.Configuration;
using SampleWordHelper.Providers.Core;

namespace SampleWordHelper.Providers.FileSystem
{
    public class FileSystemCatalogProvider: ICatalogProvider
    {
        const string SECTION_NAME = "fileSystemProvider";

        public ISettingsModel CreateSettingsModel()
        {
            var model = new Settings();
            var section = (FileSystemProviderConfigurationSection) ConfigurationManager.GetSection(SECTION_NAME);
            if (section != null)
                model.LoadFrom(section);
            return model;
        }

        public void ApplyConfiguration(ISettingsModel configuration)
        {
            var model = configuration as Settings;
            if (model == null)
                throw new ArgumentException("invalid settings model object type");
            var section = (FileSystemProviderConfigurationSection) ConfigurationManager.GetSection(SECTION_NAME) ?? new FileSystemProviderConfigurationSection();
            model.SaveTo(section);
            section.CurrentConfiguration.Save();
        }
    }
}
