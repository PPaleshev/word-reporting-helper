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
        readonly DocumentModel model;

        public DocumentPresenter(IRuntimeContext context, Document document)
        {
            this.context = context;
            model = new DocumentModel();
            ribbonView = context.ViewFactory.CreateDocumentView(new RibbonEventFilter(context, this, document.GetKey()));
            structureView = context.ViewFactory.CreateStructureView(this, model.PaneTitle);
        }

        public void OnToggleStructureVisibility()
        {
            model.IsStructureVisible = !model.IsStructureVisible;
            ribbonView.SetStructureVisible(model.IsStructureVisible);
            structureView.SetVisibility(model.IsStructureVisible);
        }

        /// <summary>
        /// Выполняет активацию презентера при смене активного документа.
        /// </summary>
        public void Activate()
        {
            ribbonView.SetStructureVisible(model.IsStructureVisible);
            structureView.SetVisibility(model.IsStructureVisible);
        }

        /// <summary>
        /// Начинает выполнение презентера.
        /// </summary>
        public void Run()
        {
            model.IsStructureVisible = false;
            Activate();
        }

        public void OnClosed()
        {
            model.IsStructureVisible = false;
            ribbonView.SetStructureVisible(model.IsStructureVisible);
        }

        public void Dispose()
        {
            ribbonView.Dispose();
            structureView.Dispose();
        }
    }
}
