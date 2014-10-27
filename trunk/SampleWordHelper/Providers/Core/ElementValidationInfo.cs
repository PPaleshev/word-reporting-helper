namespace SampleWordHelper.Providers.Core
{
    /// <summary>
    /// Информация о валидации элемента каталога.
    /// </summary>
    public sealed class ElementValidationInfo
    {
        /// <summary>
        /// Идентификатор элемента каталога.
        /// </summary>
        public string Id { get; private set; }

        /// <summary>
        /// Сообщение.
        /// </summary>
        public string Message { get; private set; }

        /// <summary>
        /// Флаг, равный true, если элемент описывает ошибку, иначе false.
        /// </summary>
        public bool IsError { get; private set; }

        public ElementValidationInfo(string id, string message, bool isError)
        {
            Id = id;
            Message = message;
            IsError = isError;
        }
    }
}
