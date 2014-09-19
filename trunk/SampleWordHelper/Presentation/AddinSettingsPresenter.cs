using System;
using SampleWordHelper.Interface;

namespace SampleWordHelper.Presentation
{
    /// <summary>
    /// Менеджер параметров работы надстройки.
    /// </summary>
    public class AddinSettingsPresenter: IDisposable
    {
        /// <summary>
        /// Экземпляр представления.
        /// </summary>
        readonly IAddinSettingsView view;

        public AddinSettingsPresenter()
        {
            view = new SettingsForm();
        }

        /// <summary>
        /// Выполняет старт диалога редактирования настроек.
        /// </summary>
        public void Run()
        {
//            view.ShowDialog();
        }

        public void Dispose()
        {
            view.Dispose();
        }
    }
}
