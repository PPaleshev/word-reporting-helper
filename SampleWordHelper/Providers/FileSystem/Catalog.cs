using System;
using System.Collections.Generic;
using System.Linq;
using SampleWordHelper.Model;

namespace SampleWordHelper.Providers.FileSystem
{
    /// <summary>
    /// Модель каталога для структурированного представления содержимого базы знаний.
    /// </summary>
    public class Catalog : ICatalog
    {
        /// <summary>
        /// Идентификатор корневой группы.
        /// </summary>
        const string ROOT_KEY = "";

        /// <summary>
        /// Отображение из идентификатора группы в список содержащихся в ней элементов.
        /// </summary>
        readonly Dictionary<string, List<string>> hierarchy = new Dictionary<string, List<string>>();

        /// <summary>
        /// Отображение из идентификатора элемента в описывающий его объект.
        /// </summary>
        readonly Dictionary<string, CatalogElement> entries = new Dictionary<string, CatalogElement>();

        /// <summary>
        /// Добавляет новое описание группы.
        /// </summary>
        /// <param name="id">Идентификатор группы.</param>
        /// <param name="parentId">Ссылка на родителя.</param>
        /// <param name="name">Название группы.</param>
        /// <param name="path">Путь к группе.</param>
        public void AddGroup(string id, string parentId, string name, string path)
        {
            AddRecord(parentId, id, name, path, ElementKind.GROUP);
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
            AddRecord(parentId, id, name, filePath, ElementKind.ITEM);
        }

        public IEnumerable<string> All()
        {
            return entries.Keys;
        }

        public IEnumerable<string> GetRootElements()
        {
            return GetChildElements(ROOT_KEY);
        }

        public IEnumerable<string> GetChildElements(string parentId)
        {
            List<string> child;
            return hierarchy.TryGetValue(parentId, out child) ? new List<string>(child) : Enumerable.Empty<string>();
        }

        public string GetName(string id)
        {
            return GetElementOrThrow(id).Name;
        }

        public string GetDescription(string id)
        {
            return "";
        }
        
        public string GetLocation(string id)
        {
            return GetElementOrThrow(id).Path;
        }

        public bool IsGroup(string id)
        {
            return GetElementOrThrow(id).Kind == ElementKind.GROUP;
        }

        public bool Contains(string id)
        {
            return entries.ContainsKey(id);
        }

        /// <summary>
        /// Возвращает элемент по его идентификатору или бросает исключение, если такого не найдено.
        /// </summary>
        CatalogElement GetElementOrThrow(string id)
        {
            CatalogElement element;
            if (entries.TryGetValue(id, out element))
                return element;
            throw new ArgumentException("invalid entry id");
        }

        /// <summary>
        /// Добавляет запись во внутреннюю структуру.
        /// </summary>
        void AddRecord(string parentId, string id, string name, string path, ElementKind kind)
        {
            parentId = string.IsNullOrWhiteSpace(parentId) ? ROOT_KEY : parentId;
            List<string> list;
            if (!hierarchy.TryGetValue(parentId, out list))
                hierarchy.Add(parentId, list = new List<string>());
            list.Add(id);
            entries.Add(id, new CatalogElement(id, name, path, kind));
        }
    }

    /// <summary>
    /// Элемент каталога.
    /// </summary>
    sealed class CatalogElement
    {
        /// <summary>
        /// Идентификатор элемента.
        /// </summary>
        public string Id { get; private set; }

        /// <summary>
        /// Название элемента.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Путь к элементу.
        /// </summary>
        public string Path { get; private set; }

        /// <summary>
        /// Вид элемента.
        /// </summary>
        public ElementKind Kind { get; private set; }

        public CatalogElement(string id, string name, string path, ElementKind kind)
        {
            Id = id;
            Name = name;
            Kind = kind;
            Path = path;
        }
    }

    /// <summary>
    /// Тип элемента каталога.
    /// </summary>
    public enum ElementKind
    {
        /// <summary>
        /// Группа.
        /// </summary>
        GROUP,

        /// <summary>
        /// Элемент.
        /// </summary>
        ITEM
    }
}
