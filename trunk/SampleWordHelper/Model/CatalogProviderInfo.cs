using System;

namespace SampleWordHelper.Model
{
    /// <summary>
    /// Информация о классе, реализующем доступ к провайдеру каталога.
    /// </summary>
    public class CatalogProviderInfo
    {
        /// <summary>
        /// Уникальный идентификатор провайдера каталога.
        /// </summary>
        public string Id { get; private set; }

        /// <summary>
        /// Экземпляр класса, реализующего провайдер каталога.
        /// </summary>
        public Type Class { get; private set; }

        /// <summary>
        /// Создаёт новый класса, описывающего экземпляр провайдера.
        /// </summary>
        public CatalogProviderInfo(string id, Type @class)
        {
            Id = id;
            Class = @class;
        }
    }
}
