using System;
using System.IO;
using SampleWordHelper.Core;
using SampleWordHelper.Model;
using SampleWordHelper.Providers.Core;

namespace SampleWordHelper.Providers.FileSystem
{
    /// <summary>
    /// Реализация стратегии провайдера каталога, основанного на расположении файлов в файловой системе.
    /// Директория считается группой, файл - конечным элементом.
    /// </summary>
    public class FileSystemProvider : IProviderStrategy, ICatalogStrategy
    {
        public IConfigurationManager ConfigurationManager { get; private set; }

        public ICatalogStrategy CatalogStrategy { get; private set; }

        public FileSystemProvider()
        {
            ConfigurationManager = new ConfigurationManager();
        }

        public bool Initialize(IRuntimeContext context, bool reinitialize)
        {
            var model = ConfigurationManager.CreateSettingsModel();
            if (!model.Validate().IsValid)
                return false;
            //Some logic here
            return true;
        }


        public void Shutdown()
        {
        }

        public CatalogModel LoadCatalog(CatalogLoadMode mode)
        {
            var builder = new CatalogBuilder(new DirectoryInfo(settings.RootPath));
            var catalog = new CatalogModel();
            builder.BuildCatalog(catalog);
            return catalog;
        }
    }
}
