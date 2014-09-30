namespace SampleWordHelper.Providers.Core
{
    /// <summary>
    /// Класс, представляющий описание провайдера для редактирования.
    /// </summary>
    public class ProviderDescription
    {
        /// <summary>
        /// Отображаемое название.
        /// </summary>
        public string DisplayName { get; private set; }

        /// <summary>
        /// Описание поставщика.
        /// </summary>
        public string Description { get; private set; }

        /// <summary>
        /// Создаёт новый экземпляр описания провайдера.
        /// </summary>
        public ProviderDescription(string displayName, string description)
        {
            DisplayName = displayName;
            Description = description;
        }
    }
}