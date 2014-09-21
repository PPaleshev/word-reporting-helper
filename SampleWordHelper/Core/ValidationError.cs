namespace SampleWordHelper.Core
{
    /// <summary>
    /// Класс, описывающий ошибку валидации.
    /// </summary>
    public class ValidationError
    {
        /// <summary>
        /// Сообщение, описывающее ошибку.
        /// </summary>
        public string Message { get; private set; }

        /// <summary>
        /// Флаг, равный true, если ошибка критическая, иначе false.
        /// </summary>
        public bool IsCritical { get; private set; }

        public ValidationError(string message, bool isCritical = false)
        {
            Message = message;
            IsCritical = isCritical;
        }
    }
}