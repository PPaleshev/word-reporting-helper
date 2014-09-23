using System;

namespace SampleWordHelper.Model
{
    /// <summary>
    /// Модель данных для вставки документа.
    /// </summary>
    [Serializable]
    public class InsertionData
    {
        /// <summary>
        /// Идентификатор элемента, в котором находится вставляемое содержимое.
        /// </summary>
        public string ItemId { get; private set; }

        /// <summary>
        /// Создаёт новый экземпляр данных.
        /// </summary>
        /// <param name="itemId">Идентификатор перетаскиваемого объекта.</param>
        public InsertionData(string itemId)
        {
            ItemId = itemId;
        }
    }
}