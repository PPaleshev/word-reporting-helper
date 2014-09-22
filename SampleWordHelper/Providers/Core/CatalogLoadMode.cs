namespace SampleWordHelper.Providers.Core
{
    /// <summary>
    /// Режим загрузки каталога.
    /// </summary>
    public enum CatalogLoadMode
    {
        /// <summary>
        /// Требуется полное сканирование структуры каталога с индексацией содержимого.
        /// </summary>
        FULL,

        /// <summary>
        /// Возможно частичное обновление структуры каталога или загрузка его из индексированного хранилища.
        /// </summary>
        PARTIAL
    }
}
