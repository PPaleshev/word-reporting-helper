using System;

namespace SampleWordHelper.Interface
{
    /// <summary>
    /// Интерфейс основного представления.
    /// </summary>
    public interface IMainView : IDisposable
    {
        /// <summary>
        /// Делает активными возможности надстройки, если <paramref name="enabled"/> содержит значение <c>true</c>, в противном случае оставляет только кнопку "Настройки" для повторной инициализации.
        /// </summary>
        /// <param name="enabled">True, если возможности доступны, false в противном случае.</param>
        /// <param name="valid">True, если настройки приложения валидны, иначе false.</param>
        /// <param name="message">Текст сообщения, отображаемого над кнопкой настроек.</param>
        void EnableAddinFeatures(bool enabled, bool valid, string message);

        /// <summary>
        /// В зависимости от значения <paramref name="pressed"/> нажимает или отжимает кнопку, показывающую видимость каталога.
        /// </summary>
        void SetCatalogButtonPressed(bool pressed);
    }
}