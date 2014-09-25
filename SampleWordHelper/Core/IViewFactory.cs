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
        /// Создаёт экземпляр основного представления.
        /// </summary>
        /// <param name="presenter">Менеджер представления.</param>
        IMainView CreateMainView(IMainPresenter presenter);

        /// <summary>
        /// Создаёт представление для отображения ленты.
        /// </summary>
        /// <param name="presenter">Менеджер представления.</param>
        IRibbonView CreateRibbonView(IRibbonEventListener presenter);

        /// <summary>
        /// Создаёт представление для просмотра структуры каталога.
        /// </summary>
        /// <param name="presenter">Менеджер представления.</param>
        /// <param name="title">Заголовок отображаемого представления.</param>
        IDocumentView CreateStructureView(IStructurePresenter presenter, string title);

        /// <summary>
        /// Создаёт представление для редактирования параметров надстройки.
        /// </summary>
        IConfigurationEditorView CreateSettingsView(IConfigurationEditorPresenter presenter);

        /// <summary>
        /// Создаёт представление для приёма данных при перетаскивании.
        /// </summary>
        /// <param name="presenter">Менеджер представления.</param>
        IDropTargetHost CreateDropHost(IDropTargetPresenter presenter);
    }
}
