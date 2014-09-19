using System;

namespace SampleWordHelper.Interface
{
    /// <summary>
    /// Интерфейс представления ленты приложения.
    /// </summary>
    public interface IRibbonView : IDisposable
    {
        /// <summary>
        /// Устанавливает признак нажатия кнопки отображения структуры.
        /// </summary>
        /// <param name="value">Флаг, равный true, если структура должна быть отображена, и false, если скрыта.</param>
        void SetStructureVisible(bool value);
    }
}
