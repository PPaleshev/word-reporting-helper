using System.Runtime.Serialization;

namespace SampleWordHelper.Model
{
    public class DocumentModel : IDocumentModel
    {
        public bool IsStructureVisible { get; set; }

        /// <summary>
        /// Заголовок панели структуры.
        /// </summary>
        public string PaneTitle
        {
            get { return "Структура каталога"; }
        }

        public DocumentModel()
        {
            IsStructureVisible = false;
        }
    }
}
