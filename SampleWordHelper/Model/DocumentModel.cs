using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace SampleWordHelper.Model
{
    /// <summary>
    /// Модель представления для работы со структурой каталога.
    /// </summary>
    /// TODO: представить некий срез, содержащий определённый набор данных.
    public class DocumentModel 
    {
        /// <summary>
        /// Объект для сортировки элементов каталога.
        /// </summary>
        Comparer nodeComparer;

        /// <summary>
        /// Модель каталога, на основании которого строится модель.
        /// </summary>
        ICatalog catalog;

        /// <summary>
        /// Отфильтрованное представление элементов.
        /// </summary>
        NodeView currentView;

        /// <summary>
        /// Создаёт новый экземпляр модели документа.
        /// </summary>
        public DocumentModel()
        {
            SetModel(new EmptyCatalog());
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
            get { return "Структура каталога"; }
        }

        /// <summary>
        /// Текст для фильтрации элементов дерева.
        /// </summary>
        public string Filter { get; private set; }

        /// <summary>
        /// Устанавливает новую модель каталога.
        /// </summary>
        public void SetModel(ICatalog catalog)
        {
            this.catalog = catalog;
            if (!string.IsNullOrWhiteSpace(Filter))
                currentView = new NodeView(catalog, Filter);
            nodeComparer = new Comparer(catalog);
        }

        /// <summary>
        /// Устанавливает фильтр для элементов дерева.
        /// </summary>
        /// <param name="filter">Текст фильтра или пустое значение при его отсутствии.</param>
        public void UpdateFilter(string filter)
        {
            Filter = filter;
            currentView = string.IsNullOrWhiteSpace(filter) ? null : new NodeView(catalog, filter);
        }

        /// <summary>
        /// Возвращает перечисление идентификаторов корневых элементов.
        /// </summary>
        public IEnumerable<string> GetRootNodes()
        {
            var roots = HasFilter ? currentView.GetRootElements() : catalog.GetRootElements();
            return roots.OrderBy(s => s, nodeComparer);
        }

        /// <summary>
        /// Возвращает перечисление идентификаторов дочерних узлов заданного узла.
        /// </summary>
        /// <param name="parentId">Идентификатор родительского узла.</param>
        public IEnumerable<string> GetChildNodes(string parentId)
        {
            var elements = HasFilter ? currentView.GetChildElements(parentId) : catalog.GetChildElements(parentId);
            return elements.OrderBy(id => id, nodeComparer);
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
            return !string.IsNullOrWhiteSpace(id) && !catalog.IsGroup(id);
        }

        /// <summary>
        /// Возвращает путь к файлу по его идентификатору.
        /// </summary>
        public string GetFilePathForId(object item)
        {
            var id = item as string;
            if (string.IsNullOrWhiteSpace(id) || !(HasFilter ? currentView.Contains(id) : catalog.Contains(id)))
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

        bool HasFilter
        {
            get { return !string.IsNullOrWhiteSpace(Filter); }
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
