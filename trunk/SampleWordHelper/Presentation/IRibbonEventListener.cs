﻿namespace SampleWordHelper.Presentation
{
    /// <summary>
    /// Интерфейс менеджера представления, ответственного за отображение ленты.
    /// </summary>
    public interface IRibbonEventListener
    {
        /// <summary>
        /// Вызывается для переключения видимости панели структуры.
        /// </summary>
        void OnToggleStructureVisibility();
    }
}