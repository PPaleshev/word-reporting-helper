using SampleWordHelper.Model;

namespace SampleWordHelper.Providers.Core
{
    /// <summary>
    /// Интерфейс к модели настроек провайдера.
    /// </summary>
    public interface ISettingsModel
    {
        /// <summary>
        /// Возвращает результат валидации модели.
        /// </summary>
        ValidationResult Validate();
    }
}
