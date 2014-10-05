using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using Microsoft.Office.Interop.Word;
using SampleWordHelper.Core.Application;
using SampleWordHelper.Core.Common;
using SampleWordHelper.Interface;

namespace SampleWordHelper.Presentation
{
    /// <summary>
    /// Вспомогательный класс для отслеживания событий Microsoft Word.
    /// </summary>
    public class ApplicationEventsListener
    {
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
        public void Listen()
        {
            var application = context.Application;
            ((ApplicationEvents4_Event)application).NewDocument += OnNewDocument;
            application.DocumentOpen += OnDocumentOpened;
            application.DocumentChange += OnDocumentChanged;
        }

        /// <summary>
        /// Приостанавливает отслеживание событий.
        /// </summary>
        public IDisposable SuspendEvents()
        {
            return eventsSuspended.Suspend();
        }

        /// <summary>
        /// Вызывается при открытии нового документа.
        /// </summary>
        /// <remarks>Для нового документа признак сохранения устанавливается в <c>false</c> для предотвращения его "перезаписывания" при открытии существующего документа.</remarks>
        void OnNewDocument(Document doc)
        {
            if (eventsSuspended)
                return;
            documentOpenedExternally = false;
            doc.Saved = false;
            SetupShutdown(doc);
            Debug.WriteLine("OnNewDocument");
            callback.OnDocumentCreated(GetDocumentKey(doc));
        }

        /// <summary>
        /// Вызывается после открытия нового документа.
        /// </summary>
        /// <param name="doc">Открытый документ.</param>
        void OnDocumentOpened(Document doc)
        {
            if (eventsSuspended)
                return;
            documentOpenedExternally = false;
            SetupShutdown(doc);
            Debug.WriteLine("OnOpened");
            callback.OnDocumentOpened(GetDocumentKey(doc));
        }

        /// <summary>
        /// Вызывается после окончательного закрытия документа.
        /// </summary>
        void OnDocumentShutdown(object documentId)
        {
            if (eventsSuspended )
                return;
            Debug.WriteLine("OnShutdown");
            callback.OnDocumentClosed(documentId);
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
                if (documentOpenedExternally)
                {
                    if (string.IsNullOrWhiteSpace(activeDocument.Path))
                        OnNewDocument(activeDocument);
                    else
                        OnDocumentOpened(activeDocument);
                }
                callback.OnDocumentActivated(GetDocumentKey(activeDocument));
                Debug.WriteLine("OnActivated");
            }
            catch (COMException e)
            {
                MessageBox.Show("OnDocumentChanged: " + e.StackTrace);
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
