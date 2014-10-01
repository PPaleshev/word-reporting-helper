using System;
using SampleWordHelper.Core;
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

        /// <summary>
        /// Кэш с файлами для предотвращения доступа по длинным именам.
        /// </summary>
        readonly LocalCache localCache;

        public ProviderStrategy(ProviderSettings settings)
        {
            this.settings = settings;
            localCache = new LocalCache("", settings.RootDirectory, false);
        }

        public bool Initialize(IRuntimeContext context)
        {
            var model = new SettingsModel(settings);
            return model.Validate().IsValid;
        }

        public ICatalog LoadCatalog()
        {
            var catalog = new Catalog();
            localCache.Clear();
            var builder = new CatalogBuilder2(settings.RootDirectory, settings.MaterializeEmptyPaths, localCache);
            builder.Build(catalog);
            localCache.Materialize();
            return catalog;
        }

        public void Shutdown()
        {
            localCache.Clear();
        }
    }
}