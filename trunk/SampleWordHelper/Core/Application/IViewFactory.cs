using SampleWordHelper.Interface;
using SampleWordHelper.Presentation;

namespace SampleWordHelper.Core.Application
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
        /// Создаёт представление для просмотра структуры каталога.
        /// </summary>
        /// <param name="presenter">Менеджер представления.</param>
        /// <param name="title">Заголовок отображаемого представления.</param>
        IDocumentView CreateStructureView(ICatalogPresenter presenter, string title);

        /// <summary>
        /// Создаёт представление для редактирования параметров надстройки.
        /// </summary>
        IConfigurationEditorView CreateSettingsView(IConfigurationEditorPresenter presenter);

        /// <summary>
        /// Создаёт представление для ожидания.
        /// </summary>
        IWaitingView CreateWaitingView();

        /// <summary>
        /// Создаёт представление для приёма данных при перетаскивании.
        /// </summary>
        /// <param name="presenter">Менеджер представления.</param>
        IDropTargetHost CreateDropHost(IDropTargetPresenter presenter);

        /// <summary>
        /// Создаёт представление для предварительного просмотра документов.
        /// </summary>
        /// <param name="presenter">Менеджер представления.</param>
        IPreviewView CreatePreviewView(PreviewPresenter presenter);
    }
}
