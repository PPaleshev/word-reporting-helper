using System;

namespace SampleWordHelper.Interface
{
    /// <summary>
    /// Интерфейс к представлению списка команд.
    /// </summary>
    public interface ICommandView : IDisposable
    {
        /// <summary>
        /// Возвращает внутреннее представление объекта.
        /// </summary>
        object RawObject { get; }

        /// <summary>
        /// Добавляет команду.
        /// </summary>
        /// <param name="text">Отображаемый текст команды.</param>
        /// <param name="toolTip">Всплывающая подсказка.</param>
        /// <param name="imageKey">Ключ картинки, соответствующий команде.</param>
        /// <param name="isEnabled">Метод определения доступности команды.</param>
        /// <param name="execute">Метод, вызываемый при активации команды.</param>
        void Add(string text, string toolTip, string imageKey, Func<bool> isEnabled, Action execute);
    }
}
