using System;
using System.Diagnostics;
using System.Windows.Forms;
using Microsoft.Office.Tools;
using SampleWordHelper.Model;
using SampleWordHelper.Presentation;

namespace SampleWordHelper.Interface
{
    /// <summary>
    /// Представление для отображения структуры каталога.
    /// </summary>
    public class StructureTreeView: IStructureView
    {
        /// <summary>
        /// Контрол со структурой.
        /// </summary>
        readonly StructureTreeControl control;

        /// <summary>
        /// Ссылка на контейнер, в коротом расположен контрол.
        /// </summary>
        readonly CustomTaskPane container;

        /// <summary>
        /// Менеджер представления.
        /// </summary>
        readonly IStructurePresenter presenter;

        public StructureTreeView(CustomTaskPane container, IStructurePresenter presenter)
        {
            control = (StructureTreeControl) container.Control;
            this.container = container;
            this.presenter = presenter;
            InitializeComponents();
        }

        public void Initialize(StructureModel model)
        {
            container.Width = 300;
        }

        public void SetVisibility(bool value)
        {
            container.Visible = value;
        }

        public void Dispose()
        {
            container.VisibleChanged -= ContainerVisibilityChanged;
            control.treeStructure.ItemDrag -= OnTreeItemDrag;
            control.treeStructure.QueryContinueDrag -= OnContinueDragRequested;
            control.treeStructure.DragLeave -= OnDraggingLeavedControlBounds;
            control.Dispose();
        }

        /// <summary>
        /// Выполняет инициализацию компонентов представления.
        /// </summary>
        void InitializeComponents()
        {
            container.VisibleChanged += ContainerVisibilityChanged;
            var tree = control.treeStructure;
            tree.ItemDrag += OnTreeItemDrag;
            tree.QueryContinueDrag += OnContinueDragRequested;
            tree.DragLeave += OnDraggingLeavedControlBounds;
        }

        /// <summary>
        /// Вызывается для начала перетаскивания выбранного узла дерева.
        /// </summary>
        public void BeginDragNode(object dragData)
        {
            control.treeStructure.DoDragDrop(dragData, DragDropEffects.Copy);
        }

        /// <summary>
        /// Вызывается при попытке начала перетаскивания узла дерева.
        /// </summary>
        void OnTreeItemDrag(object sender, ItemDragEventArgs e)
        {
            if (e.Button != MouseButtons.Left)
                return;
            var node = (TreeNode) e.Item;
            presenter.DragController.OnBeginDrag(node.Tag);
        }

        /// <summary>
        /// Вызывается при выходе перетаскиваемого объекта за границы дерева со структурой.
        /// </summary>
        void OnDraggingLeavedControlBounds(object sender, EventArgs e)
        {
            presenter.DragController.OnLeave();
        }

        /// <summary>
        /// Вызывается при определении необходимости продолжения текущей операции перетаскивания.
        /// </summary>
        void OnContinueDragRequested(object sender, QueryContinueDragEventArgs e)
        {
            var leftButtonPressed = Control.MouseButtons.HasFlag(MouseButtons.Left);
            e.Action = presenter.DragController.CheckDraggingState(e.EscapePressed, leftButtonPressed);
        }

        /// <summary>
        /// Вызывается при изменении видимости панели со структурой каталога.
        /// </summary>
        void ContainerVisibilityChanged(object sender, EventArgs e)
        {
            if (!container.Visible)
                presenter.OnClosed();
        }
    }
}
