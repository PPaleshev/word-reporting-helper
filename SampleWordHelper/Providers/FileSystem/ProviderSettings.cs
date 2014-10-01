using System;
using System.Configuration;
using System.IO;
using System.Windows.Forms;
using System.Xml.Serialization;

namespace SampleWordHelper.Providers.FileSystem
{
    [XmlRoot("FSPS")]
    public class ProviderSettings
    {
        /// <summary>
        /// Объект для сериализации настроек.
        /// </summary>
        static readonly XmlSerializer SERIALIZER = new XmlSerializer(typeof (ProviderSettings));

        /// <summary>
        /// Путь к корню каталога.
        /// </summary>
        [XmlElement("Root")]
        public string RootDirectory { get; set; }

        /// <summary>
        /// Флаг, равный true, если при обходе каталога необходимо сохранять ветви, не содержащие файлов, иначе false.
        /// </summary>
        [XmlElement("Materialize")]
        public bool MaterializeEmptyPaths { get; set; }

//        /// <summary>
//        /// Путь к каталогу, в котором будет храниться кэш с файлами.
//        /// </summary>
//        [XmlElement("CacheDir")]
//        [XmlIgnore]
//        public string CacheDirectory { get; set; }
//
//        /// <summary>
//        /// Флаг, равный true, если должно быть использовано локальное кэширование, иначе false.
//        /// </summary>
//        [XmlElement("UseCache")]
//        public bool UseLocalCache { get; set; }

        /// <summary>
        /// Создаёт новый экземпляр настроек.
        /// </summary>
        public ProviderSettings()
        {
            MaterializeEmptyPaths = false;
        }

        /// <summary>
        /// Загружает настройки из строки.
        /// </summary>
        public static ProviderSettings Deserialize(string input)
        {
            using (var reader = new StringReader(input))
                return (ProviderSettings) SERIALIZER.Deserialize(reader);
        }

        /// <summary>
        /// Сериализует содержимое объекта в строку.
        /// </summary>
        public string Serialize()
        {
            var ns = new XmlSerializerNamespaces();
            ns.Add("", "");
            using (var writer = new StringWriter())
            {
                SERIALIZER.Serialize(writer, this, ns);
                writer.Flush();
                return writer.ToString();
            }
        }
    }
}
