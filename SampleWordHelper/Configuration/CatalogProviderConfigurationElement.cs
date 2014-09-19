using System;
using System.ComponentModel;
using System.Configuration;

namespace SampleWordHelper.Configuration
{
    /// <summary>
    /// Элемент конфигурации, описывающий компонент для доступа к базе знаний.
    /// </summary>
    public class CatalogProviderConfigurationElement: ConfigurationElement
    {
        /// <summary>
        /// Имя свойства, содержащего псевдоним провайдера.
        /// </summary>
        const string NAME_PROPERTY = "name";

        /// <summary>
        /// Имя свойства, содержащего класс, реализующий провайдер.
        /// </summary>
        const string IMPLEMENTATION_PROPERTY = "implementation";

        /// <summary>
        /// Возвращает название провайдера каталога.
        /// Каждый провайдер имеет уникальное имя.
        /// </summary>
        [ConfigurationProperty(NAME_PROPERTY, IsRequired = true, IsKey = true)]
        public string Name
        {
            get { return (string) base[NAME_PROPERTY]; }
        }

        /// <summary>
        /// Возвращает тип, реализующий управление каталогом.
        /// </summary>
        [ConfigurationProperty(IMPLEMENTATION_PROPERTY, IsRequired = true)]
        [TypeConverter(typeof (TypeNameConverter))]
        public Type Class
        {
            get { return (Type) base[IMPLEMENTATION_PROPERTY]; }
        }
    }
}
