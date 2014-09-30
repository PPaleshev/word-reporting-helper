using System.Collections.Generic;
using SampleWordHelper.Providers.Core;

namespace SampleWordHelper.Model
{
    public interface ISettingsEditorModel
    {
        /// <summary>
        /// Массив с названиями всех доступных поставщиков.
        /// </summary>
        IEnumerable<ListItem> Providers { get; }

        /// <summary>
        /// Название выбранного поставщика.
        /// Содержит <c>null</c>, если не задан.
        /// </summary>
        string SelectedStrategyName { get; }

        /// <summary>
        /// Модель настроек выбранного поставщика.
        /// </summary>
        ISettingsModel ProviderSettingsModel { get; }

        /// <summary>
        /// Устанавливает новый выбранный провайдер.
        /// </summary>
        void UpdateSelectedProvider(ListItem newItem);
    }
}