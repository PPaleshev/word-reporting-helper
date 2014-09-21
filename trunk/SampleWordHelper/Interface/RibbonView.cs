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
        readonly MainRibbon ribbon;

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
        public RibbonView(MainRibbon ribbon, IRibbonPresenter presenter)
        {
            this.ribbon = ribbon;
            this.presenter = presenter;
            SetupEvents();
        }

        public void SetValid(bool isValid)
        {
            ribbon.buttonShowErrors.Visible = !isValid;
            ribbon.toggleStructureVisibility.Visible = isValid;
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
            ribbon.buttonShowErrors.Click -= OnShowErrors;
        }

        /// <summary>
        /// Выполняет подписку на события.
        /// </summary>
        void SetupEvents()
        {
            ribbon.buttonShowErrors.Click += OnShowErrors;
            ribbon.toggleStructureVisibility.Click += OnToggleStructureClicked;
            ribbon.group1.DialogLauncherClick += OnDialogLauncherClicked;
        }

        /// <summary>
        /// Вызывается для отображения ошибок.
        /// </summary>
        void OnShowErrors(object sender, RibbonControlEventArgs e)
        {
            presenter.OnShowErrors();
        }

        /// <summary>
        /// Вызывается при нажатии на кнопке настроек.
        /// </summary>
        void OnDialogLauncherClicked(object sender, RibbonControlEventArgs e)
        {
            presenter.OnShowSettings();
        }

        /// <summary>
        /// Вызывается при нажатии на кнопке переключения видимости панели структуры.
        /// </summary>
        void OnToggleStructureClicked(object sender, RibbonControlEventArgs e)
        {
            using (suspendFlag.Suspend())
                presenter.OnToggleStructureVisibility();
        }
    }
}
