using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net.NetworkInformation;
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
            var builder = new CatalogBuilder(new DirectoryInfo(settings.RootPath));
            var catalog = new CatalogModel();
            builder.BuildCatalog(catalog);
            return catalog;
        }

        public void Shutdown()
        {
        }
    }
}