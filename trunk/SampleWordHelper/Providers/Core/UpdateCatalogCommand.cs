using System;
using SampleWordHelper.Model;

namespace SampleWordHelper.Providers.Core
{
    public class UpdateCatalogCommand
    {
        readonly CatalogLoadMode mode;
        readonly ICatalogProviderStrategy provider;
        readonly ICatalogCallback callback;

        public UpdateCatalogCommand(ICatalogProviderStrategy provider, CatalogLoadMode mode, ICatalogCallback callback)
        {
            this.provider = provider;
            this.mode = mode;
            this.callback = callback;
        }

        public void Execute()
        {
            try
            {
                callback.OnStarting();
                var catalog = provider.LoadCatalog(mode);
                callback.OnSuccess(catalog);
            }
            catch (Exception e)
            {
                callback.OnError(e);
            }
        }
    }

    public interface ICatalogCallback
    {
        /// <summary>
        /// Вызывается перед началом операции.
        /// </summary>
        void OnStarting();

        /// <summary>
        /// Вызывается при успешной загрузке каталога.
        /// </summary>
        /// <param name="catalog">Загруженная модель каталога.</param>
        void OnSuccess(CatalogModel catalog);

        /// <summary>
        /// Вызывается при ошибке загрузки каталога.
        /// </summary>
        /// <param name="e">Исключение, возникшее при обновлении.</param>
        void OnError(Exception e);
    }
}