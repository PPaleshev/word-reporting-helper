using System.Configuration;

namespace SampleWordHelper.Configuration
{
    [ConfigurationCollection(typeof (CatalogProviderConfigurationElement))]
    public class CatalogProviderElementCollection : ConfigurationElementCollection
    {
        /// <summary>
        /// Название свойства для указания названия используемого провайдера.
        /// </summary>
        const string CURRENT_PROVIDER_PROPERTY = "current";

        /// <summary>
        /// Возвращает имя активного провайдера.
        /// </summary>
        [ConfigurationProperty(CURRENT_PROVIDER_PROPERTY, IsRequired = false)]
        public string CurrentProviderName
        {
            get { return (string)base[CURRENT_PROVIDER_PROPERTY]; }
            set { base[CURRENT_PROVIDER_PROPERTY] = value; }
        }

        protected override ConfigurationElement CreateNewElement()
        {
            return new CatalogProviderConfigurationElement();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((CatalogProviderConfigurationElement) element).Class.FullName;
        }
    }
}