using System.Drawing;
using System.Windows.Forms;
using SampleWordHelper.Interface;
using SampleWordHelper.Presentation;

namespace SampleWordHelper.Native
{
    /// <summary>
    /// Вспомогательное окно, используемое для осуществления операции drag'n'drop в рабочую область документа Microsoft Word.
    /// </summary>
    /// <remarks>Создано с использованием примера: http://code.msdn.microsoft.com/Word-2010-Using-the-Drag-81bb5bff </remarks>
    public partial class OverlayWindow : Form, IDropTargetHost 
    {
        /// <summary>
        /// Менеджер представления.
        /// </summary>
        readonly IDropTargetPresenter presenter;

        /// <summary>
        /// Создаёт новый экземпляр окна.
        /// </summary>
        public OverlayWindow(IDropTargetPresenter presenter)
        {
            this.presenter = presenter;
            InitializeComponent();
            AllowTransparency = true;
        }

        /// <summary>
        /// Вызывается при входе курсора в границы контрола в режиме перетаскивания.
        /// </summary>
        void OnDragEnter(object sender, DragEventArgs e)
        {
            e.Effect = presenter.CheckCanAcceptDrop(e.Data, e.AllowedEffect) ? DragDropEffects.Copy : DragDropEffects.None;
        }

        /// <summary>
        /// Вызывается при перемещении курсора в границах контрола в режиме перетаскивания.
        /// </summary>
        void OnDragOver(object sender, DragEventArgs e)
        {
            presenter.OnProcessDragging(e.Data, new Point(e.X, e.Y));
        }

        /// <summary>
        /// Вызывается при покидании курсора границ контрола в режиме перетаскивания.
        /// </summary>
        void OnDraggingLeave(object sender, System.EventArgs e)
        {
            presenter.OnLeave();
        }

        /// <summary>
        /// Вызывается при успешном завершении
        /// </summary>
        void OnDragDrop(object sender, DragEventArgs e)
        {
            presenter.CompleteDrop(e.Data, new Point(e.X, e.Y));
        }
    }
}
