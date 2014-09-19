using System.Windows.Forms;
using Microsoft.Office.Tools;
using SampleWordHelper.Interface;
using SampleWordHelper.Presentation;

namespace SampleWordHelper.Core
{
    /// <summary>
    /// Интерфейс фабрики представлений.
    /// </summary>
    public interface IViewFactory
    {
        /// <summary>
        /// Создаёт представление для отображения ленты.
        /// </summary>
        /// <param name="presenter">Менеджер представления.</param>
        IRibbonView CreateRibbonView(IRibbonPresenter presenter);

        /// <summary>
        /// Создаёт представление для просмотра структуры каталога.
        /// </summary>
        /// <param name="presenter">Менеджер представления.</param>
        IStructureView CreateStructureView(IStructurePresenter presenter);

        /// <summary>
        /// Создаёт экземпляр панели задач, в которой размещается пользовательский элемент управления.
        /// </summary>
        /// <param name="control">Пользовательский элемент управления.</param>
        /// <param name="title">Заголовок контейнера, не может быть пустым.</param>
        CustomTaskPane CreateTaskPaneContainer(UserControl control, string title);
    }
}
