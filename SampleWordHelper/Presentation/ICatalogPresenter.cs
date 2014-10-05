namespace SampleWordHelper.Presentation
{
    /// <summary>
    /// Интерфейс менеджера представления каталога.
    /// </summary>
    public interface ICatalogPresenter
    {
        /// <summary>
        /// Объект для управления операциями перетаскивания.
        /// </summary>
        IDragSourceController DragController { get; }

        /// <summary>
        /// Вызывается при закрытии панели структуры каталога.
        /// </summary>
        void OnClosed();

        /// <summary>
        /// Вызывается при двойном клике на узел дерева.
        /// </summary>
        /// <param name="item">Выбранный элемент.</param>
        void OnNodeDoubleClicked(object item);

        /// <summary>
        /// Вызывается при изменении поискового фильтра.
        /// </summary>
        /// <param name="filterText">Текст фильтра.</param>
        void OnFilterTextChanged(string filterText);
    }
}
