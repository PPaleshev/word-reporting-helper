using System;
using System.Runtime.InteropServices;
using Microsoft.Office.Interop.Word;
using NLog;
using SampleWordHelper.Core.Application;
using SampleWordHelper.Core.Common;
using SampleWordHelper.Interface;

namespace SampleWordHelper.Presentation
{
    /// <summary>
    /// Вспомогательный класс для отслеживания событий Microsoft Word.
    /// </summary>
    public class ApplicationEventsListener: BasicDisposable
    {
        /// <summary>
        /// Поддержка логирования.
        /// </summary>
        static readonly Logger LOG = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Объект для выполнения обратных вызовов.
        /// </summary>
        readonly IDocumentEventsCallback callback;

        /// <summary>
        /// Контекст выполнения.
        /// </summary>
        readonly IRuntimeContext context;

        /// <summary>
        /// Флаг, равный true, если отслеживание событий приложения приостановлено, иначе false.
        /// </summary>
        readonly SuspendFlag eventsSuspended = new SuspendFlag();

        /// <summary>
        /// Флаг, равный true, если <see cref="OnDocumentChanged"/> вызывается до <see cref="OnNewDocument"/> и <see cref="OnDocumentOpened"/>.
        /// Это означает, что документ был открыт при старте Microsoft Word.
        /// В таких случаях среда не будет вызывать <see cref="OnNewDocument"/> и <see cref="OnDocumentOpened"/>.
        /// </summary>
        bool documentOpenedExternally = true;

        public ApplicationEventsListener(IRuntimeContext context, IDocumentEventsCallback callback)
        {
            this.context = context;
            this.callback = callback;
        }

        /// <summary>
        /// Начинает прослушивание событий от приложения.
        /// </summary>
        public void StartListen()
        {
            var application = context.Application;
            ((ApplicationEvents4_Event)application).NewDocument += OnNewDocument;
            application.DocumentOpen += OnDocumentOpened;
            application.DocumentChange += OnDocumentChanged;
            LOG.Debug("Started to listen for events");
        }

        /// <summary>
        /// Приостанавливает отслеживание событий.
        /// </summary>
        public IDisposable SuspendEvents()
        {
            LOG.Debug("Event listening suspended");
            return eventsSuspended.Suspend();
        }

        /// <summary>
        /// Вызывается при открытии нового документа.
        /// </summary>
        /// <remarks>Для нового документа признак сохранения устанавливается в <c>false</c> для предотвращения его "перезаписывания" при открытии существующего документа.</remarks>
        void OnNewDocument(Document doc)
        {
            LOG.Debug("New document ({0}): [{2}] {1}", eventsSuspended ? "suspended" : "active", doc.FullName, doc.GetKey(context));
            if (eventsSuspended)
                return;
            documentOpenedExternally = false;
            doc.Saved = false;
            SetupShutdown(doc);
            callback.OnDocumentCreated(GetDocumentKey(doc));
        }

        /// <summary>
        /// Вызывается после открытия нового документа.
        /// </summary>
        /// <param name="doc">Открытый документ.</param>
        void OnDocumentOpened(Document doc)
        {
            LOG.Debug("Document opened ({0}): [{2}] {1}", eventsSuspended ? "suspended" : "active", doc.FullName, doc.GetKey(context));
            if (eventsSuspended)
                return;
            documentOpenedExternally = false;
            SetupShutdown(doc);
            callback.OnDocumentOpened(GetDocumentKey(doc));
        }

        /// <summary>
        /// Вызывается после окончательного закрытия документа.
        /// </summary>
        void OnDocumentShutdown(object documentId)
        {
            LOG.Debug("Document shutdown ({0}): [{1}]", eventsSuspended ? "suspended" : "active", documentId);
            if (eventsSuspended )
                return;
            callback.OnDocumentClosed(documentId);
        }

        /// <summary>
        /// Вызывается при изменении активного документа.
        /// </summary>
        void OnDocumentChanged()
        {
            LOG.Debug("Document changed ({0})", eventsSuspended ? "suspended" : "active");
            if (eventsSuspended)
                return;
            try
            {
                var app = context.Application;
                if (app.Documents.Count == 0 || app.ActiveProtectedViewWindow != null || !app.Visible)
                    return;
                var activeDocument = context.Application.ActiveDocument;
                if (activeDocument == null)
                    return;
                LOG.Debug("Listener changed: Key={0}; OpenedExternally={1}; Name={2}", GetDocumentKey(activeDocument), documentOpenedExternally, activeDocument.Name);
                if (documentOpenedExternally)
                {
                    if (string.IsNullOrWhiteSpace(activeDocument.Path))
                        OnNewDocument(activeDocument);
                    else
                        OnDocumentOpened(activeDocument);
                }
                callback.OnDocumentActivated(GetDocumentKey(activeDocument));
            }
            catch (COMException e)
            {
                LOG.Error("Failed to change document", (Exception) e);
            }
        }

        /// <summary>
        /// Подписывается на события закрытия документа.
        /// </summary>
        /// <param name="document">Экземпляр документа.</param>
        void SetupShutdown(Document document)
        {
            var vsto = context.ApplicationFactory.GetVstoObject(document);
            var key = vsto.GetKey();
            vsto.Shutdown += (sender, args) => OnDocumentShutdown(key);
        }

        /// <summary>
        /// Возвращает уникальный идентификатор для документа.
        /// </summary>
        object GetDocumentKey(_Document document)
        {
            var vsto = context.ApplicationFactory.GetVstoObject(document);
            return vsto.GetKey();
        }

        protected override void DisposeManaged()
        {
            var application = context.Application;
            ((ApplicationEvents4_Event) application).NewDocument -= OnNewDocument;
            application.DocumentOpen -= OnDocumentOpened;
            application.DocumentChange -= OnDocumentChanged;
        }
    }

    /// <summary>
    /// Интерфейс обратного вызова для событий, связанных с документами.
    /// </summary>
    public interface IDocumentEventsCallback
    {
        /// <summary>
        /// Вызывается при создании нового документа.
        /// </summary>
        /// <param name="documentId">Некоторый идентификатор, уникально определяющий созданный документ.</param>
        void OnDocumentCreated(object documentId);

        /// <summary>
        /// Вызывается при открытии документа.
        /// </summary>
        /// <param name="documentId">Некоторый идентификатор, уникально определяющий открытый документ.</param>
        void OnDocumentOpened(object documentId);

        /// <summary>
        /// Вызывается при активации документа.
        /// </summary>
        /// <param name="documentId">Некоторый идентификатор, уникально определяющий активный документ.</param>
        void OnDocumentActivated(object documentId);

        /// <summary>
        /// Вызывается при окончательном закрытии документа.
        /// </summary>
        /// <param name="documentId">Некоторый идентификатор, уникально определяющий закрытый документ.</param>
        void OnDocumentClosed(object documentId);
    }
}
