using System;
using SampleWordHelper.Core;
using SampleWordHelper.Interface;
using SampleWordHelper.Model;

namespace SampleWordHelper.Presentation
{
    /// <summary>
    /// Менеджер параметров работы надстройки.
    /// </summary>
    public class SettingsEditorPresenter: ISettingsEditorPresenter
    {
        /// <summary>
        /// Экземпляр представления.
        /// </summary>
        readonly ISettingsEditorView view;

        readonly SettingsEditorModel model;

        public SettingsEditorPresenter(IRuntimeContext context)
        {
            view = context.ViewFactory.CreateSettingsView(this);
            model = new SettingsEditorModel(context.Configuration);
        }

        public void OnSelectedProviderChanged(Item<string> newItem)
        {
            model.UpdateSelectedProvider(newItem);
            view.SetProviderSettings(model.ProviderSettingsModel);
            InvalidateView();
        }

        public void OnPropertyValueChanged()
        {
            InvalidateView();
        }

        /// <summary>
        /// Выполняет старт диалога редактирования настроек.
        /// </summary>
        public void Run()
        {
            view.Initialize(model);
            InvalidateView();
            view.ShowDialog();
        }

        public void Dispose()
        {
            view.Dispose();
        }

        void InvalidateView()
        {
            var result = model.Validate();
            view.SetValid(result.IsValid, result.Message);
        }
    }
}
