using System;
using SampleWordHelper.Providers.Core;

namespace SampleWordHelper.Providers.FileSystem
{
    /// <summary>
    /// Фабрика поставщиков для доступа к каталогу посредством файловой системы.
    /// </summary>
    public class FileSystemProviderFactory : IProviderFactory
    {
        /// <summary>
        /// Описание фабрики.
        /// </summary>
        static readonly ProviderDescription DESCRIPTION = new ProviderDescription("Файловая система",
            "Предоставляет доступ к структурированной базе знаний, хранящейся в файловой системе локально или на сетевом ресурсе");

        public ProviderDescription GetDescription()
        {
            return DESCRIPTION;
        }

        public ISettingsModel CreateSettingsModel()
        {
            return new SettingsModel(SettingsFacade.LoadSettings());
        }

        public void ApplyConfiguration(ISettingsModel configuration)
        {
            var model = configuration as SettingsModel;
            if (model == null)
                throw new ArgumentException("invalid settings model object type");
            SettingsFacade.SaveSettings(model.CreateSettingsObject());
        }

        public ICatalogProviderStrategy CreateStrategy()
        {
            return new ProviderStrategy(SettingsFacade.LoadSettings());
        }
    }
}
