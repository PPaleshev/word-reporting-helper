using System.Windows.Forms;
using Microsoft.Office.Tools;
using SampleWordHelper.Interface;
using SampleWordHelper.Presentation;

namespace SampleWordHelper.Core
{
    /// <summary>
    /// Экземпляр фабрики представлений.
    /// </summary>
    public class ViewFactory: IViewFactory
    {
        /// <summary>
        /// Экземпляр ленты, которая делится между всеми экземплярами окон.
        /// </summary>
        readonly ReportingRibbon ribbon;

        /// <summary>
        /// Фабрика контейнеров для панелей задач.
        /// </summary>
        readonly CustomTaskPaneCollection paneFactory;

        /// <summary>
        /// Создаёт экземпляр фабрики представлений.
        /// </summary>
        /// <param name="ribbon">Экземпляр ленты с элементами управления.</param>
        /// <param name="paneFactory">Фабрика контейнеров для панелей задач.</param>
        public ViewFactory(ReportingRibbon ribbon, CustomTaskPaneCollection paneFactory)
        {
            this.ribbon = ribbon;
            this.paneFactory = paneFactory;
        }

        public IRibbonView CreateRibbonView(IRibbonPresenter presenter)
        {
            return new RibbonView(ribbon, presenter);
        }

        public IStructureView CreateStructureView(IStructurePresenter presenter)
        {
            return new StructureTreeControl(presenter);
        }

        public CustomTaskPane CreateTaskPaneContainer(UserControl control, string title)
        {
            return paneFactory.Add(control, title);
        }
    }
}
