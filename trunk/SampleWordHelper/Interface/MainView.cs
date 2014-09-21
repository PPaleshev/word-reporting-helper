using System;
using Microsoft.Office.Core;
using Microsoft.Office.Tools.Ribbon;
using SampleWordHelper.Core;
using SampleWordHelper.Presentation;
using CustomTaskPane = Microsoft.Office.Tools.CustomTaskPane;

namespace SampleWordHelper.Interface
{
    /// <summary>
    /// Основное представление.
    /// </summary>
    public class MainView : IMainView
    {
        /// <summary>
        /// Экземпляр ленты.
        /// </summary>
        readonly MainRibbon ribbon;

        /// <summary>
        /// Экземпляр пользовательского контрола для отображения структуры каталога.
        /// </summary>
        readonly StructureTreeControl structureTree;

        /// <summary>
        /// Контейнер, в котором находится панель структуры.
        /// </summary>
        readonly CustomTaskPane paneContainer;

        /// <summary>
        /// Флаг для приостановки событий.
        /// </summary>
        readonly SuspendFlag suspendFlag = new SuspendFlag();

        /// <summary>
        /// Менеджер текущего представления.
        /// </summary>
        IMainPresenter presenter;

        /// <summary>
        /// Создаёт новый экземпляр представления.
        /// </summary>
        public MainView(MainRibbon ribbon, CustomTaskPane paneContainer)
        {
            this.ribbon = ribbon;
            structureTree = (StructureTreeControl) paneContainer.Control;
            this.paneContainer = paneContainer;
        }

        /// <summary>
        /// Выполняет инициализацию представления.
        /// </summary>
        public void Setup(IMainPresenter presenter)
        {
            this.presenter = presenter;
            ribbon.toggleStructureVisibility.Click += OnStructureVisibilityClick;
            ribbon.group1.DialogLauncherClick += SettingsLauncherDialogClick;
            paneContainer.VisibleChanged += OnPaneVisibilityChanged;
        }

        public void SetStructureVisibility(bool value)
        {
            using (suspendFlag.Suspend())
            {
                ribbon.toggleStructureVisibility.Checked = value;
                paneContainer.Visible = value;
            }
        }

        /// <summary>
        /// Вызывается при закрытии панели структуры.
        /// </summary>
        void OnPaneVisibilityChanged(object sender, EventArgs e)
        {
            if(!suspendFlag)
                presenter.OnToggleStructureVisibility();
        }

        /// <summary>
        /// Вызывается при нажатии на кнопку скрытия\отображения панели структуры.
        /// </summary>
        void OnStructureVisibilityClick(object sender, RibbonControlEventArgs e)
        {
            if (!suspendFlag)
                presenter.OnToggleStructureVisibility();
        }

        /// <summary>
        /// Вызывается при открытии диалога редактирования настроек подсистемы.
        /// </summary>
        void SettingsLauncherDialogClick(object sender, RibbonControlEventArgs e)
        {
            presenter.OnSettingsClicked();
        }
    }
}
