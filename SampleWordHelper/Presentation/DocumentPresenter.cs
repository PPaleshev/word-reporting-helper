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
        readonly IDocumentView view;

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
        /// Менеджер отображения превью.
        /// </summary>
        readonly PreviewManager previewManager;

        /// <summary>
        /// Создаёт новый экземпляр менеджера документа.
        /// </summary>
        /// <param name="context">Контекст исполнения надстройки.</param>
        /// <param name="callback">Объект для выполнения обратных вызовов при взаимодействии с панелью каталога.</param>
        public DocumentPresenter(IApplicationContext context, ICatalogPaneCallback callback)
        {
            this.context = context;
            this.callback = callback;
            model = new DocumentModel(context.Catalog, context.SearchEngine);
            view = context.Environment.ViewFactory.CreateStructureView(this, model.PaneTitle);
            dragController = new TreeDragDropController(context.Environment, view, model, this);
            previewManager = new PreviewManager(context.Environment);
        }

        public IDragSourceController DragController
        {
            get { return dragController; }
        }

        /// <summary>
        /// Вызывается при изменении видимости структуры каталога.
        /// </summary>
        public void UpdateCatalogVisibility(bool visible)
        {
            model.IsVisible = visible;
            callback.OnVisibilityChanged(model.IsVisible);
            view.SetVisibility(model.IsVisible);
        }

        void ICatalogPresenter.OnPaneVisibilityChanged(bool visible)
        {
            callback.OnVisibilityChanged(model.IsVisible = visible);
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
            model.SetFilter(filterText);
            view.SetFilterText(filterText);
            view.UpdateStructure(model);
        }

        public void OnPreviewRequested(object item)
        {
            if (!model.CanDragNode(item))
                return;
            previewManager.ShowPreview(model.GetFilePathForId(item));
        }

        /// <summary>
        /// Выполняет активацию презентера при смене активного документа.
        /// </summary>
        public void Activate()
        {
            callback.OnVisibilityChanged(model.IsVisible);
            view.SetWidth(model.DefaultWidth);
            view.SetVisibility(model.IsVisible);
            view.UpdateStructure(model);
        }

        /// <summary>
        /// Начинает выполнение презентера.
        /// </summary>
        public void Run()
        {
            Activate();
        }

        /// <summary>
        /// Вызывается при обновлении данных каталога.
        /// </summary>
        public void UpdateCatalog()
        {
            model.SetModel(context.Catalog);
            view.UpdateStructure(model);
        }

        protected override void DisposeManaged()
        {
            dragController.SafeDispose();
            view.SafeDispose();
            previewManager.SafeDispose();
        }
    }
}
