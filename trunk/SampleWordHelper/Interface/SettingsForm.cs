using System.Diagnostics;
using System.Linq;
using System.Windows.Forms;
using SampleWordHelper.Model;
using SampleWordHelper.Presentation;
using SampleWordHelper.Providers.Core;

namespace SampleWordHelper.Interface
{
    public partial class SettingsForm : Form, ISettingsEditorView
    {
        readonly ISettingsEditorPresenter presenter;

        public SettingsForm(ISettingsEditorPresenter presenter)
        {
            this.presenter = presenter;
            InitializeComponent();
        }

        public void Initialize(SettingsEditorModel model)
        {
            listProviders.DisplayMember = "DisplayName";
            listProviders.Items.AddRange(model.Providers);
            if (!string.IsNullOrWhiteSpace(model.SelectedProviderName))
                listProviders.SelectedItem = model.Providers.Single(item => item.Object.Equals(model.SelectedProviderName));
            propertyGrid1.SelectedObject = model.ProviderSettingsModel;
        }

        public void SetProviderSettings(ISettingsModel model)
        {
            propertyGrid1.SelectedObject = model;
        }

        public void SetValid(bool isValid, string message)
        {
            labelError.Text = message;
            panelError.Visible = !isValid;
            buttonSave.Enabled = isValid;
        }

        bool ISettingsEditorView.ShowDialog()
        {
            var process = Process.GetCurrentProcess();
            var parent = new NativeWindow();
            parent.AssignHandle(process.MainWindowHandle);
            try
            {
                return ShowDialog(parent) == DialogResult.OK;
            }
            finally
            {
                parent.ReleaseHandle();
            }
        }

        void OnSelectedProviderChanged(object sender, System.EventArgs e)
        {
            presenter.OnSelectedProviderChanged((Item<string>) listProviders.SelectedItem);
        }

        void OnEditablePropertyValueChanged(object s, PropertyValueChangedEventArgs e)
        {
            presenter.OnPropertyValueChanged();
        }
    }
}
