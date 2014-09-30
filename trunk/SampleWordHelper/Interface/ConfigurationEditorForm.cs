using System.Linq;
using System.Windows.Forms;
using SampleWordHelper.Model;
using SampleWordHelper.Native;
using SampleWordHelper.Presentation;

namespace SampleWordHelper.Interface
{
    /// <summary>
    /// Реализация представления для редактирования настроек.
    /// </summary>
    public partial class ConfigurationEditorForm : Form, IConfigurationEditorView
    {
        /// <summary>
        /// Менеджер представления.
        /// </summary>
        readonly IConfigurationEditorPresenter presenter;

        /// <summary>
        /// Модель представления.
        /// </summary>
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
            if (!string.IsNullOrWhiteSpace(model.SelectedStrategyName))
                listProviders.SelectedItem = model.Providers.Single(item => item.Value.Equals(model.SelectedStrategyName));
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
            using (var parent = NativeWindowSession.GetCurrentProcessMainWindow())
                return ShowDialog(parent.window) == DialogResult.OK;
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
