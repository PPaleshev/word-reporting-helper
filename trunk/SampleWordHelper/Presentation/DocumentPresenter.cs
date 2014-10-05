using System;
using System.Windows.Forms;
using Microsoft.Office.Interop.Word;
using SampleWordHelper.Core.Application;
using SampleWordHelper.Core.Common;
using SampleWordHelper.Core.IO;
using SampleWordHelper.Interface;
using SampleWordHelper.Model;
using Point = System.Drawing.Point;

namespace SampleWordHelper.Presentation
{
    /// <summary>
    /// Менеджер документа Word.
    /// </summary>
    public class DocumentPresenter : BasicDisposable, ICatalogPresenter, IDropCallback
    {
        /// <summary>
        /// Общее представление документа.
        /// </summary>
        readonly IDocumentView structureView;

        /// <summary>
        /// Модель документа.
        /// </summary>
        readonly DocumentModel model;

        /// <summary>
        /// Вспомогательный класс для управления процессом перетаскивания (drag and drop).
        /// </summary>
        readonly IDragSourceController dragController;

        /// <summary>
        /// Контекст выполнения приложения.
        /// </summary>
        readonly IApplicationContext context;

        /// <summary>
        /// Объект для выполнения обратных вызовов при взаимодействии с панелью каталога.
        /// </summary>
        readonly ICatalogPaneCallback callback;

        /// <summary>
        /// Создаёт новый экземпляр менеджера документа.
        /// </summary>
        /// <param name="context">Контекст исполнения надстройки.</param>
        /// <param name="callback">Объект для выполнения обратных вызовов при взаимодействии с панелью каталога.</param>
        public DocumentPresenter(IApplicationContext context, ICatalogPaneCallback callback)
        {
            this.context = context;
            this.callback = callback;
            model = new DocumentModel();
            model.SetModel(context.Catalog);
            structureView = context.Environment.ViewFactory.CreateStructureView(this, model.PaneTitle);
            dragController = new TreeDragDropController(context.Environment, structureView, model, this);
        }

        public IDragSourceController DragController
        {
            get { return dragController; }
        }

        public void ToggleCatalogVisibility()
        {
            model.IsVisible = !model.IsVisible;
            callback.OnVisibilityChanged(model.IsVisible);
            structureView.SetVisibility(model.IsVisible);
        }

        void ICatalogPresenter.OnClosed()
        {
            callback.OnVisibilityChanged(model.IsVisible = false);
        }

        void IDropCallback.OnDrop(IDataObject obj, Point point)
        {
            if (!model.IsValidDropData(obj))
                return;
            var application = context.Environment.Application;
            if (application.ActiveDocument == null)
                return;
            var filePath = model.ExtractFilePathFromTransferredData(obj);
            using (var safeFile = new SafeFilePath(filePath))
            {
                var range = (Range) application.ActiveWindow.RangeFromPoint(point.X, point.Y);
                range.InsertFile(safeFile.FilePath, ConfirmConversions: true);
            }
            application.ActiveWindow.SetFocus();
        }

        public void OnNodeDoubleClicked(object item)
        {
            var application = context.Environment.Application;
            if (!model.CanDragNode(item) || application.ActiveDocument == null)
                return;
            var filePath = model.GetFilePathForId(item);
            using (var safeFile = new SafeFilePath(filePath))
            {
                var range = application.Selection;
                range.InsertFile(safeFile.FilePath, ConfirmConversions: true);
            }
            application.ActiveWindow.SetFocus();
        }

        public void OnFilterTextChanged(string filterText)
        {
            if (string.Equals(model.Filter, filterText, StringComparison.InvariantCultureIgnoreCase))
                return;
            model.UpdateFilter(filterText);
            structureView.SetFilterText(filterText);
            structureView.UpdateStructure(model);
        }

        /// <summary>
        /// Выполняет активацию презентера при смене активного документа.
        /// </summary>
        public void Activate()
        {
            callback.OnVisibilityChanged(model.IsVisible);
            structureView.SetVisibility(model.IsVisible);
            structureView.UpdateStructure(model);
        }

        /// <summary>
        /// Начинает выполнение презентера.
        /// </summary>
        public void Run()
        {
            //http://code.msdn.microsoft.com/Word-2010-Using-the-Drag-81bb5bff
            model.IsVisible = true;
            Activate();
        }

        /// <summary>
        /// Вызывается при обновлении данных каталога.
        /// </summary>
        public void UpdateCatalog()
        {
            model.SetModel(context.Catalog);
            structureView.UpdateStructure(model);
        }

        protected override void DisposeManaged()
        {
            dragController.SafeDispose();
            structureView.SafeDispose();
        }
    }
}
