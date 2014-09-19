using System;
using System.Windows.Forms;

namespace SampleWordHelper.Core
{
    /// <summary>
    /// Общий интерфейс всех представлений.
    /// </summary>
    public interface IView : IDisposable
    {
        /// <summary>
        /// Возвращает внутреннее представление объекта.
        /// Для Windows Forms это может быть <see cref="UserControl"/> или <see cref="Form"/> и т.д.
        /// </summary>
         object RawObject { get; }
    }
}
