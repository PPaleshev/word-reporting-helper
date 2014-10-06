﻿using System;
using System.Windows.Forms;
using Microsoft.Office.Tools;
using SampleWordHelper.Core.Application;
using SampleWordHelper.Core.Common;
using SampleWordHelper.Model;
using SampleWordHelper.Presentation;

namespace SampleWordHelper.Interface
{
    /// <summary>
    /// Представление для отображения структуры каталога.
    /// </summary>
    public class StructureTreeView: BasicDisposable, IDocumentView
    {
        /// <summary>
        /// Контрол со структурой.
        /// </summary>
        readonly StructureTreeControl control;

        /// <summary>
        /// Ссылка на контейнер, в коротом расположен контрол.
        /// </summary>
        readonly ManagedTaskPaneContainer container;

        /// <summary>
        /// Менеджер представления.
        /// </summary>
        readonly ICatalogPresenter presenter;

        /// <summary>
        /// Флаг для предотвращения обратных вызовов.
        /// </summary>
        readonly SuspendFlag suspendEvents = new SuspendFlag();

        public StructureTreeView(ManagedTaskPaneContainer container, ICatalogPresenter presenter)
        {
            control = (StructureTreeControl) container.Control;
            this.container = container;
            this.presenter = presenter;
            InitializeComponents();
        }

        public void SetVisibility(bool value)
        {
            container.Visible = value;
        }

        public void SetFilterText(string filterText)
        {
            using (suspendEvents.Suspend())
                control.textSearch.Text = filterText;
        }

        protected override void DisposeManaged()
        {
            container.VisibleChanged -= ContainerVisibilityChanged;
            container.SafeDispose();
            control.SafeDispose();
        }

        /// <summary>
        /// Выполняет инициализацию компонентов представления.
        /// </summary>
        void InitializeComponents()
        {
            container.VisibleChanged += ContainerVisibilityChanged;
            control.textSearch.TextChanged += SearchFilterChanged;
            control.textSearch.KeyPress += CheckResetSearchFilter;
            var tree = control.treeStructure;
            tree.ItemDrag += OnTreeItemDrag;
            tree.QueryContinueDrag += OnContinueDragRequested;
            tree.DragLeave += OnDraggingLeavedControlBounds;
            tree.NodeMouseDoubleClick += OnNodeDoubleClicked;
            tree.KeyPress += CheckResetSearchFilter;
        }

        /// <summary>
        /// Вызывается для начала перетаскивания выбранного узла дерева.
        /// </summary>
        public void BeginDragNode(object dragData, DragDropEffects effect)
        {
            control.treeStructure.DoDragDrop(dragData, effect);
        }

        public void UpdateStructure(DocumentModel model)
        {
            var tree = control.treeStructure;
            tree.BeginUpdate();
            try
            {
                tree.Nodes.Clear();
                foreach (var nodeId in model.GetRootNodes())
                    BuildNode(nodeId, model, tree.Nodes);
                tree.ExpandAll();
            }
            finally
            {
                tree.EndUpdate();
                if (tree.Nodes.Count > 0)
                    tree.TopNode = tree.SelectedNode = tree.Nodes[0];
            }
        }

        /// <summary>
        /// Строит узел дерева и все его дочерние элементы.
        /// </summary>
        /// <param name="nodeId">Идентификатор строящегося узла.</param>
        /// <param name="model">Модель с данными структуры.</param>
        /// <param name="target">Коллекция, в которую должен быть добавлен строящийся узел.</param>
        static void BuildNode(string nodeId, DocumentModel model, TreeNodeCollection target)
        {
            var node = new TreeNode(model.GetText(nodeId));
            node.ImageIndex = model.GetNodeType(nodeId) == NodeType.LEAF ? 2 : 0;
            node.SelectedImageIndex = node.ImageIndex;
            node.ToolTipText = model.GetHint(nodeId);
            node.Tag = nodeId;
            foreach (var childId in model.GetChildNodes(nodeId))
                BuildNode(childId, model, node.Nodes);
            target.Add(node);
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
        /// Вызывается при двойном клике на узел дерева.
        /// </summary>
        void OnNodeDoubleClicked(object sender, TreeNodeMouseClickEventArgs e)
        {
            if(e.Button!=MouseButtons.Left)
                return;
            presenter.OnNodeDoubleClicked(e.Node.Tag);
        }

        /// <summary>
        /// Вызывается при изменении видимости панели со структурой каталога.
        /// </summary>
        void ContainerVisibilityChanged(object sender, EventArgs e)
        {
            presenter.OnPaneVisibilityChanged(container.Visible);
        }

        /// <summary>
        /// Вызывается при обновлении текста фильтра.
        /// </summary>
        void SearchFilterChanged(object sender, EventArgs e)
        {
            if (suspendEvents)
                return;
            presenter.OnFilterTextChanged(control.textSearch.Text);
        }

        /// <summary>
        /// Вызывается при сбросе активного фильтра.
        /// </summary>
        void CheckResetSearchFilter(object sender, KeyPressEventArgs e)
        {
            if (Keys.Escape != (Keys) e.KeyChar)
                return;
            presenter.OnFilterTextChanged("");
            e.Handled = true;
        }
    }
}