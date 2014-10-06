using System;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Microsoft.Office.Interop.Word;
using SampleWordHelper.Core.IO;
using Application = Microsoft.Office.Interop.Word.Application;

namespace SampleWordHelper.Indexation
{
    /// <summary>
    /// Реализация объекта для получения содержимого документов Microsoft Word.
    /// </summary>
    public class WordDocumentContentProvider : IContentProvider
    {
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

            using (var safePath = new SafeFilePath(filePath))
            {
                try
                {
                    content = LoadDocument(safePath.FilePath);
                    return true;
                }
                catch (Exception e)
                {
                    MessageBox.Show(filePath + " " + e.Message);
                    //TODO PP: log error.
                    content = null;
                    return false;
                }
            }
        }

        /// <summary>
        /// Читает содержимое документа.
        /// </summary>
        /// <param name="wordSafeFilePath">Безопасный путь к файлу для открытия в MS Word.</param>
        string LoadDocument(string wordSafeFilePath)
        {
            var doc = application.Documents.Open(wordSafeFilePath, Visible: false, ReadOnly: true, OpenAndRepair: false);
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
                doc.Close(false);
            }
        }
    }
}
