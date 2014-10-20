using System;
using System.Collections.Generic;
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
using NLog;
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
    public class SearchEngine : BasicDisposable, ISearchEngine
    {
        /// <summary>
        /// Поддержка логирования.
        /// </summary>
        readonly Logger LOG = LogManager.GetCurrentClassLogger();

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
        /// Отображение из идентификатора элемента каталога в информацию о нём.
        /// </summary>
        readonly Dictionary<string, IndexRecord> records = new Dictionary<string, IndexRecord>();

        /// <summary>
        /// Объект для извлечения содержимого из индексируемых файлов.
        /// </summary>
        readonly IContentProvider contentProvider;

        /// <summary>
        /// Флаг, равный true, если индекс содержит данные, иначе false.
        /// </summary>
        public bool IsCreated
        {
            get { return records.Count > 0; }
        }

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
            LOG.Debug("Creating index");
            using (var indexWriter = new IndexWriter(indexDirectory, analyzer, true, IndexWriter.MaxFieldLength.UNLIMITED))
            {
                var allItems = catalog.All().Where(id => !catalog.IsGroup(id)).ToArray();
                var totalProgress = allItems.Length;
                monitor.UpdateProgress("Инициализация", totalProgress, 0);
                var count = 0;
                foreach (var id in allItems)
                {
                    count++;
                    AddItemToIndex(catalog, id, indexWriter);
                    monitor.UpdateProgress("Обработка: " + catalog.GetName(id), totalProgress, count);
                }
                indexWriter.Optimize();
            }
            LOG.Debug("Index created");
        }

        /// <summary>
        /// Обновляет содержимое индекса на основании данных каталога.
        /// </summary>
        /// <param name="catalog">Индексируемый каталог.</param>
        /// <param name="monitor">Объект для отображения прогресса операции.</param>
        public void UpdateIndex(ICatalog catalog, IProgressMonitor monitor)
        {
            LOG.Debug("Updating existing index");
            List<string> newItems = new List<string>(),
                         changedItems = new List<string>(),
                         outdatedItems = new List<string>();
            monitor.UpdateProgress("Поиск изменений в каталоге", 100, 0);
            CheckIndexRecords(catalog, newItems, changedItems, outdatedItems);
            LOG.Info("New: {0}; Changed: {1}; Outdated: {2}", newItems.Count, changedItems.Count, outdatedItems.Count);
            var totalCount = newItems.Count + changedItems.Count + outdatedItems.Count;
            if(totalCount == 0)
            {
                monitor.UpdateProgress("Индексация завершена", 100, 100);
                return;
            }
            records.Clear();
            monitor.UpdateProgress("Индексация элементов каталога", totalCount, 0);
            using (var writer = new IndexWriter(indexDirectory, analyzer, IndexWriter.MaxFieldLength.UNLIMITED))
            {
                var count = 0;
                AddItems(catalog, newItems, writer);
                monitor.UpdateProgress("Обновление индекса", totalCount, count += newItems.Count);
                UpdateItems(catalog, changedItems, writer);
                monitor.UpdateProgress("Очистка индекса", totalCount, count + changedItems.Count);
                ClearOutdatedItems(outdatedItems, writer);
            }
            monitor.UpdateProgress("Выполнено", totalCount, totalCount);
            LOG.Debug("Index updating complete");
        }

        /// <summary>
        /// Удаляет все устаревшие записи из индекса.
        /// </summary>
        /// <param name="outdatedItems">Перечисление идентификаторов устаревших элементов.</param>
        /// <param name="writer">Объект для записи в индекс.</param>
        static void ClearOutdatedItems(IEnumerable<string> outdatedItems, IndexWriter writer)
        {
            foreach (var term in outdatedItems.Select(id => new Term("id", id)))
                writer.DeleteDocuments(term);
        }

        /// <summary>
        /// Добавляет в индекс новые элементы.
        /// </summary>
        /// <param name="catalog">Каталог.</param>
        /// <param name="newItems">Перечисление идентификаторов элементов, которые нужно проиндексировать.</param>
        /// <param name="writer">Объект для записи в индекс.</param>
        void AddItems(ICatalog catalog, IEnumerable<string> newItems, IndexWriter writer)
        {
            foreach (var newItemId in newItems)
                AddItemToIndex(catalog, newItemId, writer);
        }

        /// <summary>
        /// Обновляет содержимое индекса.
        /// </summary>
        /// <param name="catalog">Каталог.</param>
        /// <param name="changedItems">Перечисление идентификаторов элементов, которые нужно обновить.</param>
        /// <param name="writer">Объект для записи в индекс.</param>
        void UpdateItems(ICatalog catalog, IEnumerable<string> changedItems, IndexWriter writer)
        {
            foreach (var id in changedItems)
            {
                string content;
                if (!contentProvider.TryGetContent(catalog.GetLocation(id), out content))
                    continue;
                var term = new Term("id", id);
                writer.UpdateDocument(term, CreateDocumentForFile(id, catalog.GetLocation(id), content));
            }
        }

        /// <summary>
        /// Добавляет новый элемент в индекс: считывает его содержимое и вычисляет необходимые поля.
        /// </summary>
        /// <param name="catalog">Каталог.</param>
        /// <param name="id">Идентификатор элемента.</param>
        /// <param name="writer">Объект для записи в индекс.</param>
        void AddItemToIndex(ICatalog catalog, string id, IndexWriter writer)
        {
            string content;
            if (!contentProvider.TryGetContent(catalog.GetLocation(id), out content))
                return;
            writer.AddDocument(CreateDocumentForFile(id, catalog.GetLocation(id), content));
        }


        /// <summary>
        /// Соотносит записи в индексе с новым состоянием каталога. 
        /// Заполняет список <paramref name="newItems"/> идентификаторами элементов, которые должны быть добавлены в индекс.
        /// Заполняет список <paramref name="changedItems"/> идентификаторами элементов, которые должны быть обновлены в индексе.
        /// Заполняет список <paramref name="outdatedItems"/> идентификаторами элементов, которые должны быть удалены из индекса.
        /// </summary>
        /// <param name="catalog">Каталог.</param>
        /// <param name="newItems">Список идентификаторов элементов, которые должны быть добавлены.</param>
        /// <param name="changedItems">Список идентификаторов элементов, которые должны быть обновлены.</param>
        /// <param name="outdatedItems">Список идентификаторов элементов, которые должны быть удалены.</param>
        void CheckIndexRecords(ICatalog catalog, List<string> newItems, List<string> changedItems, List<string> outdatedItems)
        {
            var items = catalog.All().Where(id => !catalog.IsGroup(id)).ToArray();
            var temp = new Dictionary<string, IndexRecord>(records);
            foreach (var id in items)
            {
                IndexRecord record;
                if (!temp.TryGetValue(id, out record))
                {
                    newItems.Add(id);
                    continue;
                }
                if (!string.Equals(catalog.GetLocation(id), record.path, StringComparison.InvariantCultureIgnoreCase))
                    changedItems.Add(id);
                using (var stream = LongPathFile.Open(catalog.GetLocation(id), FileMode.Open, FileAccess.Read))
                {
                    if (stream.Length != record.size || !record.hash.SequenceEqual(hasher.ComputeHash(stream)))
                        changedItems.Add(id);
                }
                temp.Remove(id);
            }
            outdatedItems.AddRange(temp.Keys);
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
                    LOG.Info("Found {0} file(s) for query '{1}'", result.ScoreDocs.Length, input);
                    return result.ScoreDocs.Select(doc => searcher.Doc(doc.Doc).Get("id")).ToArray();
                }
                catch
                {
                    LOG.Error("Incorrect search criteria: '{0}'", input);
                    return new string[0];
                }
            }
        }

        /// <summary>
        /// Создаёт индексированный документ для сохранения в индексе.
        /// Добавляет элемент в <see cref="records"/>.
        /// </summary>
        /// <param name="id">Идентификатор элемента каталога. </param>
        /// <param name="filePath">Путь к файлу.</param>
        /// <param name="content">Содержимое файла.</param>
        Document CreateDocumentForFile(string id, string filePath, string content)
        {
            var doc = new Document();
            doc.Add(new Field("id", id, Field.Store.YES, Field.Index.NO));
            doc.Add(new Field("text", content, Field.Store.NO, Field.Index.ANALYZED));
            using (var stream = LongPathFile.Open(filePath, FileMode.Open, FileAccess.Read))
                records.Add(id, new IndexRecord(filePath, stream.Length, hasher.ComputeHash(stream)));
            return doc;
        }

        protected override void DisposeManaged()
        {
            records.Clear();
            analyzer.Dispose();
            indexDirectory.Dispose();
        }

        /// <summary>
        /// Запись с информацией об элементе в индексе.
        /// </summary>
        class IndexRecord
        {
            /// <summary>
            /// Путь к элементу.
            /// </summary>
            public readonly string path;

            /// <summary>
            /// Размер файла элемента.
            /// </summary>
            public readonly long size;

            /// <summary>
            /// Хэш файла.
            /// </summary>
            public readonly byte[] hash;

            public IndexRecord(string path, long size, byte[] hash)
            {
                this.path = path;
                this.size = size;
                this.hash = hash;
            }
        }
    }
}