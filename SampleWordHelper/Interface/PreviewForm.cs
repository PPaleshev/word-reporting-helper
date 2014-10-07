using System.Windows.Forms;
using SampleWordHelper.Core.Common;

namespace SampleWordHelper.Interface
{
    public partial class PreviewForm : Form
    {
        public PreviewForm()
        {
            InitializeComponent();
            previewHandlerHostControl1.FilePath = @"e:\Projects\understanding WCF extensibility.doc";
        }

        public void CloseALl()
        {
            previewHandlerHostControl1.FilePath = null;
        }
    }
}
