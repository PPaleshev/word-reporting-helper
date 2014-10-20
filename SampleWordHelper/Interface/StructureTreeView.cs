using System;
using System.Windows.Forms;
using SampleWordHelper.Core.Application;
using SampleWordHelper.Core.Common;
using SampleWordHelper.Model;
using SampleWordHelper.Presentation;

namespace SampleWordHelper.Interface
{
    /// <summary>
    /// Представление для отображения структуры каталога.
    /// </summary>
    public class StructureTreeView : BasicDisposable, IDocumentView
    {
        /// <summary>
        /// Контрол со структурой.
        /// </summary>
        readonly StructureTreeControl control;

        /// <summary>
        /// Ссылка на контейнер, в коротом расположен контрол.
        /// </summary>
        readonly ManagedTaskPane container;

        /// <summary>
        /// Менеджер представления.
        /// </summary>
        readonly ICatalogPresenter presenter;

        /// <summary>
        /// Флаг для предотвращения обратных вызовов.
        /// </summary>
        readonly SuspendFlag suspendEvents = new SuspendFlag();

        /// <summary>
        /// Контекстное меню каталога.
        /// </summary>
        ContextMenuStrip menu;

        /// <summary>
        /// Создаёт экземпляр представления структуры каталога.
        /// </summary>
        /// <param name="taskPaneFactory">Фабрика панелей задач.</param>
        /// <param name="presenter">Менеджер представления.</param>
        /// <param name="title">Заголовок панели задач.</param>
        public StructureTreeView(TaskPaneFactory taskPaneFactory, ICatalogPresenter presenter, string title)
        {
            control = new StructureTreeControl();
            container = taskPaneFactory.Create(control, title);
            this.presenter = presenter;
            InitializeComponents();
        }

        public void SetContextMenu(ICommandView menu)
        {
            this.menu = (ContextMenuStrip) menu.RawObject;
        }

        public void SetVisibility(bool value)
        {
            container.Visible = value;
        }

        public void SetWidth(int width)
        {
            container.Width = width;
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
            control.textSearch.KeyPress += FilterTextKeyPressed;
            var tree = control.treeStructure;
            tree.ItemDrag += OnTreeItemDrag;
            tree.QueryContinueDrag += OnContinueDragRequested;
            tree.DragLeave += OnDraggingLeavedControlBounds;
            tree.NodeMouseDoubleClick += OnNodeDoubleClicked;
            tree.KeyPress += TreeKeyPressed;
            tree.NodeMouseClick += NodeClicked;
            tree.AfterSelect += OnSelectedNodeChanged;
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
            node.ImageIndex = GetNodeImageIndex(model.GetNodeType(nodeId));
            node.SelectedImageIndex = node.ImageIndex;
            node.ToolTipText = model.GetHint(nodeId);
            node.Tag = nodeId;
            foreach (var childId in model.GetChildNodes(nodeId))
                BuildNode(childId, model, node.Nodes);
            target.Add(node);
        }

        /// <summary>
        /// Определяет индекс картинки, соответствующей типу узла.
        /// </summary>
        static int GetNodeImageIndex(NodeType nodeType)
        {
            switch (nodeType)
            {
                case NodeType.LEAF:
                    return 1;
                case NodeType.GROUP:
                    return 0;
                case NodeType.INDEXED_SEARCH:
                    return 2;
                default:
                    return -1;
            }
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
            if (e.Button != MouseButtons.Left)
                return;
            presenter.OnNodeDoubleClicked(e.Node.Tag);
        }

        /// <summary>
        /// Вызывается при изменении выбранного узла дерева.
        /// </summary>
        void OnSelectedNodeChanged(object sender, TreeViewEventArgs e)
        {
            var item = e.Node == null ? null : e.Node.Tag;
            presenter.OnItemSelected(item);
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
        void FilterTextKeyPressed(object sender, KeyPressEventArgs e)
        {
            switch ((Keys) e.KeyChar)
            {
                case Keys.Escape:
                    using (suspendEvents.Suspend())
                        presenter.OnFilterTextChanged("");
                    e.Handled = true;
                    break;
                case Keys.Enter:
                    SendKeys.Send("{TAB}");
                    e.Handled = true;
                    break;
                default:
                    e.Handled = false;
                    break;
            }
        }

        /// <summary>
        /// Вызывается при нажатии кнопки в дереве.
        /// </summary>
        void TreeKeyPressed(object sender, KeyPressEventArgs e)
        {
            var key = (Keys) e.KeyChar;
            switch (key)
            {
                case Keys.Escape:
                    using (suspendEvents.Suspend())
                        presenter.OnFilterTextChanged("");
                    break;
                case Keys.Enter:
                    var node = control.treeStructure.SelectedNode;
                    if (node.Tag != null)
                        presenter.OnPreviewRequested(node.Tag);
                    break;
                default:
                    control.textSearch.Focus();
                    SendKeys.Send(e.KeyChar.ToString());
                    break;
            }
            e.Handled = true;
        }

        /// <summary>
        /// Вызывается при клике на узел дерева.
        /// </summary>
        void NodeClicked(object sender, TreeNodeMouseClickEventArgs e)
        {
            if (e.Button.HasFlag(MouseButtons.Left) && Control.ModifierKeys.HasFlag(Keys.Control))
                presenter.OnPreviewRequested(e.Node.Tag);
            if (e.Button.HasFlag(MouseButtons.Right))
            {
                control.treeStructure.SelectedNode = e.Node;
                menu.Show(control.treeStructure, e.Location);
            }
        }
    }
} 