namespace SampleWordHelper.Indexation
{
    /// <summary>
    /// Интерфейс механизма для поиска данных в индексированном содержимом документов.
    /// </summary>
    public interface ISearchEngine
    {
        /// <summary>
        /// Выполняет поиск по запросу в текущем индексе.
        /// </summary>
        /// <param name="input">Поисковой запрос.</param>
        /// <returns>Возвращает массив идентификаторов документов, в которых были найдены совпадения с оригиналом, в порядке увеличения количества совпадений.</returns>
        string[] Search(string input);
    }
}
