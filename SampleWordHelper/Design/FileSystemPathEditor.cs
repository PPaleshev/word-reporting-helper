using System;
using System.ComponentModel;
using System.Drawing.Design;
using System.Windows.Forms;

namespace SampleWordHelper.Design
{
    /// <summary>
    /// Редактор для свойств, содержащих пути в файловой системе.
    /// </summary>
    public class FileSystemPathEditor : UITypeEditor 
    {
        public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
        {
            return UITypeEditorEditStyle.Modal;
        }

        public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
        {
            using (var dialog = new FolderBrowserDialog())
            {
                if (!string.IsNullOrWhiteSpace((string) value))
                    dialog.SelectedPath = (string) value;
                if (dialog.ShowDialog() != DialogResult.OK)
                    return value;
                return dialog.SelectedPath;
            }
        }
    }
}