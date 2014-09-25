using Microsoft.Office.Tools.Ribbon;
using SampleWordHelper.Presentation;

namespace SampleWordHelper.Interface
{
    /// <summary>
    /// Представление ленты с основными элементами управления надстройкой.
    /// </summary>
    public class RibbonView : IRibbonView
    {
        /// <summary>
        /// Основной элемент управления ленты.
        /// </summary>
        readonly MainRibbon ribbon;

        /// <summary>
        /// Менеджер представления.
        /// </summary>
        readonly IRibbonEventListener presenter;

        /// <summary>
        /// Флаг для предотвращения обратных вызовов при обработке событий.
        /// </summary>
        readonly SuspendFlag suspendFlag = new SuspendFlag();

        /// <summary>
        /// Создаёт новый экземпляр представления.
        /// </summary>
        public RibbonView(MainRibbon ribbon, IRibbonEventListener presenter)
        {
            this.ribbon = ribbon;
            this.presenter = presenter;
            SetupEvents();
        }

        public void SetValid(bool isValid)
        {
            ribbon.toggleStructureVisibility.Visible = isValid;
        }

        public void SetStructureVisible(bool value)
        {
            if (!suspendFlag)
                ribbon.toggleStructureVisibility.Checked = value;
        }

        public void Dispose()
        {
            ribbon.toggleStructureVisibility.Click -= OnToggleStructureClicked;
        }

        /// <summary>
        /// Выполняет подписку на события.
        /// </summary>
        void SetupEvents()
        {
            ribbon.toggleStructureVisibility.Click += OnToggleStructureClicked;
        }

        /// <summary>
        /// Вызывается при нажатии на кнопке переключения видимости панели структуры.
        /// </summary>
        void OnToggleStructureClicked(object sender, RibbonControlEventArgs e)
        {
            using (suspendFlag.Suspend())
                presenter.OnToggleStructureVisibility();
        }
    }
}
