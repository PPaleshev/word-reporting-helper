
using SampleWordHelper.Core;

namespace SampleWordHelper.Presentation
{
    /// <summary>
    /// Интерфейс главного презентера надстройки.
    /// </summary>
    public interface IMainPresenter
    {
        /// <summary>
        /// Ссылка на контекст, в котором работает надстройка.
        /// </summary>
        IRuntimeContext Context { get; }

        /// <summary>
        /// Вызывается при нажатии кнопки изменения видимости панели структуры.
        /// </summary>
        void OnToggleStructureVisibility();

        /// <summary>
        /// Вызывается при нажатии на кнопку отображения настроек.
        /// </summary>
        void OnSettingsClicked();
    }
}
