using Microsoft.Office.Interop.Word;

namespace SampleWordHelper.Presentation
{
    /// <summary>
    /// Класс, содержащий методы вставки файлов.
    /// </summary>
    static class PasteMethods
    {
        /// <summary>
        /// Вставка документа из файла с использованием <see cref="Range.InsertFile"/> или <see cref="Range.ImportFragment"/>.
        /// Не сохраняет исходного форматирования.
        /// </summary>
        /// <param name="safeFilePath">Путь к файлу.</param>
        /// <param name="range">Диапазон, в который вставляется документ.</param>
        public static void InsertFile(string safeFilePath, Range range)
        {
            range.ImportFragment(safeFilePath, false);
        }

        /// <summary>
        /// Вставка документа из файла с использованием открытия документа и копирования его содержимого.
        /// </summary>
        /// <param name="safeFilePath">Путь к файлу.</param>
        /// <param name="range">Диапазон, в который вставляется документ.</param>
        public static void OpenAndCopyPaste(string safeFilePath, Range range)
        {
            var application = range.Application;
            application.ScreenUpdating = false;
            try
            {
                var document = application.Documents.Open(safeFilePath, ReadOnly: true, AddToRecentFiles: false, Visible: false, NoEncodingDialog: true);
                document.Content.Copy();
                range.PasteAndFormat(WdRecoveryType.wdFormatOriginalFormatting);
                document.Close(false);
            }
            finally
            {
                application.ScreenUpdating = true;
                application.ScreenRefresh();
            }
        }
    }
}