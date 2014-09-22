using System.Collections.Generic;
using System.Linq;

namespace SampleWordHelper.Model
{
    /// <summary>
    /// Модель каталога для структурированного представления содержимого базы знаний.
    /// </summary>
    public class CatalogModel
    {
        /// <summary>
        /// Идентификатор корневой группы.
        /// </summary>
        const string ROOT_KEY = "";

        /// <summary>
        /// Отображение из идентификатора группы в список содержащихся в ней элементов.
        /// </summary>
        readonly Dictionary<string, List<CatalogEntry>> entryMap = new Dictionary<string, List<CatalogEntry>>();

        /// <summary>
        /// Добавляет новое описание группы.
        /// </summary>
        /// <param name="id">Идентификатор группы.</param>
        /// <param name="parentId">Ссылка на родителя.</param>
        /// <param name="name">Название группы.</param>
        public void AddGroup(string id, string parentId, string name)
        {
            var group = new CatalogGroup(id, name);
            AddRecord(parentId, group);
        }

        /// <summary>
        /// Добавляет описание элемента.
        /// </summary>
        /// <param name="id">Идентификатор элемента.</param>
        /// <param name="parentId">Идентификатор родительской группы.</param>
        /// <param name="name">Название элемента.</param>
        /// <param name="filePath">Путь к файлу.</param>
        public void AddItem(string id, string parentId, string name, string filePath)
        {
            var item = new CatalogItem(id, name, filePath);
            AddRecord(parentId, item);
        }

        /// <summary>
        /// Возвращает перечисление корневых элементов.
        /// </summary>
        public IEnumerable<CatalogEntry> GetRootEntries()
        {
            return GetChildEntries(ROOT_KEY);
        }

        /// <summary>
        /// Возвращает перечисление дочерних элементов.
        /// </summary>
        /// <param name="parentId">Идентификатор родительской группы.</param>
        public IEnumerable<CatalogEntry> GetChildEntries(string parentId)
        {
            List<CatalogEntry> entries;
            return entryMap.TryGetValue(parentId, out entries) ? new List<CatalogEntry>(entries) : Enumerable.Empty<CatalogEntry>();
        }

        /// <summary>
        /// Добавляет запись во внутреннюю структуру.
        /// </summary>
        void AddRecord(string parentId, CatalogEntry entry)
        {
            parentId = string.IsNullOrWhiteSpace(parentId) ? ROOT_KEY : parentId;
            List<CatalogEntry> list;
            if (!entryMap.TryGetValue(parentId, out list))
                entryMap.Add(parentId, list = new List<CatalogEntry>());
            list.Add(entry);
        }
    }

    /// <summary>
    /// Элемент каталога.
    /// </summary>
    public abstract class CatalogEntry
    {
        /// <summary>
        /// Идентификатор элемента.
        /// </summary>
        public string Id { get; private set; }

        /// <summary>
        /// Название элемента.
        /// </summary>
        public string Name { get; private set; }

        protected CatalogEntry(string id, string name)
        {
            Id = id;
            Name = name;
        }
    }

    /// <summary>
    /// Представляет собой группу элементов каталога.
    /// </summary>
    public class CatalogGroup : CatalogEntry
    {
        public CatalogGroup(string id, string name) : base(id, name)
        {
        }
    }

    /// <summary>
    /// Представляет собой элемент-лист.
    /// </summary>
    public class CatalogItem : CatalogEntry
    {
        /// <summary>
        /// Полный путь к файлу.
        /// </summary>
        public string FullPath { get; private set; }

        public CatalogItem(string id, string name, string fullPath) : base(id, name)
        {
            FullPath = fullPath;
        }
    }
}
