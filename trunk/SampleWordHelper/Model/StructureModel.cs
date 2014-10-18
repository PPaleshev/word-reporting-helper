namespace SampleWordHelper.Model
{
    /// <summary>
    /// Тип узла.
    /// </summary>
    public enum NodeType
    {
        /// <summary>
        /// Тип не определён.
        /// </summary>
        NONE,

        /// <summary>
        /// Группа.
        /// </summary>
        GROUP,

        /// <summary>
        /// Листовой узел.
        /// </summary>
        LEAF,

        /// <summary>
        /// Элемент, обнаруженный при индексированном поиске.
        /// </summary>
        INDEXED_SEARCH
    }
}
