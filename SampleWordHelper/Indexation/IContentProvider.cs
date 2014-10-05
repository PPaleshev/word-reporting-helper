namespace SampleWordHelper.Indexation
{
    /// <summary>
    /// Поставщик содержимого для индексации
    /// </summary>
    public interface IContentProvider
    {
        /// <summary>
        /// Пытается прочитать содержимое файла по указанному пути.
        /// </summary>
        /// <param name="filePath">Путь к файлу.</param>
        /// <param name="content">Прочитанное содержимое, если метод вернул true.</param>
        /// <returns>Возвращает true, если содержимое было прочитано, иначе false.</returns>
        bool TryGetContent(string filePath, out string content);
    }
}
