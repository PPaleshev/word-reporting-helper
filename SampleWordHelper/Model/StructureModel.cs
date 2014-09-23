using System.Windows.Forms;

namespace SampleWordHelper.Model
{
    /// <summary>
    /// Модель структуры каталога.
    /// </summary>

    public class StructureModel
    {
        /// <summary>
        /// Модель каталога, на основании которого строится модель.
        /// </summary>
        readonly CatalogModel catalogModel;

        public StructureModel(CatalogModel catalogModel)
        {
            this.catalogModel = catalogModel;
        }

        public bool CanDragNode(object item)
        {
            return true;
        }

        public object CreateTransferObject(object item)
        {
            return "text";
        }

        public bool IsValidDropData(IDataObject data)
        {
            return true;
        }
    }
}
