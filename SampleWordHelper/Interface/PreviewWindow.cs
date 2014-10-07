using System;
using System.Drawing;
using System.Windows.Forms;
using SampleWordHelper.Core.Application;
using SampleWordHelper.Presentation;

namespace SampleWordHelper.Interface
{
    /// <summary>
    /// Представление для отображения предварительного просмотра документа.
    /// </summary>
    public partial class PreviewWindow : Form, IPreviewView
    {
        /// <summary>
        /// Менеджер представления.
        /// </summary>
        readonly IPreviewPresenter presenter;

        /// <summary>
        /// Экземпляр родительского окна.
        /// </summary>
        readonly IWin32Window parent;

        /// <summary>
        /// Создаёт новый экземпляр представления.
        /// </summary>
        public PreviewWindow(IPreviewPresenter presenter, IWindowProvider windowProvider)
        {
            this.presenter = presenter;
            InitializeComponent();
            parent = windowProvider.GetMainWindow();
        }

        IntPtr IPreviewView.Handle
        {
            get { return panel1.Handle; }
        }

        public Rectangle PreviewArea
        {
            get { return panel1.ClientRectangle; }
        }

        public void CompleteLoading(bool valid, string message)
        {
            lblMessage.Visible = !valid;
            if (!valid)
                lblMessage.Text = message;
        }

        public void ShowLoading(string caption)
        {
            Text = caption;
            Show(parent);
        }

        /// <summary>
        /// Вызывается при закрытии пользователем окна предварительного просмотра.
        /// </summary>
        void PreviewWindowClosed(object sender, FormClosedEventArgs e)
        {
            presenter.OnClose();
        }

        /// <summary>
        /// Вызывается при изменении размеров области отображения предварительного просмотра.
        /// </summary>
        void PreviewWindowClientSizeChanged(object sender, System.EventArgs e)
        {
            presenter.OnSizeChanged();
        }
    }
}
