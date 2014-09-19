namespace SampleWordHelper.Model
{
    /// <summary>
    /// Интерфейс основной модели.
    /// </summary>
    public interface IDocumentModel
    {
        /// <summary>
        /// Флаг, равный true, если структура каталога видна, иначе false.
        /// </summary>
        bool IsStructureVisible { get; }
    }
}
