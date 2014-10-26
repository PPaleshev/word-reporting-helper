using System;
using System.Linq;
using System.Text;
using Microsoft.Office.Interop.Word;
using NLog;
using Application = Microsoft.Office.Interop.Word.Application;

namespace SampleWordHelper.Indexation
{
    /// <summary>
    /// Реализация объекта для получения содержимого документов Microsoft Word.
    /// </summary>
    public class WordDocumentContentProvider : IContentProvider
    {
        /// <summary>
        /// Поддержка логирования.
        /// </summary>
        static readonly Logger LOG = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Ссылка на экземпляр приложения.
        /// </summary>
        readonly Application application;

        /// <summary>
        /// Создаёт новый экземпляр объхекта для получения содержимого документов.
        /// </summary>
        /// <param name="application">Экземпляр приложения Microsoft Word.</param>
        public WordDocumentContentProvider(Application application)
        {
            this.application = application;
        }

        public bool TryGetContent(string filePath, out string content)
        {

            try
            {
                content = LoadDocument(filePath);
                return true;
            }
            catch (Exception e)
            {
                LOG.Error("Failed to load content from file: " + filePath, e);
                content = null;
                return false;
            }
        }

        /// <summary>
        /// Извлекает содержимое из документа.
        /// </summary>
        /// <param name="wordSafeFilePath">Безопасный путь к файлу для открытия в MS Word.</param>
        string LoadDocument(string wordSafeFilePath)
        {
            bool isOpened;
            var doc = GetOrOpen(wordSafeFilePath, out isOpened);
            try
            {
                var content = new StringBuilder();
                foreach (var paragraph in doc.Paragraphs.Cast<Paragraph>().Where(p => p.Range.Tables.Count == 0 && !(bool) p.Range.Information[WdInformation.wdWithInTable]))
                {
                    var text = paragraph.Range.Text;
                    content.Append(text);
                    if (!text.EndsWith(Environment.NewLine))
                        content.AppendLine();
                }
                return content.ToString();
            }
            finally
            {
                if (isOpened)
                    doc.Close(false);
            }
        }

        /// <summary>
        /// Возвращает открытый документ или открывает файл для чтения.
        /// </summary>
        /// <param name="filePath">Полный путь к файлу.</param>
        /// <param name="isNew">Флаг, равный true, если файл открыт, а если он уже был открыт, то false.</param>
        Document GetOrOpen(string filePath, out bool isNew)
        {
            var document = application.Documents.Cast<Document>().FirstOrDefault((d => string.Equals(d.FullName, filePath, StringComparison.InvariantCultureIgnoreCase)));
            isNew = document == null;
            return document ?? application.Documents.Open(filePath, Visible: false, ReadOnly: true, OpenAndRepair: false);
        }
    }
}
