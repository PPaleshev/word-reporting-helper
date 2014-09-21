using System.Configuration;

namespace SampleWordHelper.Configuration
{
    /// <summary>
    /// Секция настроек провайдера, хранящего данные о каталоге в файловой системе.
    /// </summary>
    public class FileSystemProviderConfigurationSection : ConfigurationSection
    {
        const string ROOT_PATH_PROPERTY = "rootPath";

        [ConfigurationProperty(ROOT_PATH_PROPERTY, IsRequired = true)]
        public string RootPath
        {
            get { return (string) base[RootPath]; }
            set { base[ROOT_PATH_PROPERTY] = value; }
        }
    }
}