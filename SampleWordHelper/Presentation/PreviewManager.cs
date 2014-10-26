using System;
using System.Collections.Generic;
using System.Linq;
using SampleWordHelper.Core.Application;
using SampleWordHelper.Core.Common;

namespace SampleWordHelper.Presentation
{
    /// <summary>
    /// Менеджер по управлению предварительным просмотром документов.
    /// </summary>
    public class PreviewManager : BasicDisposable
    {
        /// <summary>
        /// Фабрика представлений.
        /// </summary>
        readonly IViewFactory viewFactory;

        /// <summary>
        /// Отображение из имени файла в презентер, который им управляет.
        /// </summary>
        readonly Dictionary<string, PreviewPresenter> presenters = new Dictionary<string, PreviewPresenter>(StringComparer.InvariantCultureIgnoreCase);

        /// <summary>
        /// Объект для выполнения обратных вызовов от менеджеров превью.
        /// </summary>
        readonly IPreviewCallback callback;

        /// <summary>
        /// Создаёт новый менеджер превью.
        /// </summary>
        public PreviewManager(IRuntimeContext context, IPreviewCallback callback)
        {
            viewFactory = context.ViewFactory;
            this.callback = callback;
        }

        /// <summary>
        /// Отображает превью для файла с указанным именем.
        /// </summary>
        /// <param name="fileName">Путь к файлу для отображения.</param>
        public void ShowPreview(string fileName)
        {
            if (TryShowExisting(fileName))
                return;
            /*
            foreach (var presenter1 in presenters.Values)
                presenter1.SafeDispose();
            presenters.Clear();
             */

            var presenter = new PreviewPresenter(fileName, viewFactory, new PreviewCallbackFacade(OnPresenterClosed, callback));
            presenters.Add(fileName, presenter);
            presenter.Show(true);
        }

        /// <summary>
        /// Вызывается при закрытии одного из презентеров.
        /// </summary>
        void OnPresenterClosed(PreviewPresenter p)
        {
            var pair = presenters.FirstOrDefault(kv => kv.Value == p);
            presenters.Remove(pair.Key);
            p.SafeDispose();
        }

        /// <summary>
        /// Пытается отобразить уже открытый документ.
        /// </summary>
        /// <param name="fileName">Путь к файлу.</param>
        /// <returns>Возвращает true, если представление отображено для файла, иначе false.</returns>
        bool TryShowExisting(string fileName)
        {
            PreviewPresenter presenter;
            if (!presenters.TryGetValue(fileName, out presenter))
                return false;
            presenter.Show(false);
            return true;
        }

        /// <summary>
        /// Выгружает все активные презентеры.
        /// </summary>
        void UnloadAll()
        {
            foreach (var presenter in presenters.Values)
                presenter.SafeDispose();
            presenters.Clear();
        }

        protected override void DisposeManaged()
        {
            UnloadAll();
        }
    }

    /// <summary>
    /// Фасад для вызова методов обратного вызова.
    /// </summary>
    class PreviewCallbackFacade
    {
        /// <summary>
        /// Метод, вызываемый при закрытии менеджера.
        /// </summary>
        readonly Action<PreviewPresenter> OnClosed;

        /// <summary>
        /// Объект для обратного вызова.
        /// </summary>
        readonly IPreviewCallback callback;

        /// <summary>
        /// Создаёт новый экземпляр фасада.
        /// </summary>
        public PreviewCallbackFacade(Action<PreviewPresenter> OnClosed, IPreviewCallback callback)
        {
            this.OnClosed = OnClosed;
            this.callback = callback;
        }

        /// <summary>
        /// Вызывается при закрытии презентера.
        /// </summary>
        /// <param name="p">Закрываемый презентер.</param>
        public void OnClose(PreviewPresenter p)
        {
            OnClosed(p);
        }

        /// <summary>
        /// Вызывается для вставки данных в документ.
        /// </summary>
        public void OnPaste(string fileName)
        {
            callback.OnPaste(fileName);
        }
    }
}
