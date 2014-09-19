using System;
using System.Windows.Forms;
using Microsoft.Office.Tools.Word;
using SampleWordHelper.Core;
using SampleWordHelper.Interface;
using SampleWordHelper.Model;

namespace SampleWordHelper.Presentation
{
    public class DocumentPresenter: IRibbonPresenter, IStructurePresenter, IDisposable
    {
        readonly IRuntimeContext context;
        readonly IRibbonView ribbonView;
        readonly IStructureView structureView;
        readonly Microsoft.Office.Tools.CustomTaskPane taskPaneView;
        readonly DocumentModel model;

        public DocumentPresenter(IRuntimeContext context, Document document)
        {
            this.context = context;
            model = new DocumentModel();
            ribbonView = context.ViewFactory.CreateRibbonView(new RibbonEventFilter(context, this, document.GetKey()));
            structureView = context.ViewFactory.CreateStructureView(this);
            taskPaneView = context.ViewFactory.CreateTaskPaneContainer((UserControl) structureView.RawObject, model.PaneTitle);
            taskPaneView.VisibleChanged += TaskPaneViewVisibleChanged;
        }

        public void OnToggleStructureVisibility()
        {
            model.IsStructureVisible = !model.IsStructureVisible;
            ribbonView.SetStructureVisible(model.IsStructureVisible);
            taskPaneView.Visible = model.IsStructureVisible;
        }

        public void OnShowSettings()
        {
            MessageBox.Show("Settings here");
        }

        /// <summary>
        /// Выполняет активацию презентера при смене активного документа.
        /// </summary>
        public void Activate()
        {
            ribbonView.SetStructureVisible(model.IsStructureVisible);
            taskPaneView.Visible = model.IsStructureVisible;
        }

        /// <summary>
        /// Начинает выполнение презентера.
        /// </summary>
        public void Run()
        {
            model.IsStructureVisible = false;
            Activate();
        }

        /// <summary>
        /// Вызывается при изменении видимости панели структуры.
        /// Используется для обработки закрытии панели пользователем.
        /// </summary>
        void TaskPaneViewVisibleChanged(object sender, EventArgs eventArgs)
        {
            if (taskPaneView.Visible)
                return;
            model.IsStructureVisible = false;
            ribbonView.SetStructureVisible(model.IsStructureVisible);
        }

        public void Dispose()
        {
            taskPaneView.VisibleChanged -= TaskPaneViewVisibleChanged;
            ribbonView.Dispose();
            structureView.Dispose();
        }
    }
}
