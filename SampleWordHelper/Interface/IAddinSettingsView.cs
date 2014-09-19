using System;

namespace SampleWordHelper.Interface
{
    /// <summary>
    /// Интерфейс представления для редактирования параметров надстройки.
    /// </summary>
    public interface IAddinSettingsView : IDisposable
    {
        /// <summary>
        /// Открывает представление для редактирования.
        /// </summary>
        /// <returns>Возвращает true, если необходимо сохранить сделанные изменения, иначе false.</returns>
        bool ShowDialog();
    }
}
