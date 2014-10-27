using System;
using SampleWordHelper.Model;

namespace SampleWordHelper.Presentation
{
    /// <summary>
    /// Интерфейс менеджера представления для редактирования настроек.
    /// </summary>
    public interface IConfigurationEditorPresenter: IDisposable
    {
        /// <summary>
        /// Вызывается при изменении выбранного поставщика.
        /// </summary>
        void OnSelectedProviderChanged(ListItem newListItem);

        /// <summary>
        /// Вызывается при изменении значения редактируемого свойства модели.
        /// </summary>
        void OnPropertyValueChanged();

        /// <summary>
        /// Вызывается при клике на надпись с путём к логам.
        /// </summary>
        /// <param name="type">Тип каталога для открытия.</param>
        void OnOpenAdvancedCatalog(string type);
    }
}