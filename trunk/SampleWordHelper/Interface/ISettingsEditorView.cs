using System;
using SampleWordHelper.Model;
using SampleWordHelper.Providers.Core;

namespace SampleWordHelper.Interface
{
    /// <summary>
    /// Интерфейс представления для редактирования параметров надстройки.
    /// </summary>
    public interface ISettingsEditorView : IDisposable
    {
        /// <summary>
        /// Инициализирует представление.
        /// </summary>
        void Initialize(SettingsEditorModel model);

        /// <summary>
        /// Устанавливает новую модель параметров провайдера.
        /// </summary>
        void SetProviderSettings(ISettingsModel model);

        /// <summary>
        /// Устанавливает результат валидации модели.
        /// </summary>
        /// <param name="isValid"></param>
        /// <param name="message"></param>
        void SetValid(bool isValid, string message);

        /// <summary>
        /// Открывает представление для редактирования.
        /// </summary>
        /// <returns>Возвращает true, если необходимо сохранить сделанные изменения, иначе false.</returns>
        bool ShowDialog();
    }
}
