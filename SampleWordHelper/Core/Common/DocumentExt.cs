using Microsoft.Office.Interop.Word;
using SampleWordHelper.Core.Application;
using Document = Microsoft.Office.Tools.Word.Document;

namespace SampleWordHelper.Core.Common
{
    /// <summary>
    /// Вспомогательные методы.
    /// </summary>
    public static class DocumentExt
    {
        /// <summary>
        /// Возвращает ключ, уникально идентифицирующий документ vsto.
        /// </summary>
        public static object GetKey(this Document document)
        {
            return document.DocID;
        }

        /// <summary>
        /// Возвращает ключ, уникально идентифицирующий документ word.
        /// </summary>
        public static object GetKey(this _Document document, IRuntimeContext context)
        {
            return GetKey(context.ApplicationFactory.GetVstoObject(document));
        }

        /// <summary>
        /// Возвращает идентификатор активного в данный момент документа.
        /// </summary>
        public static object GetActiveDocumentKey(this IRuntimeContext context)
        {
            return GetKey(context.Application.ActiveDocument, context);
        }
    }
}
