using System;
using System.Drawing;

namespace SampleWordHelper.Interface
{
    /// <summary>
    /// Интерфейс представления предварительного просмотра.
    /// </summary>
    public interface IPreviewView : IDisposable
    {
        /// <summary>
        /// Дескриптор элемента управления, внутри которого будет отображаться предварительный просмотр.
        /// </summary>
        IntPtr Handle { get; }

        /// <summary>
        /// Прямоугольник, описывающий область отображения предварительного просмотра.
        /// </summary>
        Rectangle Bounds { get; }

        /// <summary>
        /// Отображает представление.
        /// </summary>
        void Show();

        /// <summary>
        /// Отображает ошибку предварительного просмотра.
        /// </summary>
        /// <param name="message">Текст ошибки.</param>
        void SetErrorMessage(string message);
    }
}