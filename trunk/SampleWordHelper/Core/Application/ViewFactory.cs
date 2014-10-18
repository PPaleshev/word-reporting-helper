using Microsoft.Office.Tools;
using SampleWordHelper.Interface;
using SampleWordHelper.Presentation;

namespace SampleWordHelper.Core.Application
{
    /// <summary>
    /// Экземпляр фабрики представлений.
    /// </summary>
    public class ViewFactory: IViewFactory
    {
        /// <summary>
        /// Экземпляр ленты, которая делится между всеми экземплярами окон.
        /// </summary>
        readonly MainRibbon ribbon;

        /// <summary>
        /// Фабрика панелей задач.
        /// </summary>
        readonly TaskPaneFactory paneFactory;

        /// <summary>
        /// Объект, предоставляющий доступ к главному окну приложения.
        /// </summary>
        readonly IWindowProvider windowProvider;

        /// <summary>
        /// Создаёт экземпляр фабрики представлений.
        /// </summary>
        /// <param name="ribbon">Экземпляр ленты с элементами управления.</param>
        /// <param name="paneFactory">Фабрика контейнеров для панелей задач.</param>
        /// <param name="windowProvider">Объект, предоставляющий доступ к главному окну приложения.</param>
        public ViewFactory(MainRibbon ribbon, CustomTaskPaneCollection paneFactory, IWindowProvider windowProvider)
        {
            this.ribbon = ribbon;
            this.paneFactory = new TaskPaneFactory(paneFactory);
            this.windowProvider = windowProvider;
        }

        public IMainView CreateMainView(IMainPresenter presenter)
        {
            return new MainView(ribbon, presenter);
        }

        public IDocumentView CreateStructureView(ICatalogPresenter presenter, string title)
        {
            return new StructureTreeView(paneFactory, presenter, title);
        }

        public IConfigurationEditorView CreateSettingsView(IConfigurationEditorPresenter presenter)
        {
            return new ConfigurationEditorForm(presenter);
        }

        public IWaitingView CreateWaitingView()
        {
            return new WaitingForm();
        }

        public IDropTargetHost CreateDropHost(IDropTargetPresenter presenter)
        {
            return new OverlayWindow(presenter);
        }

        public IPreviewView CreatePreviewView(IPreviewPresenter presenter)
        {
            return new PreviewWindow(presenter, windowProvider);
        }

        public ICommandView CreateCommandView()
        {
            return new CommandView();
        }
    }
}
