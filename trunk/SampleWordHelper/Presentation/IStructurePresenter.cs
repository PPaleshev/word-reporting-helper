namespace SampleWordHelper.Presentation
{
    /// <summary>
    /// Интерфейс менеджера представления структуры каталога.
    /// </summary>
    public interface IStructurePresenter
    {
        /// <summary>
        /// Вызывается при закрытии панели структуры каталога.
        /// </summary>
        void OnClosed();
    }
}
