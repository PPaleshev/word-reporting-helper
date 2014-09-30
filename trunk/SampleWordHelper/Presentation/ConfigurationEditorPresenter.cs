using SampleWordHelper.Core;
using SampleWordHelper.Interface;
using SampleWordHelper.Model;

namespace SampleWordHelper.Presentation
{
    /// <summary>
    /// Менеджер параметров работы надстройки.
    /// </summary>
    public class ConfigurationEditorPresenter: BasicDisposable, IConfigurationEditorPresenter
    {
        /// <summary>
        /// Экземпляр представления.
        /// </summary>
        readonly IConfigurationEditorView view;

        /// <summary>
        /// Модель для редактирования настроек системы.
        /// </summary>
        readonly ConfigurationEditorModel model;

        public ConfigurationEditorPresenter(IViewFactory viewFactory, ConfigurationEditorModel model)
        {
            view = viewFactory.CreateSettingsView(this);
            this.model = model;
        }

        public void OnSelectedProviderChanged(ListItem newListItem)
        {
            model.UpdateSelectedProvider(newListItem);
            view.UpdateProviderInfo();
            InvalidateView();
        }

        public void OnPropertyValueChanged()
        {
            InvalidateView();
        }

        /// <summary>
        /// Выполняет старт диалога редактирования настроек.
        /// </summary>
        /// <returns>Возвращает true, если настройки провайдера были успешно сохранены, иначе false.</returns>
        public bool Edit()
        {
            view.Initialize(model);
            InvalidateView();
            return view.ShowDialog();
        }

        protected override void DisposeManaged()
        {
            view.SafeDispose();
        }

        void InvalidateView()
        {
            var result = model.Validate();
            view.SetValid(result.IsValid, result.Message);
        }
    }
}
