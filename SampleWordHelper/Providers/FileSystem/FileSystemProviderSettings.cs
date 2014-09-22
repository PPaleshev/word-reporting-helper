using System.IO;
using System.Xml.Serialization;

namespace SampleWordHelper.Providers.FileSystem
{
    [XmlRoot("FSPS")]
    public class FileSystemProviderSettings
    {
        /// <summary>
        /// Объект для сериализации настроек.
        /// </summary>
        static readonly XmlSerializer SERIALIZER = new XmlSerializer(typeof (FileSystemProviderSettings));

        /// <summary>
        /// Путь к корню каталога.
        /// </summary>
        [XmlElement("Root")]
        public string RootPath { get; set; }

        /// <summary>
        /// Загружает настройки из строки.
        /// </summary>
        public static FileSystemProviderSettings Deserialize(string input)
        {
            using (var reader = new StringReader(input))
                return (FileSystemProviderSettings) SERIALIZER.Deserialize(reader);
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
