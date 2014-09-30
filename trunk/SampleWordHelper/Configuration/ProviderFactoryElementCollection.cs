using System.Configuration;

namespace SampleWordHelper.Configuration
{
    /// <summary>
    /// Элемент конфигурации, содержащий список фабрик доступных поставщиков.
    /// </summary>
    [ConfigurationCollection(typeof (ProviderFactoryConfigurationElement))]
    public class ProviderFactoryElementCollection : ConfigurationElementCollection
    {
        protected override ConfigurationElement CreateNewElement()
        {
            return new ProviderFactoryConfigurationElement();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((ProviderFactoryConfigurationElement) element).Class.FullName;
        }
    }
}