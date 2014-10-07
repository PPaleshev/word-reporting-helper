using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using ICSharpCode.SharpZipLib.Core;
using SampleWordHelper.Indexation;

namespace SampleWordHelper.Model
{
    /// <summary>
    /// Модель представления для работы со структурой каталога.
    /// </summary>
    public class DocumentModel
    {
        /// <summary>
        /// Механизм поиска по содержимому.
        /// </summary>
        readonly ISearchEngine searchEngine;

        /// <summary>
        /// Объект для сортировки элементов каталога.
        /// </summary>
        Comparer nodeComparer;

        /// <summary>
        /// Модель каталога, на основании которого строится модель.
        /// </summary>
        ICatalog catalog;

        /// <summary>
        /// Текущий источник данных модели.
        /// </summary>
        ElementSource currentSource;

        /// <summary>
        /// Источник элементов, содержащий нефильтрованные данные.
        /// </summary>
        ElementSource nonFilteredSource;

        /// <summary>
        /// Флаг, равный true, если каталог был отображён хотя бы раз.
        /// </summary>
        public bool WasShown { get; set; }

        /// <summary>
        /// Создаёт новый экземпляр модели документа.
        /// </summary>
        public DocumentModel(ICatalog catalog, ISearchEngine searchEngine)
        {
            this.searchEngine = searchEngine;
            SetModel(catalog);
        }

        /// <summary>
        /// Флаг, равный true, если панель с данными видна, иначе false.
        /// </summary>
        public bool IsVisible { get; set; }

        /// <summary>
        /// Возвращает true, если текущая модель с учётом фильтра содержит элементы, иначе false.
        /// </summary>
        public bool HasElements { get; private set; }

        /// <summary>
        /// Заголовок панели структуры.
        /// </summary>
        public string PaneTitle
        {
            get { return "Каталог шаблонов"; }
        }

        /// <summary>
        /// Ширина панели каталога по умолчанию.
        /// </summary>
        public int DefaultWidth
        {
            get { return 400; }
        }

        /// <summary>
        /// Текст для фильтрации элементов дерева.
        /// </summary>
        public string Filter { get; private set; }

        /// <summary>
        /// Возвращает true, если текущая модель содержит отфильтрованные данные, иначе false.
        /// </summary>
        public bool IsFiltered
        {
            get { return !string.IsNullOrWhiteSpace(Filter); }
        }

        /// <summary>
        /// Устанавливает новую модель каталога.
        /// </summary>
        public void SetModel(ICatalog catalog)
        {
            this.catalog = catalog;
            nonFilteredSource = new ElementSource(catalog, new NullFilterStrategy());
            currentSource = GetElementSource(Filter);
            nodeComparer = new Comparer(catalog);
        }

        /// <summary>
        /// Устанавливает фильтр для элементов дерева.
        /// </summary>
        /// <param name="filter">Текст фильтра или пустое значение при его отсутствии.</param>
        public void SetFilter(string filter)
        {
            Filter = filter;
            currentSource = GetElementSource(filter);
        }

        /// <summary>
        /// Возвращает перечисление идентификаторов корневых элементов.
        /// </summary>
        public IEnumerable<string> GetRootNodes()
        {
            return currentSource.GetRootElements().OrderBy(s => s, nodeComparer);
        }

        /// <summary>
        /// Возвращает перечисление идентификаторов дочерних узлов заданного узла.
        /// </summary>
        /// <param name="parentId">Идентификатор родительского узла.</param>
        public IEnumerable<string> GetChildNodes(string parentId)
        {
            return currentSource.GetChildElements(parentId).OrderBy(id => id, nodeComparer);
        }

        /// <summary>
        /// Возвращает текст для узла с заданным идентификатором.
        /// </summary>
        /// <param name="id">Идентификатор узла.</param>
        public string GetText(string id)
        {
            return catalog.GetName(id);
        }

        /// <summary>
        /// Возвращает текст всплывающей подсказки для узла.
        /// </summary>
        /// <param name="id">Идентификатор узла.</param>
        public string GetHint(string id)
        {
            return catalog.IsGroup(id) ? "" : catalog.GetLocation(id);
        }

        /// <summary>
        /// Возвращает тип узла.
        /// </summary>
        /// <param name="id">Идентификатор узла.</param>
        public NodeType GetNodeType(string id)
        {
            return catalog.IsGroup(id) ? NodeType.GROUP : NodeType.LEAF;
        }

        /// <summary>
        /// Возвращает true, если узел может быть перетащен, иначе false.
        /// </summary>
        /// <param name="item">Идентификатор элемента.</param>
        public bool CanDragNode(object item)
        {
            var id = item as string;
            return !string.IsNullOrWhiteSpace(id) && currentSource.Contains(id) && !catalog.IsGroup(id);
        }

        /// <summary>
        /// Возвращает путь к файлу по его идентификатору.
        /// </summary>
        public string GetFilePathForId(object item)
        {
            var id = item as string;
            if (string.IsNullOrWhiteSpace(id) || !currentSource.Contains(id))
                throw new InvalidOperationException();
            return catalog.GetLocation(id);
        }

        /// <summary>
        /// Создаёт транспортный объект для перемещения посредством drag'n'drop.
        /// </summary>
        /// <param name="item">Объект, для которого нужно создать объект передачи.</param>
        public object CreateTransferObject(object item)
        {
            var id = item as string;
            if (string.IsNullOrWhiteSpace(id))
                throw new InvalidOperationException("unable to create trasfer object for specified item");
            return new CatalogItemTransferObject(id);
        }

        /// <summary>
        /// Извлекает название файла, из которого нужно вставить данные в текущий документ.
        /// </summary>
        public string ExtractFilePathFromTransferredData(IDataObject obj)
        {
            if (!IsValidDropData(obj))
                throw new InvalidOperationException("invalid drop data");
            var dto = obj.GetData(typeof (CatalogItemTransferObject)) as CatalogItemTransferObject;
            if (catalog.IsGroup(dto.ItemId))
                throw new InvalidOperationException("failed to drop whole group");
            return catalog.GetLocation(dto.ItemId);
        }

        /// <summary>
        /// Определяет, что данные, которые поступили на вход, могут быть приняты и обработаны.
        /// </summary>
        public bool IsValidDropData(IDataObject data)
        {
            if (!data.GetDataPresent(typeof (CatalogItemTransferObject)))
                return false;
            var dto = data.GetData(typeof (CatalogItemTransferObject)) as CatalogItemTransferObject;
            return dto != null && catalog.Contains(dto.ItemId);
        }

        /// <summary>
        /// Возвращает источник данных для текущего фильтра.
        /// </summary>
        /// <param name="filter">Текст фильтра.</param>
        ElementSource GetElementSource(string filter)
        {
            if (string.IsNullOrWhiteSpace(filter))
                return nonFilteredSource;
            var source = new ElementSource(catalog, new ElementNameFilterStrategy(catalog, filter));
            return !source.IsEmpty ? source : new ElementSource(catalog, new ContentFilterStrategy(searchEngine, filter));
        }

        /// <summary>
        /// Класс для упорядочения элементов в дерева.
        /// </summary>
        sealed class Comparer: IComparer<string>
        {
            /// <summary>
            /// Модель каталога.
            /// </summary>
            readonly ICatalog catalog;

            public Comparer(ICatalog catalog)
            {
                this.catalog = catalog;
            }

            public int Compare(string first, string second)
            {
                var result = catalog.IsGroup(first).CompareTo(catalog.IsGroup(second));
                return result != 0 ? result : string.Compare(catalog.GetName(first), catalog.GetName(second), StringComparison.CurrentCultureIgnoreCase);
            }
        }
    }
}
