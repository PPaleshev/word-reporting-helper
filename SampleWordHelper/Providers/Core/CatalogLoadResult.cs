using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using SampleWordHelper.Model;

namespace SampleWordHelper.Providers.Core
{
    /// <summary>
    /// Результат загрузки каталога.
    /// </summary>
    public class CatalogLoadResult
    {
        /// <summary>
        /// Флаг, равный true, если результат каталога содержит ошибки.
        /// </summary>
        public bool IsError { get; private set; }

        /// <summary>
        /// Экземпляр каталога.
        /// </summary>
        public ICatalog Catalog { get; private set; }

        /// <summary>
        /// Коллекция ошибок.
        /// </summary>
        public ReadOnlyCollection<ElementValidationInfo> Errors { get; private set; }

        public CatalogLoadResult(ICatalog catalog, IEnumerable<ElementValidationInfo> errors)
        {
            Catalog = catalog;
            Errors = new ReadOnlyCollection<ElementValidationInfo>((errors ?? Enumerable.Empty<ElementValidationInfo>()).ToArray());
            IsError = Errors.Any(info => info.IsError);
        }
    }
}
