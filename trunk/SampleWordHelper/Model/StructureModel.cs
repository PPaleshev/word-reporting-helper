using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace SampleWordHelper.Model
{
    /// <summary>
    /// Модель структуры каталога.
    /// </summary>
    public class StructureModel
    {
        /// <summary>
        /// Идентификатор корневого элемента.
        /// </summary>
        const string ROOT_ELEMENT_ID = "";

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

        public StructureModel(CatalogModel catalog)
        {
            SetModel(catalog);
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
        }

        /// <summary>
        /// Возвращает перечисление идентификаторов дочерних узлов заданного узла.
        /// </summary>
        /// <param name="parentId">Идентификатор родительского узла.</param>
        public IEnumerable<string> GetNodeIds(string parentId)
        {
            List<string> temp;
            return !hierarchy.TryGetValue(parentId, out temp) ? Enumerable.Empty<string>() : new List<string>(temp);
        }

        /// <summary>
        /// Возвращает перечисление идентификаторов корневых элементов.
        /// </summary>
        public IEnumerable<string> GetRootNodes()
        {
            return GetNodeIds(ROOT_ELEMENT_ID);
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
    }

    /// <summary>
    /// Тип узла.
    /// </summary>
    public enum NodeType
    {
        /// <summary>
        /// Тип не определён.
        /// </summary>
        NONE,

        /// <summary>
        /// Группа.
        /// </summary>
        GROUP,

        /// <summary>
        /// Листовой узел.
        /// </summary>
        LEAF
    }
}
