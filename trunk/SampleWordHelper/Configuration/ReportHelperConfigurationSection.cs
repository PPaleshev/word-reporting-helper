using System.Configuration;

namespace SampleWordHelper.Configuration
{
    /// <summary>
    /// Секция параметров приложения.
    /// </summary>
    public class ReportHelperConfigurationSection : ConfigurationSection
    {
        /// <summary>
        /// Название свойства для задания списка провайдеров каталога.
        /// </summary>
        const string CATALOG_PROVIDERS_PROPERTY = "providerFactories";

        /// <summary>
        /// Возвращает коллекцию классов, управляющих схемой доступа к каталогу базы знаний.
        /// </summary>
        [ConfigurationProperty(CATALOG_PROVIDERS_PROPERTY, Options = ConfigurationPropertyOptions.IsTypeStringTransformationRequired)]
        public ProviderFactoryElementCollection ProviderFactories
        {
            get { return (ProviderFactoryElementCollection) base[CATALOG_PROVIDERS_PROPERTY]; }
        }
    }
}
