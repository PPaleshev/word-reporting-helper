using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using Microsoft.Office.Interop.Word;
using NLog;
using SampleWordHelper.Core.Application;
using SampleWordHelper.Core.Common;
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
        /// Поддержка логирования.
        /// </summary>
        static readonly Logger LOG = LogManager.GetCurrentClassLogger();

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
        /// Представление списка команд.
        /// </summary>
        readonly ICommandView commandView;

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
            commandView = context.Environment.ViewFactory.CreateCommandView();
            dragController = new TreeDragDropController(context.Environment, view, model, this);
            previewManager = new PreviewManager(context.Environment);
            InitializeCommands(commandView);
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

        /// <summary>
        /// Инициализирует команды.
        /// </summary>
        void InitializeCommands(ICommandView cmdView)
        {
            cmdView.Add("Вставить", "Добавляет содержимое выбранного элемента каталога в редактируемый документ.", "paste",
                () => model.IsContentNode(model.SelectedNodeId), () => OnNodeDoubleClicked(model.SelectedNodeId));
            cmdView.Add("Предварительный просмотр", "Отображает содержимое выбранного элемента каталога в окне предварительного просмотра.", "preview",
                () => model.IsContentNode(model.SelectedNodeId), () => OnPreviewRequested(model.SelectedNodeId));
            cmdView.Add("Открыть", "Открывает выбранный элемент каталога в новом окне в режиме только для чтения.", "open",
                () => model.IsContentNode(model.SelectedNodeId), OpenDocumentInNewWindow);
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
            PasteFile(model.ExtractFilePathFromTransferredData(obj), (Range) application.ActiveWindow.RangeFromPoint(point.X, point.Y));
        }

        public void OnNodeDoubleClicked(object item)
        {
            var application = context.Environment.Application;
            if (!model.IsContentNode(item) || application.ActiveDocument == null)
                return;
            PasteFile(model.GetFilePathForId(item), application.Selection.Range);
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
            if (!model.IsContentNode(item))
                return;
            previewManager.ShowPreview(model.GetFilePathForId(item), true);
        }

        public void OnItemSelected(object item)
        {
            model.SelectedNodeId = model.IsContentNode(item) ? item : null;
        }

        /// <summary>
        /// Выполняет активацию презентера при смене активного документа.
        /// </summary>
        public void Activate()
        {
            callback.OnVisibilityChanged(model.IsVisible);
            view.SetContextMenu(commandView);
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
            commandView.SafeDispose();
            dragController.SafeDispose();
            view.SafeDispose();
            previewManager.SafeDispose();
        }

        /// <summary>
        /// Открывает выбранный элемент каталога в новом окне в режиме только для чтения.
        /// </summary>
        void OpenDocumentInNewWindow()
        {
            var application = context.Environment.Application;
            var doc = application.Documents.Open(model.GetFilePathForId(model.SelectedNodeId), ReadOnly: true, Visible: true, AddToRecentFiles: true);
            doc.Activate();
        }

        /// <summary>
        /// Выполняет вставку данных из файла в текущий документ.
        /// </summary>
        /// <param name="unsafeFilePath">Путь к файлу.</param>
        /// <param name="range">Диапазон.</param>
        static void PasteFile(string unsafeFilePath, Range range)
        {
            try
            {
                PasteMethods.OpenAndCopyPaste(unsafeFilePath, range);
                range.Application.ActiveWindow.SetFocus();
            }
            catch (COMException e)
            {
                LOG.Error("Error inserting file into document: " + unsafeFilePath, (Exception) e);
                MessageBox.Show("Ошибка вставки документа.\r\nПричина: " + e.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
