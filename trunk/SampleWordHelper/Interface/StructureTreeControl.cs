using System.Windows.Forms;

namespace SampleWordHelper.Interface
{
    /// <summary>
    /// Реализация представления для просмотра структуры.
    /// </summary>
    public partial class StructureTreeControl : UserControl
    {
        /// <summary>
        /// Создаёт новый экземпляр представления.
        /// </summary>
        public StructureTreeControl()
        {
            InitializeComponent();
        }

        void TreeItemDrag(object sender, ItemDragEventArgs e)
        {
            var node = (TreeNode) e.Item;
            if (node.Nodes.Count == 0)
                DoDragDrop(node, DragDropEffects.All);
        }
    }
}
