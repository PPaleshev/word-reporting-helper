using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using Microsoft.Office.Interop.Word;
using SampleWordHelper.Core.Application;
using SampleWordHelper.Core.Common;
using SampleWordHelper.Interface;
using SampleWordHelper.Model;
using SampleWordHelper.Providers.FileSystem;

namespace SampleWordHelper.Presentation
{
    /// <summary>
    /// Основной менеджер надстройки.
    /// </summary>
    public class DocumentManager: IRibbonEventListener, IDisposable
    {
        /// <summary>
        /// Контекст работы надстройки.
        /// </summary>
        readonly IRuntimeContext context;

        /// <summary>
        /// Отображение из ключа документа в управляющий им менеджер.
        /// </summary>
        readonly Dictionary<object, DocumentPresenter> presenters = new Dictionary<object, DocumentPresenter>();

        /// <summary>
        /// Экземпляр представления для управления элементами ленты.
        /// </summary>
        readonly IRibbonView ribbonView;

        /// <summary>
        /// Текущий экземпляр каталога.
        /// </summary>
        ICatalog currentCatalog;

        /// <summary>
        /// Флаг, равный true, если <see cref="OnDocumentChanged"/> вызывается до <see cref="OnNewDocument"/> и <see cref="OnDocumentOpened"/>.
        /// Это означает, что документ был открыт при старте Microsoft Word.
        /// В таких случаях среда не будет вызывать <see cref="OnNewDocument"/> и <see cref="OnDocumentOpened"/>.
        /// </summary>
        bool documentOpenedExternally = true;

        /// <summary>
        /// Создаёт новый экземпляр менеджера документов.
        /// </summary>
        public DocumentManager(IRuntimeContext context)
        {
            this.context = context;
            ribbonView = context.ViewFactory.CreateRibbonView(this);
            var application = context.Application;
            ((ApplicationEvents4_Event) application).NewDocument += OnNewDocument;
            application.DocumentOpen += OnDocumentOpened;
            application.DocumentChange += OnDocumentChanged;
            currentCatalog = new Catalog();
        }

        /// <summary>
        /// Вызывается для обновления каталога.
        /// </summary>
        /// <param name="catalog">Модель каталога.</param>
        public void UpdateCatalog(ICatalog catalog)
        {
            currentCatalog = catalog;
            var subscribers = presenters.Values.ToArray();
            foreach (var presenter in subscribers)
                presenter.UpdateCatalog(currentCatalog);
        }

        /// <summary>
        /// Добавляет документ в список активных.
        /// </summary>
        void RegisterDocument(Document document)
        {
            var toolDoc = context.ApplicationFactory.GetVstoObject(document);
            var key = toolDoc.GetKey();
            toolDoc.Shutdown += (sender, args) => OnDocumentShutdown(key);
            var presenter = new DocumentPresenter(context, ribbonView, currentCatalog);
            presenters.Add(key, presenter);
            presenter.Run();
        }

        /// <summary>
        /// Пытается получить презентер для указанного документа.
        /// </summary>
        bool TryGetPresenter(Document document, out DocumentPresenter presenter)
        {
            var vsto = context.ApplicationFactory.GetVstoObject(document);
            return presenters.TryGetValue(vsto.GetKey(), out presenter);
        }

        /// <summary>
        /// Вызывается при открытии нового документа.
        /// </summary>
        /// <remarks>Для нового документа признак сохранения устанавливается в <c>false</c> для предотвращения его "перезаписывания" при открытии существующего документа.</remarks>
        void OnNewDocument(Document newDocument)
        {
            documentOpenedExternally = false;
            newDocument.Saved = false;
            RegisterDocument(newDocument);
        }

        /// <summary>
        /// Вызывается после открытия нового документа.
        /// </summary>
        /// <param name="doc">Открытый документ.</param>
        void OnDocumentOpened(Document doc)
        {
            documentOpenedExternally = false;
            RegisterDocument(doc);
        }

        /// <summary>
        /// Вызывается после окончательного закрытия документа.
        /// </summary>
        void OnDocumentShutdown(object documentKey)
        {
            DocumentPresenter presenter;
            if (!presenters.TryGetValue(documentKey, out presenter))
                return;
            presenters.Remove(documentKey);
            presenter.Dispose();
        }

        /// <summary>
        /// Вызывается при изменении активного документа.
        /// </summary>
        void OnDocumentChanged()
        {
            try
            {
                if (context.Application.Documents.Count == 0)
                    return;
                if (context.Application.ActiveProtectedViewWindow != null)
                    return;
                var activeDocument = context.Application.ActiveDocument;
                if (activeDocument == null)
                    return;
                DocumentPresenter presenter;
                if (documentOpenedExternally)
                {
                    if (string.IsNullOrWhiteSpace(activeDocument.Path))
                        OnNewDocument(activeDocument);
                    else
                        OnDocumentOpened(activeDocument);
                }
                else if (TryGetPresenter(activeDocument, out presenter))
                    presenter.Activate();
            }
            catch (COMException e)
            {
                MessageBox.Show("OnDocumentChanged: " + e.StackTrace);
            }
        }

        public void OnToggleStructureVisibility()
        {
            var doc = context.Application.ActiveDocument;
            if (doc == null)
                return;
            DocumentPresenter p;
            if (TryGetPresenter(doc, out p))
                p.OnToggleStructureVisibility();
        }

        public void Dispose()
        {
            ribbonView.Dispose();
            var application = context.Application;
            if (application == null)
                return;
            ((ApplicationEvents4_Event) application).NewDocument -= OnNewDocument;
            application.DocumentOpen -= OnDocumentOpened;
            application.DocumentChange -= OnDocumentChanged;
        }
    }
}
