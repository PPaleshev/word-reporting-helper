using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using SampleWordHelper.Core.Application;
using SampleWordHelper.Core.Common;
using SampleWordHelper.Interface;
using SampleWordHelper.Model;

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
        /// Создаёт новый менеджер превью.
        /// </summary>
        public PreviewManager(IRuntimeContext context)
        {
            viewFactory = context.ViewFactory;
        }

        /// <summary>
        /// Отображает превью для файла с указанным именем.
        /// </summary>
        /// <param name="fileName">Путь к файлу для отображения.</param>
        /// <param name="allowMultiple">Флаг, равный true, если возможно отображение нескольких окон предварительного просмотра, иначе только одно.</param>
        public void ShowPreview(string fileName, bool allowMultiple)
        {
            if (TryShowExisting(fileName))
                return;
            if (!allowMultiple)
            {
                foreach (var presenter1 in presenters.Values)
                    presenter1.SafeDispose();
                presenters.Clear();
            }
            var presenter = new PreviewPresenter(fileName, viewFactory, OnPresenterClosed);
            presenters.Add(fileName, presenter);
            presenter.Activate(true);
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
            presenter.Activate(false);
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
    /// Менеджер отображения представления.
    /// </summary>
    class PreviewPresenter : BasicDisposable, IPreviewPresenter
    {
        /// <summary>
        /// Метод обратного вызова при закрытии представления текущего презентера.
        /// </summary>
        readonly Action<PreviewPresenter> closedCallback;

        /// <summary>
        /// Модель презентера.
        /// </summary>
        readonly PreviewModel model;

        /// <summary>
        /// Текущее представление.
        /// </summary>
        readonly IPreviewView view;

        /// <summary>
        /// Создаёт новый экземпляр презентера.
        /// </summary>
        /// <param name="fileName">Путь к файлу.</param>
        /// <param name="viewFactory">Фабрика представлений.</param>
        /// <param name="closedCallback">Метод обратного вызова при закрытии представления.</param>
        public PreviewPresenter(string fileName, IViewFactory viewFactory, Action<PreviewPresenter> closedCallback)
        {
            this.closedCallback = closedCallback;
            view = viewFactory.CreatePreviewView(this);
            model = new PreviewModel(fileName);
        }

        /// <summary>
        /// Активирует текущий презентер.
        /// </summary>
        public void Activate(bool initialize)
        {
            if (initialize)
            {
                var caption = Path.GetFileName(model.FileName);
                view.SetCaption(caption);
                if (model.IsValid)
                    model.GeneratePreview(view.Handle, view.PreviewArea);
            }
            view.Show(model.IsValid, model.ErrorMessage);
        }

        void IPreviewPresenter.OnSizeChanged()
        {
            model.UpdateSize(view.PreviewArea);
        }

        void IPreviewPresenter.OnClose()
        {
            closedCallback(this);
        }

        protected override void DisposeManaged()
        {
            view.SafeDispose();
            model.SafeDispose();
        }
    }
}
