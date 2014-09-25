using System;

namespace SampleWordHelper.Model
{
    /// <summary>
    /// DTO объект для передачи данных посредством технологии drag'n'drop.
    /// </summary>
    [Serializable]
    public class CatalogItemDto
    {
        /// <summary>
        /// Идентификатор переносимого элемента.
        /// </summary>
        public string ItemId { get; private set; }

        /// <summary>
        /// Создаёт новый экземпляр DTO.
        /// </summary>
        public CatalogItemDto(string itemId)
        {
            ItemId = itemId;
        }
    }
}