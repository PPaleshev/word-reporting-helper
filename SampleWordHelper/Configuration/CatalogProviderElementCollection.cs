using System.Configuration;

namespace SampleWordHelper.Configuration
{
    [ConfigurationCollection(typeof (CatalogProviderConfigurationElement))]
    public class CatalogProviderElementCollection : ConfigurationElementCollection
    {
        protected override ConfigurationElement CreateNewElement()
        {
            return new CatalogProviderConfigurationElement();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((CatalogProviderConfigurationElement) element).Name;
        }
    }
}