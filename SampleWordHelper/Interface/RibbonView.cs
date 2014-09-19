using Microsoft.Office.Tools.Ribbon;
using SampleWordHelper.Core;
using SampleWordHelper.Presentation;

namespace SampleWordHelper.Interface
{
    /// <summary>
    /// Представление ленты с основными элементами управления надстройкой.
    /// </summary>
    public class RibbonView : IRibbonView
    {
        /// <summary>
        /// Основной элемент управления ленты.
        /// </summary>
        readonly ReportingRibbon ribbon;

        /// <summary>
        /// Менеджер представления.
        /// </summary>
        readonly IRibbonPresenter presenter;

        /// <summary>
        /// Флаг для предотвращения обратных вызовов при обработке событий.
        /// </summary>
        readonly SuspendFlag suspendFlag = new SuspendFlag();

        /// <summary>
        /// Создаёт новый экземпляр представления.
        /// </summary>
        public RibbonView(ReportingRibbon ribbon, IRibbonPresenter presenter)
        {
            this.ribbon = ribbon;
            this.presenter = presenter;
            SetupEvents();
        }

        public void SetStructureVisible(bool value)
        {
            if (!suspendFlag)
                ribbon.toggleStructureVisibility.Checked = value;
        }

        public void Dispose()
        {
            ribbon.toggleStructureVisibility.Click -= OnToggleStructureClicked;
            ribbon.group1.DialogLauncherClick -= OnDialogLauncherClicked;
        }

        void SetupEvents()
        {
            ribbon.toggleStructureVisibility.Click += OnToggleStructureClicked;
            ribbon.group1.DialogLauncherClick += OnDialogLauncherClicked;
        }

        void OnDialogLauncherClicked(object sender, RibbonControlEventArgs e)
        {
            presenter.OnShowSettings();
        }

        void OnToggleStructureClicked(object sender, RibbonControlEventArgs e)
        {
            using (suspendFlag.Suspend())
                presenter.OnToggleStructureVisibility();
        }
    }
}
