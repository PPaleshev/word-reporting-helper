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
        Rectangle PreviewArea { get; }

        /// <summary>
        /// Отображает представление в режиме загрузки.
        /// </summary>
        void ShowLoading(string caption);

        /// <summary>
        /// Завершает загрузку предварительного просмотра и отображает результат.
        /// </summary>
        /// <param name="valid">Флаг, равный false, если в процессе отображения превью возникли ошибки, иначе true.</param>
        /// <param name="message">Текст ошибки, если есть.</param>
        void CompleteLoading(bool valid, string message);
    }
}