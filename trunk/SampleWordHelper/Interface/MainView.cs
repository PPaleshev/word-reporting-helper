using SampleWordHelper.Presentation;

namespace SampleWordHelper.Interface
{
    /// <summary>
    /// Основное представление.
    /// </summary>
    public class MainView : IMainView
    {
        /// <summary>
        /// Экземпляр ленты.
        /// </summary>
        readonly MainRibbon ribbon;

        /// <summary>
        /// Менеджер текущего представления.
        /// </summary>
        readonly IMainPresenter presenter;

        /// <summary>
        /// Сообщение над кнопкой настройки по умолчанию.
        /// </summary>
        readonly string defaultSettingsSuperTip;

        /// <summary>
        /// Создаёт новый экземпляр представления.
        /// </summary>
        public MainView(MainRibbon ribbon, IMainPresenter presenter)
        {
            this.ribbon = ribbon;
            this.presenter = presenter;
            this.ribbon.buttonSettings.Click += OnSettingsButtonClick;
            defaultSettingsSuperTip = ribbon.buttonSettings.SuperTip;
        }

        void OnSettingsButtonClick(object sender, Microsoft.Office.Tools.Ribbon.RibbonControlEventArgs e)
        {
            presenter.OnEditSettings();
        }

        public void Dispose()
        {
            ribbon.buttonSettings.Click -= OnSettingsButtonClick;
        }

        public void EnableAddinFeatures(bool enable, string message)
        {
            ribbon.buttonSettings.Image = enable ? Properties.Resources.sprocket_light : Properties.Resources.warning_triangle;
            ribbon.buttonSettings.SuperTip = string.IsNullOrWhiteSpace(message) ? defaultSettingsSuperTip : message;
            ribbon.toggleStructureVisibility.Visible = enable;
            ribbon.separator.Visible = enable;
        }
    }
}
