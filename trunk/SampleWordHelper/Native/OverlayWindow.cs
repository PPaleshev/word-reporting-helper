using System.Windows.Forms;
using SampleWordHelper.Presentation;

namespace SampleWordHelper.Native
{
    /// <summary>
    /// Вспомогательное окно, используемое для осуществления операции drag'n'drop в рабочую область документа Microsoft Word.
    /// </summary>
    /// <remarks>Создано с использованием примера: http://code.msdn.microsoft.com/Word-2010-Using-the-Drag-81bb5bff </remarks>
    public partial class OverlayWindow : Form
    {
        /// <summary>
        /// Менеджер представления.
        /// </summary>
        readonly IDragDropPresenter presenter;

        /// <summary>
        /// Создаёт новый экземпляр окна.
        /// </summary>
        public OverlayWindow(IDragDropPresenter presenter)
        {
            this.presenter = presenter;
            InitializeComponent();
        }
    }
}
