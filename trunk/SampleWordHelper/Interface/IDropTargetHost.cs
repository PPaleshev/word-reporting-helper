using System;

namespace SampleWordHelper.Interface
{
    /// <summary>
    /// Интерфейс представления, в которое выполняется перетаскивание.
    /// </summary>
    public interface IDropTargetHost: IDisposable
    {
        /// <summary>
        /// Дескриптор окна представления.
        /// </summary>
        IntPtr Handle { get; }

        /// <summary>
        /// Отображает представление.
        /// </summary>
        void Show();

        /// <summary>
        /// Скрывает представление.
        /// </summary>
        void Hide();
    }
}
