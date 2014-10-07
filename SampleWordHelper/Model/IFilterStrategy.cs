using System.Collections.Generic;
using System.Linq;

namespace SampleWordHelper.Model
{
    /// <summary>
    /// Интерфейс стратегии фильтрации данных каталога.
    /// </summary>
    public interface IFilterStrategy
    {
        /// <summary>
        /// Флаг, равный true, если при фильтрации должна сохраняться структура каталога, иначе false.
        /// </summary>
        bool PreserveCatalogStructure { get; }

        /// <summary>
        /// Возвращает true, если элемент с указанным идентификатором удовлетворяет стратегии поиска, иначе false.
        /// </summary>
        /// <param name="elementId">Идентификаторо проверяемого элемента.</param>
        bool Satisfies(string elementId);
    }

    /// <summary>
    /// Пустая стратегия, не выполняющая никаких действий.
    /// </summary>
    public class NullFilterStrategy : IFilterStrategy
    {
        public bool PreserveCatalogStructure
        {
            get { return true; }
        }

        public bool Satisfies(string elementId)
        {
            return true;
        }
    }

    /// <summary>
    /// Стратегия фильтрации элементов каталога по имени документа.
    /// </summary>
    public class ElementNameFilterStrategy : IFilterStrategy
    {
        /// <summary>
        /// Текст фильтра.
        /// </summary>
        readonly string filterText;

        /// <summary>
        /// Экземпляр каталога.
        /// </summary>
        readonly ICatalog catalog;

        /// <summary>
        /// Создаёт новый экземпляр стратегии.
        /// </summary>
        public ElementNameFilterStrategy(ICatalog catalog, string name)
        {
            this.catalog = catalog;
            filterText = (name ?? "").ToLower();
        }

        public bool PreserveCatalogStructure
        {
            get { return true; }
        }

        public bool Satisfies(string elementId)
        {
            var elementName = (catalog.GetName(elementId) ?? "").ToLower();
            return elementName.Contains(filterText);
        }
    }

    /// <summary>
    /// Стратегия фильтрации, использующая поиск по индексированному содержимому документов каталога.
    /// </summary>
    public class ContentFilterStrategy : IFilterStrategy
    {
        /// <summary>
        /// Множество идентификаторов элементов, удовлетворяющих поисковому критерию.
        /// </summary>
        readonly HashSet<string> suitableIds;

        /// <summary>
        /// Создаёт новый экземпляр стратегии.
        /// </summary>
        public ContentFilterStrategy(IEnumerable<string> elementIds)
        {
            suitableIds = new HashSet<string>(elementIds ?? Enumerable.Empty<string>());
        }

        public bool PreserveCatalogStructure
        {
            get { return false; }
        }

        public bool Satisfies(string elementId)
        {
            return suitableIds.Contains(elementId);
        }
    }
}