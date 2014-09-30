using System.IO;
using SampleWordHelper.Core;
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

        public CatalogModel LoadCatalog(CatalogLoadMode mode)
        {
            var catalog = new CatalogModel();
            var builder = new CatalogBuilder2(settings.RootPath, settings.MaterializeEmptyPaths);
            builder.Build(catalog);
            return catalog;
        }

        public void Shutdown()
        {
        }
    }
}