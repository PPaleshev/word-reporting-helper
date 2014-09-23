﻿using SampleWordHelper.Core;
using SampleWordHelper.Model;

namespace SampleWordHelper.Interface
{
    /// <summary>
    /// Интерфейс представления структуры каталога.
    /// </summary>
    public interface IStructureView : IView
    {
        /// <summary>
        /// Инициализирует представление данными модели.
        /// </summary>
        void Initialize(StructureModel model);

        /// <summary>
        /// Показывает\скрывает представление в зависимости от переданного флага.
        /// </summary>
        void SetVisibility(bool value);

        /// <summary>
        /// Начинает перетаскивание выбранного узла в дереве.
        /// В качестве перетаскиваемых данных устанавливается <paramref name="dragData"/>.
        /// </summary>
        /// <param name="dragData">Объект с передаваемыми данными.</param>
        void BeginDragNode(object dragData);
    }
}
