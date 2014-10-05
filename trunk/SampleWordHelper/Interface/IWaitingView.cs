using System;
using SampleWordHelper.Indexation;

namespace SampleWordHelper.Interface
{
    /// <summary>
    /// Интерфейс представления ожидания выполнения операций.
    /// </summary>
    public interface IWaitingView : IProgressMonitor, IDisposable
    {
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