using System.Diagnostics;
using System.Linq;
using System.Windows.Forms;
using SampleWordHelper.Model;
using SampleWordHelper.Presentation;
using SampleWordHelper.Providers.Core;

namespace SampleWordHelper.Interface
{
    public partial class ConfigurationEditorForm : Form, IConfigurationEditorView
    {
        readonly IConfigurationEditorPresenter presenter;
        ISettingsEditorModel model;

        public ConfigurationEditorForm(IConfigurationEditorPresenter presenter)
        {
            this.presenter = presenter;
            InitializeComponent();
        }

        public void Initialize(ISettingsEditorModel model)
        {
            this.model = model;
            listProviders.DisplayMember = "DisplayName";
            listProviders.Items.AddRange(model.Providers.ToArray());
            if (!string.IsNullOrWhiteSpace(model.SelectedProviderName))
                listProviders.SelectedItem = model.Providers.Single(item => item.Value.Equals(model.SelectedProviderName));
            propertyGrid1.SelectedObject = model.ProviderSettingsModel;
        }

        public void UpdateProviderSettings()
        {
            propertyGrid1.SelectedObject = model.ProviderSettingsModel;
        }

        public void SetValid(bool isValid, string message)
        {
            labelError.Text = message;
            panelError.Visible = !isValid;
            buttonSave.Enabled = isValid;
            buttonSave.DialogResult = isValid ? DialogResult.OK : DialogResult.None;
        }

        bool IConfigurationEditorView.ShowDialog()
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
            presenter.OnSelectedProviderChanged((ListItem) listProviders.SelectedItem);
        }

        void OnEditablePropertyValueChanged(object s, PropertyValueChangedEventArgs e)
        {
            presenter.OnPropertyValueChanged();
        }
    }
}
