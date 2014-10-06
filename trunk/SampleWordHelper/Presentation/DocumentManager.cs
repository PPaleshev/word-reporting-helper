using System.Collections.Generic;
using System.Linq;
using SampleWordHelper.Core.Application;
using SampleWordHelper.Core.Common;

namespace SampleWordHelper.Presentation
{
    /// <summary>
    /// Основной менеджер надстройки.
    /// TODO PP: различать ситуации, когда Application.ShowInTaskBar = false. В этом случае окно создаётся одно и task pane отображаются в рамках этого одного окна.
    /// TODO PP: Необходимо каким-то образом отслеживать данную ситуацию и соответствующим образом корректировать презентеры документов.
    /// </summary>
    public class DocumentManager: BasicDisposable, IDocumentEventsCallback
    {
        /// <summary>
        /// Контекст работы надстройки.
        /// </summary>
        readonly IApplicationContext context;

        /// <summary>
        /// Объект для выполнения обратных вызовов при взаимодействии с панелью каталога.
        /// </summary>
        readonly ICatalogPaneCallback callback;

        /// <summary>
        /// Отображение из ключа документа в управляющий им менеджер.
        /// </summary>
        readonly Dictionary<object, DocumentPresenter> presenters = new Dictionary<object, DocumentPresenter>();

        /// <summary>
        /// Создаёт новый экземпляр менеджера документов.
        /// </summary>
        public DocumentManager(IApplicationContext context, ICatalogPaneCallback callback)
        {
            this.context = context;
            this.callback = callback;
        }

        /// <summary>
        /// Вызывается для обновления каталога.
        /// </summary>
        public void UpdateCatalog()
        {
            var subscribers = presenters.Values.ToArray();
            foreach (var presenter in subscribers)
                presenter.UpdateCatalog();
        }

        /// <summary>
        /// Изменяет видимость панели каталога.
        /// </summary>
        public void UpdateCatalogVisibility(bool visible)
        {
            var key = context.Environment.GetActiveDocumentKey();
            DocumentPresenter presenter;
            if (presenters.TryGetValue(key, out presenter))
                presenter.UpdateCatalogVisibility(visible);
        }

        /// <summary>
        /// Добавляет документ в список активных.
        /// </summary>
        /// <param name="documentId">Уникальный идентификатор открываемого документа.</param>
        void RegisterDocument(object documentId)
        {
            var presenter = new DocumentPresenter(context, callback);
            presenters.Add(documentId, presenter);
            presenter.Run();
        }

        void IDocumentEventsCallback.OnDocumentCreated(object documentId)
        {
            if (!presenters.ContainsKey(documentId))
                RegisterDocument(documentId);
        }

        void IDocumentEventsCallback.OnDocumentOpened(object documentId)
        {
            if (!presenters.ContainsKey(documentId))
                RegisterDocument(documentId);
        }

        void IDocumentEventsCallback.OnDocumentActivated(object documentId)
        {
            DocumentPresenter presenter;
            if (presenters.TryGetValue(documentId, out presenter))
                presenter.Activate();
        }

        void IDocumentEventsCallback.OnDocumentClosed(object documentId)
        {
            DocumentPresenter presenter;
            if (!presenters.TryGetValue(documentId, out presenter))
                return;
            presenters.Remove(documentId);
            presenter.SafeDispose();
        }

        protected override void DisposeManaged()
        {
            var data = presenters.Values.ToArray();
            foreach (var presenter in data)
                presenter.SafeDispose();
            presenters.Clear();
        }
    }
}
