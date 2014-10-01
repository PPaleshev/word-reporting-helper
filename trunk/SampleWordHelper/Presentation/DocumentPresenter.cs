using System;
using System.IO;
using System.Windows.Forms;
using Microsoft.Office.Interop.Word;
using SampleWordHelper.Core;
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
    public class DocumentPresenter: IRibbonEventListener, IStructurePresenter, IDropCallback, IDisposable
    {
        /// <summary>
        /// Представление ленты.
        /// </summary>
        readonly IRibbonView ribbonView;

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
        /// Контекст времени исполнения надстройки.
        /// </summary>
        readonly IRuntimeContext context;

        /// <summary>
        /// Создаёт новый экземпляр менеджера документа.
        /// </summary>
        /// <param name="context">Контекст исполнения надстройки.</param>
        /// <param name="view">Экземпляр представления для управления лентой.</param>
        /// <param name="catalog">Модель каталога.</param>
        public DocumentPresenter(IRuntimeContext context, IRibbonView view, ICatalog catalog)
        {
            this.context = context;
            ribbonView = view;
            model = new DocumentModel();
            model.SetModel(catalog);
            structureView = context.ViewFactory.CreateStructureView(this, model.PaneTitle);
            dragController = new TreeDragDropController(context, structureView, model, this);
        }

        public IDragSourceController DragController
        {
            get { return dragController; }
        }

        public void OnToggleStructureVisibility()
        {
            model.IsVisible = !model.IsVisible;
            ribbonView.SetStructureVisible(model.IsVisible);
            structureView.SetVisibility(model.IsVisible);
        }

        public void OnClosed()
        {
            model.IsVisible = false;
            ribbonView.SetStructureVisible(model.IsVisible);
        }

        void IDropCallback.OnDrop(IDataObject obj, Point point)
        {
            if (!model.IsValidDropData(obj))
                return;
            if (context.Application.ActiveDocument == null)
                return;
            var filePath = model.ExtractFilePathFromTransferredData(obj);
            using (var safeFile = new SafeFilePath(filePath))
            {
                var range = (Range) context.Application.ActiveWindow.RangeFromPoint(point.X, point.Y);
                range.InsertFile(safeFile.FilePath, ConfirmConversions: true);
            }
            context.Application.ActiveWindow.SetFocus();
        }

        public void OnNodeDoubleClicked(object item)
        {
            if (!model.CanDragNode(item) || context.Application.ActiveDocument == null)
                return;
            var filePath = model.GetFilePathForId(item);
            using (var safeFile = new SafeFilePath(filePath))
            {
                var range = context.Application.Selection;
                range.InsertFile(safeFile.FilePath, ConfirmConversions: true);
            }
            context.Application.ActiveWindow.SetFocus();
        }

        /// <summary>
        /// Выполняет активацию презентера при смене активного документа.
        /// </summary>
        public void Activate()
        {
            ribbonView.SetStructureVisible(model.IsVisible);
            structureView.SetVisibility(model.IsVisible);
            structureView.UpdateStructure(model);
        }

        /// <summary>
        /// Начинает выполнение презентера.
        /// </summary>
        public void Run()
        {
            //http://code.msdn.microsoft.com/Word-2010-Using-the-Drag-81bb5bff
            model.IsVisible = false;
            Activate();
        }

        /// <summary>
        /// Вызывается при обновлении данных каталога.
        /// </summary>
        /// <param name="catalog">Обновлённая модель каталога.</param>
        public void UpdateCatalog(ICatalog catalog)
        {
            model.SetModel(catalog);
            structureView.UpdateStructure(model);
        }

        public void Dispose()
        {
            dragController.SafeDispose();
            structureView.SafeDispose();
        }
    }

    /// <summary>
    /// Интерфейс обратного вызова для обработки успешного перетаскивания.
    /// </summary>
    public interface IDropCallback
    {
        /// <summary>
        /// Вызывается при успешном перетаскивании указанного элемента каталога.
        /// </summary>
        /// <param name="obj">Перетащенный объект.</param>
        /// <param name="point">Экранные координаты точки, в которой был сброшен объект.</param>
        void OnDrop(IDataObject obj, Point point);
    }
}
