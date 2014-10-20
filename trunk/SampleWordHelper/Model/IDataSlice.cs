using System;
using System.Collections.Generic;
using System.Linq;

namespace SampleWordHelper.Model
{
    /// <summary>
    /// Интерфейс среза данных.
    /// </summary>
    public interface IDataSlice
    {   
        /// <summary>
        /// Возвращает перечисление корневых элементов дерева.
        /// </summary>
        IEnumerable<string> GetRootElements();

        /// <summary>
        /// Возвращает перечисление идентификаторов дочерних элементов.
        /// </summary>
        /// <param name="parent">Идентификатор родительского элемента.</param>
        IEnumerable<string> GetChildElements(string parent);

        /// <summary>
        /// Возвращает true, если текущий срез содержит указанный идентификатор, иначе false.
        /// </summary>
        /// <param name="id">Идентификатор элемента.</param>
        bool Contains(string id);

        /// <summary>
        /// Возвращает тип элемента.
        /// </summary>
        /// <param name="id">Идентификатор элемента.</param>
        NodeType GetNodeType(string id);
    }

    /// <summary>
    /// Класс, адаптирующий каталог к срезу данных.
    /// Представляет все данные каталога без фильтрации.
    /// </summary>
    public class CatalogSliceAdapter : IDataSlice, IComparer<string>
    {
        /// <summary>
        /// Каталог, которому делегируются все обращения.
        /// </summary>
        readonly ICatalog catalog;

        /// <summary>
        /// Создаёт новый адаптер данных над каталогом.
        /// </summary>
        /// <param name="catalog">Экземпляр каталога.</param>
        public CatalogSliceAdapter(ICatalog catalog)
        {
            this.catalog = catalog;
        }

        public IEnumerable<string> GetRootElements()
        {
            return catalog.GetRootElements().OrderBy(id => id, this);
        }

        public IEnumerable<string> GetChildElements(string parent)
        {
            return catalog.GetChildElements(parent).OrderBy(id => id, this);
        }

        public bool Contains(string id)
        {
            return catalog.Contains(id);
        }

        public NodeType GetNodeType(string id)
        {
            return catalog.IsGroup(id) ? NodeType.GROUP : NodeType.LEAF;
        }

        int IComparer<string>.Compare(string first, string second)
        {
            var result = catalog.IsGroup(first).CompareTo(catalog.IsGroup(second));
            return result != 0 ? result : string.Compare(catalog.GetName(first), catalog.GetName(second), StringComparison.CurrentCultureIgnoreCase);
        }
    }

    /// <summary>
    /// Срез данных с результатами поиска.
    /// </summary>
    public class FoundDataSlice : IDataSlice
    {
        /// <summary>
        /// Множество идентификаторов элементов, найденных по индексированному содержимому.
        /// </summary>
        readonly HashSet<string> foundBySearch;

        /// <summary>
        /// Множество идентификаторов элементов, найденных по имени.
        /// </summary>
        readonly HashSet<string> foundByName;

        /// <summary>
        /// Список всех элементов в порядке их отображения.
        /// </summary>
        readonly List<string> allElements;

        /// <summary>
        /// Создаёт новый экземпляр среза данных.
        /// </summary>
        public FoundDataSlice(HashSet<string> foundBySearch, HashSet<string> foundByName)
        {
            this.foundBySearch = foundBySearch;
            this.foundByName = foundByName;
            allElements = new List<string>();
            allElements.AddRange(foundBySearch);
            allElements.AddRange(foundByName);
        }

        public IEnumerable<string> GetRootElements()
        {
            return allElements;
        }

        public IEnumerable<string> GetChildElements(string parent)
        {
            return Enumerable.Empty<string>();
        }

        public bool Contains(string id)
        {
            return foundBySearch.Contains(id) || foundByName.Contains(id);
        }

        public NodeType GetNodeType(string id)
        {
            if (foundBySearch.Contains(id))
                return NodeType.INDEXED_SEARCH;
            if (foundByName.Contains(id))
                return NodeType.LEAF;
            return NodeType.NONE;
        }
    }
}