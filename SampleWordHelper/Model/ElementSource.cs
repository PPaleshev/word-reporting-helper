using System.Collections.Generic;
using System.Linq;

namespace SampleWordHelper.Model
{
    /// <summary>
    /// Срез элементов каталога.
    /// </summary>
    public class ElementSource
    {
        /// <summary>
        /// Экземпляр каталога, которому принадлежат данные.
        /// </summary>
        readonly ICatalog catalog;

        /// <summary>
        /// Множество всех элементов, находящихся в текущем срезе.
        /// </summary>
        readonly HashSet<string> entries = new HashSet<string>();

        /// <summary>
        /// Список корневых элементов среза.
        /// </summary>
        readonly List<string> roots = new List<string>();

        /// <summary>
        /// Отображение из идентификатора элемента в список его дочерних элементов.
        /// </summary>
        readonly Dictionary<string, List<string>> hierarchy = new Dictionary<string, List<string>>();

        /// <summary>
        /// Текст установленного фильтра.
        /// </summary>
        readonly IFilterStrategy filterStrategy;

        /// <summary>
        /// Флаг, равный true, если текущий срез каталога не содержит данных, иначе false.
        /// </summary>
        public bool IsEmpty
        {
            get { return entries.Count == 0; }
        }

        /// <summary>
        /// Создаёт новый срез, построенный на фильтре <paramref name="filterStrategy"/>.
        /// </summary>
        /// <param name="catalog">Каталог.</param>
        /// <param name="filterStrategy">Текст фильтра.</param>
        public ElementSource(ICatalog catalog, IFilterStrategy filterStrategy)
        {
            this.catalog = catalog;
            this.filterStrategy = filterStrategy;
            if (filterStrategy.PreserveCatalogStructure)
                FilterPreserveStructure();
            else
                FilterPlain();
        }

        /// <summary>
        /// Возвращает перечисление корневых элементов дерева.
        /// </summary>
        public IEnumerable<string> GetRootElements()
        {
            return roots;
        }

        /// <summary>
        /// Возвращает перечисление идентификаторов дочерних элементов.
        /// </summary>
        /// <param name="parent">Идентификатор родительского элемента.</param>
        public IEnumerable<string> GetChildElements(string parent)
        {
            List<string> child;
            return hierarchy.TryGetValue(parent, out child) ? child : Enumerable.Empty<string>();
        }

        /// <summary>
        /// Возвращает true, если текущий срез содержит указанный идентификатор, иначе false.
        /// </summary>
        /// <param name="id">Идентификатор элемента.</param>
        public bool Contains(string id)
        {
            return entries.Contains(id);
        }

        /// <summary>
        /// Фильтрует все элементы каталога без учёта структуры.
        /// </summary>
        void FilterPlain()
        {
            foreach (var element in catalog.All().Where(id => !catalog.IsGroup(id) && filterStrategy.Satisfies(id)))
            {
                entries.Add(element);
                roots.Add(element);
            }
        }

        /// <summary>
        /// Фильтрует элементы каталога в соответствии с <see cref="filterStrategy"/>.
        /// </summary>
        void FilterPreserveStructure()
        {
            foreach (var root in catalog.GetRootElements().Where(FilterInChild))
            {
                entries.Add(root);
                roots.Add(root);
            }
        }

        /// <summary>
        /// Фильтрует дочерние элементы узла с идентификатором <see cref="parentId"/>.
        /// </summary>
        /// <param name="parentId">Идентификатор родительского элемента.</param>
        /// <returns>Возвращает true, если текущий узел или один из его дочерних удовлетворяет фильтру, иначе false.</returns>
        bool FilterInChild(string parentId)
        {
            var result = false;
            foreach (var childId in catalog.GetChildElements(parentId))
            {
                if (catalog.IsGroup(childId) ? FilterInChild(childId) : filterStrategy.Satisfies(childId))
                    result |= AddToHierarchy(parentId, childId);
            }
            return result;
        }

        /// <summary>
        /// Добавляет элемент во внутренние структуры среза.
        /// </summary>
        /// <param name="parent">Идентификатор родительского элемента.</param>
        /// <param name="elementId">Идентификатор дочернего элемента.</param>
        /// <returns>Возвращает true.</returns>
        bool AddToHierarchy(string parent, string elementId)
        {
            List<string> elements;
            if (!hierarchy.TryGetValue(parent, out elements))
                hierarchy.Add(parent, elements = new List<string>());
            elements.Add(elementId);
            entries.Add(elementId);
            return true;
        }
    }
}
