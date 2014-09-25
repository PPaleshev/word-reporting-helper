using Microsoft.Office.Tools.Ribbon;
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
            this.ribbon.buttonReload.Click += OnReloadCatalogButtonClick;
            defaultSettingsSuperTip = ribbon.buttonSettings.SuperTip;
        }

        /// <summary>
        /// Вызывается при нажатии на кнопку отображения настроек.
        /// </summary>
        void OnSettingsButtonClick(object sender, RibbonControlEventArgs e)
        {
            presenter.OnEditSettings();
        }

        /// <summary>
        /// Вызывается при нажатии на кнопку перезагрузки каталога.
        /// </summary>
        void OnReloadCatalogButtonClick(object sender, RibbonControlEventArgs e)
        {
            presenter.OnUpdateCatalog();
        }

        public void Dispose()
        {
            ribbon.buttonSettings.Click -= OnSettingsButtonClick;
        }

        public void EnableAddinFeatures(bool enable, string message)
        {
            ribbon.buttonSettings.Image = enable ? Properties.Resources.settings : Properties.Resources.warning;
            ribbon.buttonSettings.SuperTip = string.IsNullOrWhiteSpace(message) ? defaultSettingsSuperTip : message;
            ribbon.toggleStructureVisibility.Visible = enable;
            ribbon.separator.Visible = enable;
            ribbon.buttonReload.Visible = enable;
        }
    }
}
