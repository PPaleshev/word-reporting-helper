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
        /// Фабрика контейнеров для панелей задач.
        /// </summary>
        readonly CustomTaskPaneCollection paneFactory;

        /// <summary>
        /// Создаёт экземпляр фабрики представлений.
        /// </summary>
        /// <param name="ribbon">Экземпляр ленты с элементами управления.</param>
        /// <param name="paneFactory">Фабрика контейнеров для панелей задач.</param>
        public ViewFactory(MainRibbon ribbon, CustomTaskPaneCollection paneFactory)
        {
            this.ribbon = ribbon;
            this.paneFactory = paneFactory;
        }

        public IMainView CreateMainView(IMainPresenter presenter)
        {
            return new MainView(ribbon, presenter);
        }

        public IDocumentView CreateStructureView(ICatalogPresenter presenter, string title)
        {
            var control = new StructureTreeControl();
            var container = paneFactory.Add(control, title);
            return new StructureTreeView(container, presenter);
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
    }
}
