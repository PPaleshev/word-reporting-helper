using System;
using System.IO;
using System.Windows.Forms;
using SampleWordHelper.Core.Application;
using SampleWordHelper.Core.Common;
using SampleWordHelper.Interface;
using SampleWordHelper.Model;

namespace SampleWordHelper.Presentation
{
    /// <summary>
    /// Менеджер по управлению предварительным просмотром документов.
    /// </summary>
    public class PreviewManager : BasicDisposable, IPreviewPresenter
    {
        /// <summary>
        /// Фабрика представлений.
        /// </summary>
        readonly IViewFactory viewFactory;

        /// <summary>
        /// Текущее состояние превью.
        /// </summary>
        PreviewState state;

        /// <summary>
        /// Экземпляр отображаемого в текущий момент представление.
        /// </summary>
        IPreviewView view;

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
        public void ShowPreview(string fileName)
        {
            var caption = Path.GetFileName(fileName);
            if (state != null && string.Equals(fileName, state.FileName, StringComparison.InvariantCultureIgnoreCase))
            {
                view.SetCaption(caption);
                view.Show(state.IsValid, state.ErrorMessage);
                return;
            }
            if (state != null)
                Unload();
            view = viewFactory.CreatePreviewView(this);
            view.SetCaption(caption);
            state = new PreviewState(fileName);
            if (state.IsValid)
                state.ShowPreview(view.Handle, view.PreviewArea);
            view.Show(state.IsValid, state.ErrorMessage);
        }

        public void OnSizeChanged()
        {
            state.UpdateSize(view.PreviewArea);
        }

        public void OnClose()
        {
            Unload();
        }

        /// <summary>
        /// Выполняет очистку текущего состояния менеджера.
        /// </summary>
        void Unload()
        {
            if (state != null)
                state.SafeDispose();
            if (view != null)
                view.SafeDispose();
            state = null;
            view = null;
        }

        protected override void DisposeManaged()
        {
            Unload();
        }
    }
}
