
using System;
using SampleWordHelper.Configuration;
using SampleWordHelper.Core;

namespace SampleWordHelper.Presentation
{
    /// <summary>
    /// Интерфейс главного презентера приложение.
    /// </summary>
    public interface IMainPresenter: IDisposable
    {
        /// <summary>
        /// Ссылка на контекст, в котором работает приложение.
        /// </summary>
        IRuntimeContext Context { get; }

        /// <summary>
        /// Вызывается для отображения настроек приложения.
        /// </summary>
        void OnEditSettings();
    }
}
