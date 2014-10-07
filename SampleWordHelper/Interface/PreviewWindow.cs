using System;
using System.Drawing;
using System.Windows.Forms;
using SampleWordHelper.Core.Common;
using SampleWordHelper.Core.Native;
using SampleWordHelper.Presentation;

namespace SampleWordHelper.Interface
{
    public partial class PreviewWindow : Form, IPreviewView
    {
        readonly IPreviewPresenter presenter;
        readonly NativeWindowSession.DisposableNativeWindow parent;

        public PreviewWindow(IPreviewPresenter presenter)
        {
            this.presenter = presenter;
            InitializeComponent();
            parent = NativeWindowSession.GetCurrentProcessMainWindow();
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
            BringToFront();
            presenter.OnSizeChanged();
        }

        public void ShowLoading(string caption)
        {
            Text = caption;
            Show(parent.window);
        }

        void PreviewWindowClosed(object sender, FormClosedEventArgs e)
        {
            parent.SafeDispose();
            presenter.OnClose();
        }

        void PreviewWindowClientSizeChanged(object sender, System.EventArgs e)
        {
            presenter.OnSizeChanged();
        }
    }
}
