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

        /// <summary>
        /// Ширина панели структуры по умолчанию.
        /// </summary>
        public int DefaultPanelSize
        {
            get { return 300; }
        }

        public DocumentModel()
        {
            IsStructureVisible = false;
        }
    }
}
