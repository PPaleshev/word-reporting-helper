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
        /// Создаёт новый экземпляр представления.
        /// </summary>
        public MainView(MainRibbon ribbon, IMainPresenter presenter)
        {
            this.ribbon = ribbon;
            this.presenter = presenter;
            this.ribbon.buttonSettings.Click += OnSettingsButtonClick;
        }

        void OnSettingsButtonClick(object sender, Microsoft.Office.Tools.Ribbon.RibbonControlEventArgs e)
        {
            presenter.OnEditSettings();
        }

        public void Dispose()
        {
            ribbon.buttonSettings.Click -= OnSettingsButtonClick;
        }
    }
}
