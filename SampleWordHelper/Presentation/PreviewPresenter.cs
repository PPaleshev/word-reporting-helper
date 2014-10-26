using System.IO;
using SampleWordHelper.Core.Application;
using SampleWordHelper.Core.Common;
using SampleWordHelper.Interface;
using SampleWordHelper.Model;

namespace SampleWordHelper.Presentation
{
    /// <summary>
    /// �������� ����������� �������������.
    /// </summary>
    class PreviewPresenter : BasicDisposable, IPreviewPresenter
    {
        /// <summary>
        /// ����� ��������� ������ ��� �������� ������������� �������� ����������.
        /// </summary>
        readonly PreviewCallbackFacade callbackFacade;

        /// <summary>
        /// ������ ����������.
        /// </summary>
        readonly PreviewModel model;

        /// <summary>
        /// ������� �������������.
        /// </summary>
        readonly IPreviewView view;

        /// <summary>
        /// ������ ����� ��������� ����������.
        /// </summary>
        /// <param name="fileName">���� � �����.</param>
        /// <param name="viewFactory">������� �������������.</param>
        /// <param name="callbackFacade">������ ��������� ������.</param>
        public PreviewPresenter(string fileName, IViewFactory viewFactory, PreviewCallbackFacade callbackFacade)
        {
            this.callbackFacade = callbackFacade;
            view = viewFactory.CreatePreviewView(this);
            model = new PreviewModel(fileName);
        }

        /// <summary>
        /// ���������� ������� ���������.
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
         * ���������� �������� ������ ������ � ����� ������������������, ��� ��� ��� ����������� ������ ���� ������������ ������ Application.
         * ������� ���������� ������������� ���������� ����� ��������, ������������ ��� ������. �� ���������� ��� �������������� � ������ ��� ����� �����������, ������� 
         * ������� ���-�� �������� � ���� ������� � ������.
         * ������� ����������� ������� ���� ���������������� ���������, � ��� ����� ���������� ����� ������� �����������.
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