using System;
using System.Windows.Forms;
using SampleWordHelper.Core.Native;

namespace SampleWordHelper.Interface
{
    /// <summary>
    /// Экземпляр представления для ожидания операции.
    /// Все методы представления безопасны с точки зрения вызова из не-GUI потоков.
    /// </summary>
    public partial class WaitingForm : Form, IWaitingView
    {
        public WaitingForm()
        {
            InitializeComponent();
        }

        public void UpdateProgress(string action, int maxValue, int currentValue)
        {
            if (InvokeRequired)
                BeginInvoke(new Action(() => UpdateProgress(action, maxValue, currentValue)));
            else
            {
                label2.Text = action;
                progressBar1.Maximum = maxValue;
                progressBar1.Value = currentValue;
            }
        }

        void IWaitingView.Show()
        {
            using (var window = NativeWindowSession.GetCurrentProcessMainWindow())
            {
                TopLevel = true;
                StartPosition = FormStartPosition.CenterScreen;
                ShowDialog(window.window);
            }
        }

        void IWaitingView.Close()
        {
            if (InvokeRequired)
                BeginInvoke((Action) Close);
            else
                Close();
        }
    }
}
