using System.Windows.Forms;
using SampleWordHelper.Core.Application;
using SampleWordHelper.Model;

namespace SampleWordHelper.Interface
{
    /// <summary>
    /// Интерфейс представления структуры каталога.
    /// </summary>
    public interface IDocumentView : IView
    {
        /// <summary>
        /// Устанавливает ширину панели структуры.
        /// </summary>
        /// <param name="width"></param>
        void SetWidth(int width);

        /// <summary>
        /// Устанавливает контекстное меню.
        /// </summary>
        void SetContextMenu(ICommandView menu);

        /// <summary>
        /// Показывает\скрывает представление в зависимости от переданного флага.
        /// </summary>
        void SetVisibility(bool value);

        /// <summary>
        /// Начинает перетаскивание выбранного узла в дереве.
        /// В качестве перетаскиваемых данных устанавливается <paramref name="dragData"/>.
        /// </summary>
        /// <param name="dragData">Объект с передаваемыми данными.</param>
        /// <param name="effect">Требуемый эффект для перетаскивания.</param>
        void BeginDragNode(object dragData, DragDropEffects effect);

        /// <summary>
        /// Обновляет структуру каталога.
        /// </summary>
        void UpdateStructure(DocumentModel model);

        /// <summary>
        /// Устанавливает текст фильтра.
        /// </summary>
        void SetFilterText(string filterText);
    }
}
