namespace SampleWordHelper.Presentation
{
    /// <summary>
    /// Интерфейс обратного вызова при закрытии панели каталога.
    /// </summary>
    public interface ICatalogPaneCallback
    {
        /// <summary>
        /// Вызывается для управления кнопкой "Показать\скрыть каталог" в главном представлении.
        /// </summary>
        /// <param name="visible">True, если панель отображается, в противном случае false.</param>
        void OnVisibilityChanged(bool visible);
    }
}
