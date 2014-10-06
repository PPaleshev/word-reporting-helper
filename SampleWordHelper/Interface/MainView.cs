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
        /// Флаг для предотвращения обратных вызовов.
        /// </summary>
        readonly SuspendFlag eventsSuspended = new SuspendFlag();

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
            this.ribbon.toggleStructureVisibility.Click += OnToggleCatalogVisibility;
            this.ribbon.toggleEnabled.Click += OnEnabledChanged;
            defaultSettingsSuperTip = ribbon.buttonSettings.SuperTip;
        }

        /// <summary>
        /// Вызывается при изменении активности надстройки.
        /// </summary>
        void OnEnabledChanged(object sender, RibbonControlEventArgs e)
        {
            presenter.OnEnabledChanged(ribbon.toggleEnabled.Checked);
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

        /// <summary>
        /// Вызывается при нажатии на кнопку "Скрыть\показать структуру".
        /// </summary>
        void OnToggleCatalogVisibility(object sender, RibbonControlEventArgs e)
        {
            if (eventsSuspended)
                return;
            presenter.OnUpdateCatalogVisibility(ribbon.toggleStructureVisibility.Checked);
        }

        public void Dispose()
        {
            ribbon.buttonSettings.Click -= OnSettingsButtonClick;
            ribbon.buttonReload.Click -= OnReloadCatalogButtonClick;
        }

        public void EnableAddinFeatures(bool enabled, bool valid, string message)
        {
            ribbon.toggleEnabled.Checked = enabled;
            ribbon.group2.Visible = enabled;

            ribbon.toggleStructureVisibility.Visible = ribbon.buttonReload.Visible = enabled && valid;
            if (!enabled)
                return;
            ribbon.buttonSettings.Image = valid ? Properties.Resources.settings : Properties.Resources.warning;
            ribbon.buttonSettings.SuperTip = string.IsNullOrWhiteSpace(message) ? defaultSettingsSuperTip : message;
        }

        public void SetCatalogButtonPressed(bool pressed)
        {
            using (eventsSuspended.Suspend())
                ribbon.toggleStructureVisibility.Checked = pressed;
        }
    }
}
