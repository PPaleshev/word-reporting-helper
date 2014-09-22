using System;
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
        readonly Document document;

        public DocumentPresenter(IRuntimeContext context, Document document)
        {
            this.context = context;
            this.document = document;
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
            //http://code.msdn.microsoft.com/Word-2010-Using-the-Drag-81bb5bff
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
