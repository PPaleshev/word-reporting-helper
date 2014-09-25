using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace SampleWordHelper.Model
{
    /// <summary>
    /// Модель представления для работы со структурой каталога.
    /// </summary>
    public class DocumentModel : IDocumentModel
    {
        public bool IsStructureVisible { get; set; }

        /// <summary>
        /// Заголовок панели структуры.
        /// </summary>
        public string PaneTitle
        {
            get { return "Структура каталога"; }
        }

        /// <summary>
        /// Идентификатор корневого элемента.
        /// </summary>
        static readonly string ROOT_ELEMENT_ID = Guid.NewGuid().ToString();

        /// <summary>
        /// Модель каталога, на основании которого строится модель.
        /// </summary>
        CatalogModel catalog;

        /// <summary>
        /// Отображение из идентификатора элемента в описывающий его объект.
        /// </summary>
        readonly Dictionary<string, CatalogEntry> entries = new Dictionary<string, CatalogEntry>();

        /// <summary>
        /// Описание иерархии элементов.
        /// </summary>
        readonly Dictionary<string, List<string>> hierarchy = new Dictionary<string, List<string>>();

        /// <summary>
        /// Создаёт новый экземпляр модели документа.
        /// </summary>
        public DocumentModel()
        {
            SetModel(new CatalogModel());
        }

        /// <summary>
        /// Устанавливает новую модель каталога.
        /// </summary>
        public void SetModel(CatalogModel catalog)
        {
            this.catalog = catalog;
            hierarchy.Clear();
            entries.Clear();
            FillNodes(ROOT_ELEMENT_ID, catalog.GetRootEntries());
            OrderNodes();
        }

        /// <summary>
        /// Возвращает перечисление идентификаторов дочерних узлов заданного узла.
        /// </summary>
        /// <param name="parentId">Идентификатор родительского узла.</param>
        public IEnumerable<string> GetChildNodes(string parentId)
        {
            List<string> temp;
            return !hierarchy.TryGetValue(parentId, out temp) ? Enumerable.Empty<string>() : new List<string>(temp);
        }

        /// <summary>
        /// Возвращает перечисление идентификаторов корневых элементов.
        /// </summary>
        public IEnumerable<string> GetRootNodes()
        {
            return GetChildNodes(ROOT_ELEMENT_ID);
        }

        /// <summary>
        /// Возвращает текст для узла с заданным идентификатором.
        /// </summary>
        /// <param name="id">Идентификатор узла.</param>
        public string GetText(string id)
        {
            CatalogEntry item;
            return entries.TryGetValue(id, out item) ? item.Name : "";
        }

        /// <summary>
        /// Возвращает текст всплывающей подсказки для узла.
        /// </summary>
        /// <param name="id">Идентификатор узла.</param>
        public string GetHint(string id)
        {
            CatalogEntry entry;
            return entries.TryGetValue(id, out entry) && entry.Kind == EntryKind.ITEM ? ((CatalogItem) entry).FullPath : "";
        }

        /// <summary>
        /// Возвращает тип узла.
        /// </summary>
        /// <param name="id">Идентификатор узла.</param>
        public NodeType GetNodeType(string id)
        {
            CatalogEntry entry;
            if (!entries.TryGetValue(id, out entry))
                return NodeType.NONE;
            return entry.Kind == EntryKind.ITEM ? NodeType.LEAF : NodeType.GROUP;
        }

        /// <summary>
        /// Возвращает true, если узел может быть перетащен, иначе false.
        /// </summary>
        /// <param name="item">Идентификатор элемента.</param>
        public bool CanDragNode(object item)
        {
            var id = item as string;
            if (string.IsNullOrWhiteSpace(id))
                return false;
            CatalogEntry entry;
            return entries.TryGetValue(id, out entry) && entry.Kind == EntryKind.ITEM;
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
            return new CatalogItemDto(id);
        }

        /// <summary>
        /// Извлекает название файла, из которого нужно вставить данные в текущий документ.
        /// </summary>
        public string ExtractFilePathFromTransferredData(IDataObject obj)
        {
            if (!IsValidDropData(obj))
                throw new InvalidOperationException("invalid drop data");
            var dto = obj.GetData(typeof (CatalogItemDto)) as CatalogItemDto;
            var entry = entries[dto.ItemId];
            if (entry.Kind != EntryKind.ITEM)
                throw new InvalidOperationException("failed to drop whole group");
            return ((CatalogItem) entry).FullPath;
        }

        /// <summary>
        /// Определяет, что данные, которые поступили на вход, могут быть приняты и обработаны.
        /// </summary>
        public bool IsValidDropData(IDataObject data)
        {
            if (!data.GetDataPresent(typeof (CatalogItemDto)))
                return false;
            var dto = data.GetData(typeof (CatalogItemDto)) as CatalogItemDto;
            return dto != null && entries.ContainsKey(dto.ItemId);
        }

        /// <summary>
        /// Заполняет внутренние структуры дерева элементов.
        /// </summary>
        void FillNodes(string parentId, IEnumerable<CatalogEntry> items)
        {
            foreach (var item in items)
            {
                entries.Add(item.Id, item);
                List<string> temp;
                if (!hierarchy.TryGetValue(parentId, out temp))
                    hierarchy[parentId] = temp = new List<string>();
                temp.Add(item.Id);
                FillNodes(item.Id, catalog.GetChildEntries(item.Id));
            }
        }

        /// <summary>
        /// Упорядочивает дочерние элементы узлов в порядке их следования.
        /// </summary>
        void OrderNodes()
        {
            foreach (var list in hierarchy.Values)
                list.Sort(NodeIdComparison);
        }

        /// <summary>
        /// Метод упорядочения идентификаторов узлов в порядке их следования в дереве.
        /// </summary>
        int NodeIdComparison(string first, string second)
        {
            var node1 = entries[first];
            var node2 = entries[second];
            var result = node1.Kind.CompareTo(node2.Kind);
            if (result != 0)
                return result;
            return String.Compare(node1.Name, node2.Name, StringComparison.CurrentCultureIgnoreCase);
        }
    }
}
