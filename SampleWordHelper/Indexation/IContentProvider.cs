namespace SampleWordHelper.Indexation
{
    /// <summary>
    /// Интерфейс поставщика содержимого документов.
    /// </summary>
    public interface IContentProvider
    {
        /// <summary>
        /// Пытается извлечь содержимое файла <paramref name="fileName"/>.
        /// </summary>
        /// <param name="fileName">Путь к файлу, содержимое которого требуется получить.</param>
        /// <param name="content">Извлечённое содержимое документа.</param>
        /// <returns>Возвращает true, если содержимое было успешно извлечено, иначе false.</returns>
        bool TryGetContent(string fileName, out string content);
    }
}
