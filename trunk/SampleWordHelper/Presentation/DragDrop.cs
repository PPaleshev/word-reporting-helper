using System;
using System.Drawing;
using System.Windows.Forms;

namespace SampleWordHelper.Presentation
{
    /// <summary>
    /// Интерфейс менеджера, реализующего поведение источника при выполнении drag'n'drop.
    /// </summary>
    public interface IDragSourceController : IDisposable
    {
        /// <summary>
        /// Вызывается при запроса начала операции перетаскивания из дерева.
        /// </summary>
        /// <param name="item">Запрашиваемый элемент для перетаскивания.</param>
        void OnBeginDrag(object item);

        /// <summary>
        /// Вызывается при выходе перетаскиваемого объекта за пределы источника.
        /// </summary>
        void OnLeave();

        /// <summary>
        /// Вызывается для определения необходимости продолжения перетаскивания.
        /// </summary>
        /// <param name="escapePresed">Флаг, равный true, если в процессе перетаскивания была нажата клавиша Esc, иначе false.</param>
        /// <param name="keyState">Состояние прочих нажатых клавиш (Ctrl, Alt, Shift).</param>
        /// <returns>Возвращает true, если перетаскивание может быть продолжено, и false, если должно быть отменено.</returns>
        DragAction CheckDraggingState(bool escapePresed, bool keyState);
    }

    /// <summary>
    /// Интерфейс менеджера, реализующего поведение целевого компонента при выполнении drag'n'drop.
    /// </summary>
    public interface IDropTargetPresenter : IDisposable
    {
        /// <summary>
        /// Вызывается при вхождении перетаскивания в границы элемента управления.
        /// </summary>
        /// <param name="data">Контейнер для перетаскиваемых данных.</param>
        /// <param name="sourceEffect">Эффект, предлагаемый источником.</param>
        /// <returns>Возвращает true, если данные могут быть приняты и обработаны, иначе false.</returns>
        bool CheckCanAcceptDrop(IDataObject data, DragDropEffects sourceEffect);

        /// <summary>
        /// Вызывается в процессе нахождения курсора в границах элемента управления при перетаскивании.
        /// </summary>
        /// <param name="data">Контейнер для перетаскиваемых данных.</param>
        /// <param name="point">Экранные координаты точки.</param>
        void OnProcessDragging(IDataObject data, Point point);

        /// <summary>
        /// Вызывается при покидания курсором границ элемента управления при перетаскивании.
        /// </summary>
        void OnLeave();

        /// <summary>
        /// Вызывается при завершении операции перетаскивания.
        /// </summary>
        /// <param name="data">Контейнер с данными.</param>
        /// <param name="point">Экранные координаты точки, в которых было завершено перетаскивание.</param>
        void CompleteDrop(IDataObject data, Point point);
    }

    /// <summary>
    /// Интерфейс обратного вызова для обработки успешного перетаскивания.
    /// </summary>
    public interface IDropCallback
    {
        /// <summary>
        /// Вызывается при успешном перетаскивании указанного элемента каталога.
        /// </summary>
        /// <param name="obj">Перетащенный объект.</param>
        /// <param name="point">Экранные координаты точки, в которой был сброшен объект.</param>
        void OnDrop(IDataObject obj, Point point);
    }
}
