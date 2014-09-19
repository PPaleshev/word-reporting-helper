using System.Windows.Forms;
using SampleWordHelper.Model;
using SampleWordHelper.Presentation;

namespace SampleWordHelper.Interface
{
    /// <summary>
    /// Реализация представления для просмотра структуры.
    /// </summary>
    public partial class StructureTreeControl : UserControl, IStructureView
    {
        /// <summary>
        /// Менеджер представления.
        /// </summary>
        readonly IStructurePresenter presenter;

        /// <summary>
        /// Создаёт новый экземпляр представления.
        /// </summary>
        public StructureTreeControl(IStructurePresenter presenter) 
        {
            this.presenter = presenter;
            InitializeComponent();
        }

        public object RawObject
        {
            get { return this; }
        }

        public void Initialize(StructureModel model)
        {
//            foreach (var rootItem in model.GetRootItems())
//            {
//                var rootNode = new TreeNode(model.GetText(rootItem));
//                foreach (var child in model.GetChildItems(rootItem))
//                {
//                    var childNode = new TreeNode(model.GetText(child));
//                    rootNode.Nodes.Add(childNode);
//                }
//            }
        }
    }
}
