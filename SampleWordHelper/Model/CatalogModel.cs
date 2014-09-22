using System.Collections.Generic;
using System.IO;

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
        const string ROOT_GROUP = ":ROOT";

        readonly Dictionary<string, List<CatalogEntry>> entryMap = new Dictionary<string, List<CatalogEntry>>();

        public void AddGroup(string id, string parentId, string name)
        {
            var group = new CatalogGroup(id, name);
            AddRecord(parentId, group);
        }

        public void AddItem(string parentId, string id, string name, string filePath)
        {
            var item = new CatalogItem(id, name, filePath);
            AddRecord(parentId, item);
        }

        void AddRecord(string parentId, CatalogEntry entry)
        {
            parentId = string.IsNullOrWhiteSpace(parentId) ? ROOT_GROUP : parentId;
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
