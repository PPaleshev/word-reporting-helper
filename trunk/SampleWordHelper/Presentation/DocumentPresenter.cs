using System;
using Microsoft.Office.Tools.Word;
using SampleWordHelper.Core;
using SampleWordHelper.Interface;
using SampleWordHelper.Model;

namespace SampleWordHelper.Presentation
{
    public class DocumentPresenter: IRibbonPresenter, IStructurePresenter, IDisposable
    {
        readonly IRibbonView ribbonView;
        readonly IStructureView structureView;
        readonly DocumentModel model;
        readonly IDragSourceController structureDragController;

        public DocumentPresenter(IRuntimeContext context, Document document)
        {
            model = new DocumentModel();
            ribbonView = context.ViewFactory.CreateDocumentView(new RibbonEventFilter(context, this, document.GetKey()));
            structureView = context.ViewFactory.CreateStructureView(this, model.PaneTitle);
            structureDragController = new TreeDragDropController(context, structureView, new StructureModel(null));
        }

        public IDragSourceController DragController
        {
            get { return structureDragController; }
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
            model.IsStructureVisible = true;
            Activate();
        }

        public void OnClosed()
        {
            model.IsStructureVisible = false;
            ribbonView.SetStructureVisible(model.IsStructureVisible);
        }

        public void Dispose()
        {
            structureDragController.SafeDispose();
            ribbonView.SafeDispose();
            structureView.SafeDispose();
        }
    }
}
