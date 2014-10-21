using System;

namespace SampleWordHelper.Providers.Core
{
    /// <summary>
    /// Исключение, возникшее при загрузке каталога.
    /// </summary>
    public class CatalogLoadException : ApplicationException 
    {
        public CatalogLoadException(string message) : base(message)
        {
        }

        public CatalogLoadException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}