using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
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
        /// Модель каталога, на основании которого строится модель.
        /// </summary>
        ICatalog catalog;

        /// <summary>
        /// Текущий источник данных модели.
        /// </summary>
        IDataSlice currentSource;

        /// <summary>
        /// Источник элементов, содержащий нефильтрованные данные.
        /// </summary>
        IDataSlice fullDataSlice;

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
        /// Идентификатор выбранного элемента в дереве.
        /// </summary>
        public object SelectedNodeId { get; set; }

        /// <summary>
        /// Устанавливает новую модель каталога.
        /// </summary>
        public void SetModel(ICatalog catalog)
        {
            this.catalog = catalog;
            fullDataSlice = new CatalogSliceAdapter(catalog);
            currentSource = GetDataSlice(Filter);
            SelectedNodeId = null;
        }

        /// <summary>
        /// Устанавливает фильтр для элементов дерева.
        /// </summary>
        /// <param name="filter">Текст фильтра или пустое значение при его отсутствии.</param>
        public void SetFilter(string filter)
        {
            Filter = filter;
            currentSource = GetDataSlice(filter);
        }

        /// <summary>
        /// Возвращает перечисление идентификаторов корневых элементов.
        /// </summary>
        public IEnumerable<string> GetRootNodes()
        {
            return currentSource.GetRootElements();
        }

        /// <summary>
        /// Возвращает перечисление идентификаторов дочерних узлов заданного узла.
        /// </summary>
        /// <param name="parentId">Идентификатор родительского узла.</param>
        public IEnumerable<string> GetChildNodes(string parentId)
        {
            return currentSource.GetChildElements(parentId);
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
            return currentSource.GetNodeType(id);
        }

        /// <summary>
        /// Возвращает true, если узел может быть перетащен, иначе false.
        /// </summary>
        /// <param name="item">Идентификатор элемента.</param>
        public bool IsContentNode(object item)
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
        IDataSlice GetDataSlice(string filter)
        {
            if (string.IsNullOrWhiteSpace(filter))
                return fullDataSlice;
            var foundBySearch = new HashSet<string>(searchEngine.Search(filter));
            var lowerFilter = filter.ToLower();
            var foundByName = catalog.All().Where(id => !catalog.IsGroup(id) && !foundBySearch.Contains(id) && SatisfiesName(catalog, id, lowerFilter));
            return new FoundDataSlice(foundBySearch, new HashSet<string>(foundByName));
        }

        /// <summary>
        /// Определяет, удовлетворяет ли название элемента каталога указанному фильтру.
        /// </summary>
        /// <param name="catalog">Экземпляр каталога.</param>
        /// <param name="id">Идентификатор анализируемого элемента.</param>
        /// <param name="criteria">Поисковой критерий.</param>
        /// <returns>Возвращает true, если элемент удовлетворяет критерию, иначе false.</returns>
        static bool SatisfiesName(ICatalog catalog, string id, string criteria)
        {
            return string.IsNullOrWhiteSpace(criteria) || (catalog.GetName(id) ?? "").ToLower().Contains(criteria);
        }
    }
}
