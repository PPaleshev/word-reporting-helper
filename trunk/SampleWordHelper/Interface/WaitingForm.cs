using System;
using System.Windows.Forms;
using SampleWordHelper.Core.Application;

namespace SampleWordHelper.Interface
{
    /// <summary>
    /// Экземпляр представления для ожидания операции.
    /// Все методы представления безопасны с точки зрения вызова из не-GUI потоков.
    /// </summary>
    public partial class WaitingForm : Form, IWaitingView
    {
        /// <summary>
        /// Объект для доступа к главному окну приложения.
        /// </summary>
        readonly IWindowProvider parentProvider;

        public WaitingForm(IWindowProvider parentProvider)
        {
            this.parentProvider = parentProvider;
            InitializeComponent();
        }

        public void UpdateProgress(string action, int maxValue, int currentValue)
        {
            if (InvokeRequired)
                Invoke(new Action(() => UpdateProgress(action, maxValue, currentValue)));
            else
            {
                label2.Text = action;
                progressBar1.Maximum = maxValue;
                progressBar1.Value = currentValue;
            }
        }

        void IWaitingView.Show()
        {
            if (InvokeRequired)
                Invoke((Action) (this as IWaitingView).Show);
            else
            {
                StartPosition = FormStartPosition.CenterScreen;
                ShowDialog(parentProvider.GetMainWindow());
            }
        }

        void IWaitingView.Close()
        {
            if (InvokeRequired)
                Invoke((Action) Close);
            else
                Close();
        }
    }
}
