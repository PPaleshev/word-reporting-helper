namespace SampleWordHelper.Presentation
{
    /// <summary>
    /// Интерфейс для выполнения обратных вызовов из превью.
    /// </summary>
    public interface IPreviewCallback
    {
        /// <summary>
        /// Вызывается для вставки содержимого просматриваемого элемента в редактируемый документ.
        /// </summary>
        /// <param name="fileName">Путь к файлу.</param>
        void OnPaste(string fileName);
    }
}
