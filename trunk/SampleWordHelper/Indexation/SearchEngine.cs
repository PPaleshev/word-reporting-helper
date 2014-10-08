using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using Lucene.Net.Analysis;
using Lucene.Net.Analysis.Ru;
using Lucene.Net.Documents;
using Lucene.Net.Index;
using Lucene.Net.QueryParsers;
using Lucene.Net.Search;
using Lucene.Net.Store;
using SampleWordHelper.Core.Common;
using SampleWordHelper.Core.IO;
using SampleWordHelper.Model;
using Directory = Lucene.Net.Store.Directory;
using Version = Lucene.Net.Util.Version;

namespace SampleWordHelper.Indexation
{
    /// <summary>
    /// Реализация поискового механизма.
    /// </summary>
    /// TODO добавить данные для определения, какие документы в данный момент есть в индексе. Создавать их при создании индекса и не хранить их в lucene.
    public class SearchEngine : BasicDisposable, ISearchEngine
    {
        /// <summary>
        /// Директория, в которой хранится индекс Lucene.
        /// Во время жизни приложения используется отображение в памяти, по завершению данные могут сохраняться на диск.
        /// </summary>
        readonly Directory indexDirectory;

        /// <summary>
        /// Анализатор, используемый для работы с индексом.
        /// </summary>
        readonly Analyzer analyzer;

        /// <summary>
        /// Объект для вычисления контрольных сумм для документов.
        /// </summary>
        readonly SHA256Managed hasher = new SHA256Managed();

        /// <summary>
        /// Объект для извлечения содержимого из индексируемых файлов.
        /// </summary>
        readonly IContentProvider contentProvider;

        /// <summary>
        /// Создаёт экземпляр поискового механизма.
        /// </summary>
        /// <param name="contentProvider">Объект для извлечения содержимого из индексируемых файлов.</param>
        public SearchEngine(IContentProvider contentProvider)
        {
            this.contentProvider = contentProvider;
            indexDirectory = new RAMDirectory();
            analyzer = new RussianAnalyzer(Version.LUCENE_30);
            hasher.Initialize();
        }

        /// <summary>
        /// Пересоздаёт индекс заново.
        /// </summary>
        /// <param name="catalog">Индексируемый каталог.</param>
        /// <param name="monitor">Объект для отображения прогресса выполнения операции.</param>
        public void BuildIndex(ICatalog catalog, IProgressMonitor monitor)
        {
            using (var indexWriter = new IndexWriter(indexDirectory, analyzer, true, IndexWriter.MaxFieldLength.UNLIMITED))
            {
                var allItems = catalog.All().Where(id => !catalog.IsGroup(id)).ToArray();
                var totalProgress = allItems.Length;
                monitor.UpdateProgress("Инициализация", totalProgress, 0);
                var count = 0;
                foreach (var id in allItems)
                {
                    count++;
                    string content;
                    var location = catalog.GetLocation(id);
                    if (!contentProvider.TryGetContent(location, out content))
                        continue;
                    indexWriter.AddDocument(CreateDocumentForFile(id, location, content));
                    monitor.UpdateProgress("Обработка: " + catalog.GetName(id), totalProgress, count);
                }
                indexWriter.Optimize();
            }
        }

        /// <summary>
        /// Выполняет поиск по запросу в текущем индексе.
        /// </summary>
        /// <param name="input">Поисковой запрос.</param>
        /// <returns>Возвращает массив идентификаторов документов, в которых были найдены совпадения с оригиналом, в порядке увеличения количества совпадений.</returns>
        public string[] Search(string input)
        {
            using (var searcher = new IndexSearcher(indexDirectory, true))
            {
                try
                {
                    var query = new QueryParser(Version.LUCENE_30, "text", analyzer).Parse(input);
                    var result = searcher.Search(query, 10);
                    return result.ScoreDocs.Select(doc => searcher.Doc(doc.Doc).Get("id")).ToArray();
                }
                catch
                {
                    return new string[0];
                }
            }
        }

        /// <summary>
        /// Создаёт индексированный документ для сохранения в индексе.
        /// </summary>
        /// <param name="id">Идентификатор элемента каталога. </param>
        /// <param name="filePath">Путь к файлу.</param>
        /// <param name="content">Содержимое файла.</param>
        Document CreateDocumentForFile(string id, string filePath, string content)
        {
            var doc = new Document();
            doc.Add(new Field("id", id, Field.Store.YES, Field.Index.NO));
            doc.Add(new Field("path", filePath, Field.Store.YES, Field.Index.NO));
            doc.Add(new Field("text", content, Field.Store.NO, Field.Index.ANALYZED));
            using (var stream = LongPathFile.Open(filePath, FileMode.Open, FileAccess.Read))
            {
                doc.Add(new NumericField("size").SetLongValue(stream.Length));
                var hash = hasher.ComputeHash(stream);
                doc.Add(new Field("checksum", Convert.ToBase64String(hash), Field.Store.YES, Field.Index.NO));
            }
            return doc;
        }

        protected override void DisposeManaged()
        {
            analyzer.Dispose();
            indexDirectory.Dispose();
        }
    }
}