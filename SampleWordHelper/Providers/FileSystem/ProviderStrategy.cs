using SampleWordHelper.Core.Application;
using SampleWordHelper.Model;
using SampleWordHelper.Providers.Core;

namespace SampleWordHelper.Providers.FileSystem
{
    /// <summary>
    /// Реализация стратегии доступа к каталогу, хранящемуся в файловой системе.
    /// </summary>
    public class ProviderStrategy : ICatalogProviderStrategy
    {
        /// <summary>
        /// Настройки провайдера каталога файловой системы.
        /// </summary>
        readonly ProviderSettings settings;

        public ProviderStrategy(ProviderSettings settings)
        {
            this.settings = settings;
        }

        public bool Initialize(IRuntimeContext context)
        {
            var model = new SettingsModel(settings);
            return model.Validate().IsValid;
        }

        public ICatalog LoadCatalog()
        {
            var catalog = new Catalog();
            var builder = new CatalogBuilder2(settings.RootDirectory, settings.MaterializeEmptyPaths);
            builder.Build(catalog);
            return catalog;
        }

        public void Shutdown()
        {
        }
    }
}