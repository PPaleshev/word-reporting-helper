using System.Collections.Generic;
using SampleWordHelper.Providers.Core;

namespace SampleWordHelper.Model
{
    public interface ISettingsEditorModel
    {
        /// <summary>
        /// Массив с названиями всех доступных поставщиков.
        /// </summary>
        IEnumerable<ListItem> Factories { get; }

        /// <summary>
        /// Название выбранного поставщика.
        /// Содержит <c>null</c>, если не задан.
        /// </summary>
        string SelectedProviderName { get; }

        /// <summary>
        /// Описание выбранного провайдера.
        /// </summary>
        string SelectedProviderDescription { get; }

        /// <summary>
        /// Модель настроек выбранного поставщика.
        /// </summary>
        ISettingsModel ProviderSettingsModel { get; }
    }
}