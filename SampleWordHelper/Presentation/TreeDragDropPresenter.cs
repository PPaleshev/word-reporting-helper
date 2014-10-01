using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using Microsoft.Office.Interop.Word;
using SampleWordHelper.Core;
using SampleWordHelper.Core.Application;
using SampleWordHelper.Core.Common;
using SampleWordHelper.Interface;
using SampleWordHelper.Model;
using SampleWordHelper.Native;
using Point = System.Drawing.Point;

namespace SampleWordHelper.Presentation
{
    /// <summary>
    /// Менеджер, управляющий перетаскиванием элементов.
    /// </summary>
    public class TreeDragDropController : IDragSourceController, IDropTargetPresenter
    {
        readonly IRuntimeContext context;
        readonly IDocumentView view;
        readonly DocumentModel model;
        readonly IDropCallback callback;

        IDropTargetHost dropHost;

        State state = State.NONE;

        bool isDraggingOnTarget = false;


        public TreeDragDropController(IRuntimeContext context, IDocumentView view, DocumentModel model, IDropCallback callback)
        {
            this.context = context;
            this.view = view;
            this.model = model;
            this.callback = callback;
        }

        public void Dispose()
        {
            dropHost.SafeDispose();
        }

        void IDragSourceController.OnBeginDrag(object item)
        {
            Debug.WriteLine("Source: BeginDrag");
            if (state != State.NONE)
                throw new InvalidOperationException("invalid state to begin dragging (" + state + ")");
            if (!model.CanDragNode(item))
                return;
            state = State.INITIATED;
            view.BeginDragNode(model.CreateTransferObject(item), DragDropEffects.Copy);
        }

        void IDragSourceController.OnLeave()
        {
            Debug.WriteLine("Source: Leave");
            if (state != State.INITIATED)
                return;
            state = State.DRAGGING;
            CreateAndInitializeHost();
        }

        DragAction IDragSourceController.CheckDraggingState(bool escapePressed, bool leftButtonPressed)
        {
            if (state == State.NONE)
                throw new InvalidOperationException("invalid state to continue dragging (" + state + ")");
            if (!leftButtonPressed && isDraggingOnTarget)
                return DragAction.Drop;
            if (leftButtonPressed && !escapePressed && state != State.INVALID)
                return DragAction.Continue;
            Reset();
            return DragAction.Cancel;
        }

        bool IDropTargetPresenter.CheckCanAcceptDrop(IDataObject data, DragDropEffects sourceEffect)
        {
            Debug.WriteLine("Target: DragEnter");
            isDraggingOnTarget = model.IsValidDropData(data) && sourceEffect.HasFlag(DragDropEffects.Copy);
            Debug.WriteLine("Target: drag accepted = " + isDraggingOnTarget);
            return isDraggingOnTarget;
        }

        void IDropTargetPresenter.OnProcessDragging(IDataObject data, Point point)
        {
            try
            {
                var window = context.Application.ActiveWindow;
                var range = (Range)window.RangeFromPoint(point.X, point.Y);
                range.Select();
            }
            catch (COMException)
            {
                state = State.INVALID;
            }
        }

        void IDropTargetPresenter.OnLeave()
        {
            Debug.WriteLine("Target: Leave");
            if (state != State.DRAGGING)
                throw new InvalidOperationException("invalid state to leave target control (" + state + ")");
            isDraggingOnTarget = false;
        }

        void IDropTargetPresenter.CompleteDrop(IDataObject data, Point point)
        {
            Debug.WriteLine("Target: CompleteDrop");
            if (state != State.DRAGGING)
                throw new InvalidOperationException("invalid state to complete drop (" + state + "");
            Reset();
            try
            {
                callback.OnDrop(data, point);
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
            }
        }

        /// <summary>
        /// Выполняет сброс состояния и очищает ресурсы цели перетаскивания.
        /// </summary>
        void Reset()
        {
            state = State.NONE;
            isDraggingOnTarget = false;
            if (dropHost == null)
                return;
            dropHost.Hide();
            dropHost.SafeDispose();
        }

        /// <summary>
        /// Создаёт хост для принятия перетаскиваемых данных.
        /// </summary>
        void CreateAndInitializeHost()
        {
            var mainWindowTitle = Process.GetCurrentProcess().MainWindowTitle;
            var parent = WinApi.FindMicrosoftWordDocumentWindow(mainWindowTitle);
            if (parent == IntPtr.Zero)
                throw new ApplicationException("failed to find application window");
            Debug.WriteLine("Parent window = {0}", parent);
            var rect = WinApi.GetWindowBounds(parent);
            Debug.WriteLine("CreatedWindow = {0}", rect);
            dropHost = context.ViewFactory.CreateDropHost(this);
            WinApi.SetWindowPos((uint) dropHost.Handle.ToInt32(), parent.ToInt32(), 0, 0, rect.Width, rect.Height, WinApi.SWP_NOACTIVATE | WinApi.SWP_NOZORDER);
            WinApi.SetParent(dropHost.Handle, parent);
            dropHost.Show();
        }

        /// <summary>
        /// Состояние процесса перетаскивания.
        /// </summary>
        enum State
        {
            /// <summary>
            /// Перетаскивание не выполняется.
            /// </summary>
            NONE,

            /// <summary>
            /// Перетаскивание инициировано, но курсор не покинул границы контрола.
            /// </summary>
            INITIATED,

            /// <summary>
            /// Выполняется перетаскивание за пределами источника.
            /// </summary>
            DRAGGING,

            /// <summary>
            /// Возникла ошибка обработки во время перетаскивания. При запросе состояния необходимо отменить передачу.
            /// </summary>
            INVALID 
        }
    }
}