using System;

namespace SampleWordHelper.Interface
{
    /// <summary>
    /// Интерфейс основного представления.
    /// </summary>
    public interface IMainView : IDisposable
    {
        /// <summary>
        /// Делает активными возможности надстройки, если <paramref name="enable"/> содержит значение <c>true</c>, в противном случае оставляет только кнопку "Настройки" для повторной инициализации.
        /// </summary>
        /// <param name="enable">True, если возможности доступны, false в противном случае.</param>
        /// <param name="message">Текст сообщения, отображаемого над кнопкой настроек.</param>
        void EnableAddinFeatures(bool enable, string message);

        /// <summary>
        /// В зависимости от значения <paramref name="pressed"/> нажимает или отжимает кнопку, показывающую видимость каталога.
        /// </summary>
        void SetCatalogButtonPressed(bool pressed);
    }
}