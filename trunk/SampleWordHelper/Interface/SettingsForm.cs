using System.Windows.Forms;

namespace SampleWordHelper.Interface
{
    public partial class SettingsForm : Form, IAddinSettingsView
    {
        public SettingsForm()
        {
            InitializeComponent();
        }

        bool IAddinSettingsView.ShowDialog()
        {
            return ShowDialog() == DialogResult.OK;
        }
    }
}
