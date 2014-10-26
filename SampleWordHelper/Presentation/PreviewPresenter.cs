using System.IO;
using SampleWordHelper.Core.Application;
using SampleWordHelper.Core.Common;
using SampleWordHelper.Interface;
using SampleWordHelper.Model;

namespace SampleWordHelper.Presentation
{
    /// <summary>
    /// Менеджер отображения представления.
    /// </summary>
    class PreviewPresenter : BasicDisposable, IPreviewPresenter
    {
        /// <summary>
        /// Метод обратного вызова при закрытии представления текущего презентера.
        /// </summary>
        readonly PreviewCallbackFacade callbackFacade;

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
        /// <param name="callbackFacade">Объект обратного вызова.</param>
        public PreviewPresenter(string fileName, IViewFactory viewFactory, PreviewCallbackFacade callbackFacade)
        {
            this.callbackFacade = callbackFacade;
            view = viewFactory.CreatePreviewView(this);
            model = new PreviewModel(fileName);
        }

        /// <summary>
        /// Активирует текущий презентер.
        /// </summary>
        public void Show(bool initialize)
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
            callbackFacade.OnClose(this);
        }

        /**
         * Необходимо вызывать методы именно в такой последовательности, так как для отображения превью тоже используется объект Application.
         * Текущим документом используемого приложения будет документ, используемый для превью. Он недоступен для редактирования и вообще для любых манипуляций, поэтому 
         * попытка что-то вставить в него приведёт к ошибке.
         * Сначала закрывается текущее окно предварительного просмотра, а уже затем вызывается метод вставки содержимого.
         */ 
        void IPreviewPresenter.OnPaste()
        {
            callbackFacade.OnClose(this);
            callbackFacade.OnPaste(model.FileName);
        }

        protected override void DisposeManaged()
        {
            view.SafeDispose();
            model.SafeDispose();
        }
    }
}