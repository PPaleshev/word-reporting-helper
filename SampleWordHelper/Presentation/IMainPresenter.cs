using System;

namespace SampleWordHelper.Presentation
{
    /// <summary>
    /// Интерфейс главного презентера приложение.
    /// </summary>
    public interface IMainPresenter: IDisposable
    {
        /// <summary>
        /// Вызывается для отображения настроек приложения.
        /// </summary>
        void OnEditSettings();

        /// <summary>
        /// Вызывается для перезагрузки содержимого каталога.
        /// </summary>
        void OnUpdateCatalog();

        /// <summary>
        /// Вызывается для изменения видимости панели каталога.
        /// </summary>
        void OnUpdateCatalogVisibility(bool visible);

        /// <summary>
        /// Вызывается при нажатии на кнопку включения\выключения возможностей надстройки.
        /// </summary>
        /// <param name="enabled">True, если надстройка включается, иначе false.</param>
        void OnEnabledChanged(bool enabled);
    }
}
