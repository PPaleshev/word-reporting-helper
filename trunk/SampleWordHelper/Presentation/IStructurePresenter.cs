using System;

namespace SampleWordHelper.Presentation
{
    /// <summary>
    /// Интерфейс менеджера представления структуры каталога.
    /// </summary>
    public interface IStructurePresenter
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
    }
}
