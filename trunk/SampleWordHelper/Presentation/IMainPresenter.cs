﻿
using System;

namespace SampleWordHelper.Presentation
{
    /// <summary>
    /// Интерфейс главного презентера приложение.
    /// </summary>
    public interface IMainPresenter: IDisposable
    {
        /// <summary>
        /// Вызывается для отображения настроек приложения.
        /// </summary>
        void OnEditSettings();
    }
}