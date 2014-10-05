using System.Windows.Forms;

namespace SampleWordHelper.Interface
{
    /// <summary>
    /// Экземпляр представления для ожидания операции.
    /// </summary>
    public partial class WaitingForm : Form, IWaitingView
    {
        public WaitingForm()
        {
            InitializeComponent();
        }

        public void UpdateProgress(string action, int maxValue, int currentValue)
        {
            label2.Text = action;
            progressBar1.Maximum = maxValue;
            progressBar1.Value = currentValue;
            Application.DoEvents();
        }

        void IWaitingView.Show()
        {
            TopLevel = true;
            StartPosition = FormStartPosition.CenterScreen;
            Show();
        }

        void IWaitingView.Hide()
        {
            Close();
        }
    }
}
