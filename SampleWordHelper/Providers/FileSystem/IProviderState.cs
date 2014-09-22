using System;
using System.IO;
using SampleWordHelper.Model;
using SampleWordHelper.Providers.Core;

namespace SampleWordHelper.Providers.FileSystem
{
    /// <summary>
    /// Интерфейс состояния поставщика каталога.
    /// </summary>
    public interface IProviderState
    {
        void Initialize();
        void Shutdown();
        CatalogModel LoadCatalog(CatalogLoadMode mode);
    }

    public class InactiveState : IProviderState
    {
        public void Initialize()
        {
            throw new InvalidOperationException("unable to initialize inactive state");
        }

        public CatalogModel LoadCatalog(CatalogLoadMode mode)
        {
            throw new InvalidOperationException("unable to load catalog in inactive state");
        }

        public void Shutdown()
        {
            throw new InvalidOperationException("unable to shutdown inactive state");
        }
    }

    /// <summary>
    /// Активное состояние провайдера.
    /// </summary>
    public class ActiveState : IProviderState
    {
        const string FILTER = "*.docx";

        /// <summary>
        /// Параметры работы текущего провайдера.
        /// </summary>
        readonly FileSystemProviderSettings settings;

        public ActiveState(FileSystemProviderSettings settings)
        {
            this.settings = settings;
        }

        public void Initialize()
        {
        }

        public CatalogModel LoadCatalog(CatalogLoadMode mode)
        {
            var catalog = new CatalogModel();
            var root = new DirectoryInfo(settings.RootPath);
            var rootUri = new Uri(root.FullName);
            catalog.AddGroup(root.FullName, null, root.Name);
            var items = root.GetFiles(FILTER);
            foreach (var file in items)
            {
                var id = GetRelativePath(rootUri, file.FullName);
                var parentId = GetRelativePath(rootUri, file.DirectoryName + "\\");
                catalog.AddGroup();
            }
            return catalog;
        }

        static string GetRelativePath(Uri root, string current)
        {
            var uri = new Uri(current);
            var relUri = root.MakeRelativeUri(uri);
            return Uri.UnescapeDataString(relUri.OriginalString);
        }

        public void Shutdown()
        {
        }
    }
}