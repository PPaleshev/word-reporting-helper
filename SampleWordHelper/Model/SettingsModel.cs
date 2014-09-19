using System.Collections.ObjectModel;
using System.Configuration;

namespace SampleWordHelper.Model
{
    /// <summary>
    /// Модель настроек приложения.
    /// </summary>
    public class SettingsModel
    {
        /// <summary>
        /// Флаг, равный true, если модель корректно проинициализирована и содержит все необходимые данные.
        /// </summary>
        public bool IsInitialized { get; private set; }

        /// <summary>
        /// Коллекция описаний обнаруженных в системе провайдеров каталога.
        /// </summary>
        public ReadOnlyCollection<CatalogProviderInfo> Providers { get; private set; }

        /// <summary>
        /// Идентификатор используемого в данный момент провайдера. Может быть не задан.
        /// </summary>
        public string CurrentProviderId { get; private set; }

        SettingsModel()
        {
        }
    }
}
